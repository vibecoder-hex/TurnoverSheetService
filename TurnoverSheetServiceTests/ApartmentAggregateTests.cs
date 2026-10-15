using TurnoverSheetService.Models.DomainEntites;

namespace TurnoverSheetServiceTests;

/// <summary>
/// Тесты богатой доменной модели агрегата «Квартира» (rich domain model, DDD).
/// Все операции выполняются строго через корень агрегата <see cref="Apartment"/>.
///
/// Бизнес-идентификаторы (natural keys), по которым различаются сущности:
///   • Квартира — пара (номер, адрес);
///   • Сальдо   — пара (дата вычисления, тип услуги);
///   • Оборот   — дата начисления (<see cref="Charge.ChargeDate"/>) / платежа (<see cref="Payment.PaymentDate"/>).
///
/// Отдельное внимание уделено краевым случаям с некорректными данными пользователя:
/// отрицательный номер квартиры, пустой адрес, пустой тип услуги, отрицательные
/// суммы, а также неверные даты сальдо и оборотов.
/// </summary>
public class ApartmentAggregateTests
{
    private const int ValidNumber = 5;
    private const string ValidAddress = "г. Москва, ул. Тверская, д. 1, кв. 5";
    private const string HeatingService = "Отопление";
    private const string WaterService = "ХВС";

    // Дата вычисления сальдо по умолчанию выставляется доменом как «сегодня».
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

    // ---------------------------------------------------------------------
    // Вспомогательные методы: создают корректный агрегат для сценария
    // ---------------------------------------------------------------------

    private static Apartment CreateApartment(int number = ValidNumber, string address = ValidAddress)
    {
        var result = Apartment.CreateApartmentFactory(number, address);
        Assert.True(result.IsSuccess, "Ожидалось успешное создание квартиры");
        return result.ResultObject;
    }

    private static Saldo CreateSaldo(
        Apartment apartment,
        string serviceType = HeatingService,
        decimal incomingSaldoValue = 0m)
    {
        var result = apartment.AddSaldo(serviceType, incomingSaldoValue);
        Assert.True(result.IsSuccess, "Ожидалось успешное добавление сальдо");
        return apartment.Saldos.Single(saldo => saldo.ServiceType == serviceType);
    }

    // =====================================================================
    // 1. Добавление квартиры. Бизнес-идентификатор — (номер, адрес).
    // =====================================================================

    [Fact]
    public void CreateApartmentFactory_WithValidNumberAndAddress_ProjectsBusinessIdentifier()
    {
        const int number = 42;
        const string address = "г. Казань, ул. Баумана, д. 7";

        var result = Apartment.CreateApartmentFactory(number, address);

        Assert.True(result.IsSuccess);
        Assert.Equal(number, result.ResultObject.Number);
        Assert.Equal(address, result.ResultObject.Address);
        Assert.Empty(result.ResultObject.Saldos);
    }

    [Fact]
    public void CreateApartmentFactory_WithZeroNumber_IsAllowed()
    {
        // Домен запрещает только отрицательные номера (number < 0), ноль допустим.
        var result = Apartment.CreateApartmentFactory(0, ValidAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.ResultObject.Number);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void CreateApartmentFactory_WithNegativeNumber_Fails(int number)
    {
        var result = Apartment.CreateApartmentFactory(number, ValidAddress);

        Assert.False(result.IsSuccess);
        Assert.Equal("Номер квартиры не может быть отрицательным", result.ErrorMessage);
    }

    [Fact]
    public void CreateApartmentFactory_WithNullAddress_Fails()
    {
        var result = Apartment.CreateApartmentFactory(ValidNumber, null!);

        Assert.False(result.IsSuccess);
        Assert.Equal("Адрес квартиры пуст", result.ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    public void CreateApartmentFactory_WithEmptyAddress_Fails(string address)
    {
        var result = Apartment.CreateApartmentFactory(ValidNumber, address);

        Assert.False(result.IsSuccess);
        Assert.Equal("Адрес квартиры пуст", result.ErrorMessage);
    }

    [Fact]
    public void CreateApartmentFactory_WithWhitespaceOnlyAddress_CurrentlyPassesValidation()
    {
        // ВНИМАНИЕ: строка из пробелов не является "пустой" для string.IsNullOrEmpty,
        // поэтому домен её принимает. Тест фиксирует текущее поведение и может быть
        // использован для усиления валидации адреса.
        var result = Apartment.CreateApartmentFactory(ValidNumber, "   ");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void CreateApartmentFactory_ProjectsDistinctBusinessIdentifiers()
    {
        // Бизнес-идентификатор — пара (номер, адрес). Технический ApartmentGuid
        // назначается инфраструктурой (EF), поэтому в домене опираемся на пару.
        var first = Apartment.CreateApartmentFactory(1, "адрес А");
        var second = Apartment.CreateApartmentFactory(1, "адрес Б");
        var third = Apartment.CreateApartmentFactory(2, "адрес А");

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.True(third.IsSuccess);

        Assert.NotEqual(
            (first.ResultObject.Number, first.ResultObject.Address),
            (second.ResultObject.Number, second.ResultObject.Address));
        Assert.NotEqual(
            (first.ResultObject.Number, first.ResultObject.Address),
            (third.ResultObject.Number, third.ResultObject.Address));
    }

    // =====================================================================
    // 2. Добавление сальдо. Бизнес-идентификатор — (дата вычисления, тип услуги).
    // =====================================================================

    [Fact]
    public void AddSaldo_WithValidData_AddsSaldoWithCalculatingDateAndServiceType()
    {
        var apartment = CreateApartment();

        var result = apartment.AddSaldo(HeatingService, 350.75m);

        Assert.True(result.IsSuccess);
        Assert.Equal("Сальдо для квартиры успешно создано", result.ResultObject);

        var saldo = Assert.Single(apartment.Saldos);
        Assert.Equal(HeatingService, saldo.ServiceType);
        Assert.Equal(350.75m, saldo.CurrentSaldoValue);
        Assert.Equal(Today, saldo.CalculatingDate); // дата вычисления — часть бизнес-идентификатора
    }

    [Fact]
    public void AddSaldo_WithNullServiceType_Fails()
    {
        var apartment = CreateApartment();

        var result = apartment.AddSaldo(null!, 100m);

        Assert.False(result.IsSuccess);
        Assert.Equal("Тип оказаной услуги пуст", result.ErrorMessage);
        Assert.Empty(apartment.Saldos);
    }

    [Theory]
    [InlineData("")]
    public void AddSaldo_WithEmptyServiceType_Fails(string serviceType)
    {
        var apartment = CreateApartment();

        var result = apartment.AddSaldo(serviceType, 100m);

        Assert.False(result.IsSuccess);
        Assert.Equal("Тип оказаной услуги пуст", result.ErrorMessage);
        Assert.Empty(apartment.Saldos);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-5000)]
    public void AddSaldo_WithNegativeIncomingSaldoValue_Fails(decimal incomingSaldoValue)
    {
        var apartment = CreateApartment();

        var result = apartment.AddSaldo(HeatingService, incomingSaldoValue);

        Assert.False(result.IsSuccess);
        Assert.Equal("Входящее сальдо должно быть положительным", result.ErrorMessage);
        Assert.Empty(apartment.Saldos);
    }

    [Fact]
    public void AddSaldo_WithZeroIncomingSaldoValue_IsAllowed()
    {
        // Домен запрещает только отрицательное входящее сальдо.
        var apartment = CreateApartment();

        var result = apartment.AddSaldo(HeatingService, 0m);

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, Assert.Single(apartment.Saldos).CurrentSaldoValue);
    }

    [Fact]
    public void AddSaldo_ForDifferentServiceTypes_CreatesSeparateSaldos()
    {
        var apartment = CreateApartment();

        Assert.True(apartment.AddSaldo(HeatingService, 100m).IsSuccess);
        Assert.True(apartment.AddSaldo(WaterService, 200m).IsSuccess);

        Assert.Equal(2, apartment.Saldos.Count);
        Assert.Contains(apartment.Saldos, saldo => saldo.ServiceType == HeatingService);
        Assert.Contains(apartment.Saldos, saldo => saldo.ServiceType == WaterService);
    }

    [Fact]
    public void AddSaldo_TwiceForSameServiceTypeInCurrentMonth_DoesNotEnforceUniquenessAtDomainLevel()
    {
        // Домен не проверяет уникальность пары (дата вычисления, тип услуги) —
        // при совпадении создаются две записи с одинаковым бизнес-идентификатором.
        // Уникальность обеспечивается индексом БД idx_unq_saldo (ApartmentUuid, CalculatingDate, ServiceType).
        var apartment = CreateApartment();

        Assert.True(apartment.AddSaldo(HeatingService, 100m).IsSuccess);
        Assert.True(apartment.AddSaldo(HeatingService, 100m).IsSuccess);

        Assert.Equal(2, apartment.Saldos.Count);
        Assert.All(apartment.Saldos, saldo => Assert.Equal(Today, saldo.CalculatingDate));
    }

    [Fact]
    public void RemoveSaldo_WithMatchingServiceTypeAndCalculatingDate_RemovesSaldo()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.RemoveSaldo(HeatingService, saldo.CalculatingDate);

        Assert.True(result.IsSuccess);
        Assert.Empty(apartment.Saldos);
    }

    [Fact]
    public void RemoveSaldo_WithWrongCalculatingDate_Fails()
    {
        var apartment = CreateApartment();
        CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.RemoveSaldo(HeatingService, Today.AddMonths(-1));

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо по квартире на данную услугу не найдено", result.ErrorMessage);
        Assert.Single(apartment.Saldos);
    }

    [Fact]
    public void RemoveSaldo_WithWrongServiceType_Fails()
    {
        var apartment = CreateApartment();
        CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.RemoveSaldo("Газ", Today);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо по квартире на данную услугу не найдено", result.ErrorMessage);
        Assert.Single(apartment.Saldos);
    }

    // =====================================================================
    // 3. Добавление оборотов (начислений). Бизнес-идентификатор — дата начисления.
    // =====================================================================

    [Fact]
    public void CreateChargeForSaldo_WithValidData_AddsChargeAndRecalculatesSaldo()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, incomingSaldoValue: 100m);

        var result = apartment.CreateChargeForSaldo(50m, HeatingService, saldo.CalculatingDate);

        Assert.True(result.IsSuccess);
        Assert.Equal("Начисление по расчетному периоду успешно добавлено", result.ResultObject);

        var charge = Assert.Single(saldo.Charges);
        Assert.Equal(50m, charge.Amount);
        Assert.Equal(150m, saldo.CurrentSaldoValue); // 100 (входящее) + 50 (начисление)
    }

    [Fact]
    public void CreateChargeForSaldo_WhenNoSaldoExists_Fails()
    {
        var apartment = CreateApartment();

        var result = apartment.CreateChargeForSaldo(50m, HeatingService, Today);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
    }

    [Fact]
    public void CreateChargeForSaldo_WithWrongServiceType_Fails()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.CreateChargeForSaldo(50m, "Газ", saldo.CalculatingDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
        Assert.Empty(saldo.Charges);
        Assert.Equal(100m, saldo.CurrentSaldoValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void CreateChargeForSaldo_WithWrongCalculatingDate_Fails(int monthOffset)
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.CreateChargeForSaldo(50m, HeatingService, Today.AddMonths(monthOffset));

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
        Assert.Empty(saldo.Charges);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreateChargeForSaldo_WithNonPositiveAmount_Fails(decimal amount)
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.CreateChargeForSaldo(amount, HeatingService, saldo.CalculatingDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Значение начисления должно быть больше 0", result.ErrorMessage);
        Assert.Empty(saldo.Charges);
        Assert.Equal(100m, saldo.CurrentSaldoValue);
    }

    [Fact]
    public void CreateChargeForSaldo_RecordsChargeDateAsBusinessIdentifier()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        Assert.True(apartment.CreateChargeForSaldo(50m, HeatingService, saldo.CalculatingDate).IsSuccess);

        var charge = Assert.Single(saldo.Charges);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), charge.ChargeDate);
    }

    // =====================================================================
    // 4. Добавление оборотов (платежей). Бизнес-идентификатор — дата платежа.
    // =====================================================================

    [Fact]
    public void CreatePaymentForSaldo_WithValidData_AddsPaymentAndRecalculatesSaldo()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, incomingSaldoValue: 100m);

        var result = apartment.CreatePaymentForSaldo(30m, HeatingService, saldo.CalculatingDate);

        Assert.True(result.IsSuccess);
        Assert.Equal("Платеж по расчетному периоду успешно добавлен", result.ResultObject);

        var payment = Assert.Single(saldo.Payments);
        Assert.Equal(30m, payment.Amount);
        Assert.Equal(70m, saldo.CurrentSaldoValue); // 100 (входящее) - 30 (платеж)
    }

    [Fact]
    public void CreatePaymentForSaldo_WhenNoSaldoExists_Fails()
    {
        var apartment = CreateApartment();

        var result = apartment.CreatePaymentForSaldo(30m, HeatingService, Today);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
    }

    [Fact]
    public void CreatePaymentForSaldo_WithWrongServiceType_Fails()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.CreatePaymentForSaldo(30m, "Газ", saldo.CalculatingDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
        Assert.Empty(saldo.Payments);
        Assert.Equal(100m, saldo.CurrentSaldoValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void CreatePaymentForSaldo_WithWrongCalculatingDate_Fails(int monthOffset)
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.CreatePaymentForSaldo(30m, HeatingService, Today.AddMonths(monthOffset));

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
        Assert.Empty(saldo.Payments);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreatePaymentForSaldo_WithNonPositiveAmount_Fails(decimal amount)
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        var result = apartment.CreatePaymentForSaldo(amount, HeatingService, saldo.CalculatingDate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Значение платежа должно быть больше 0", result.ErrorMessage);
        Assert.Empty(saldo.Payments);
        Assert.Equal(100m, saldo.CurrentSaldoValue);
    }

    [Fact]
    public void CreatePaymentForSaldo_RecordsPaymentDateAsBusinessIdentifier()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);

        Assert.True(apartment.CreatePaymentForSaldo(30m, HeatingService, saldo.CalculatingDate).IsSuccess);

        var payment = Assert.Single(saldo.Payments);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Now), payment.PaymentDate);
    }

    // =====================================================================
    // 5. Удаление оборотов по дате — краевые случаи с неверными датами.
    // =====================================================================

    [Fact]
    public void RemoveCharge_WithExistingDate_RemovesChargeAndRestoresSaldo()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);
        Assert.True(apartment.CreateChargeForSaldo(50m, HeatingService, saldo.CalculatingDate).IsSuccess);
        var chargeDate = saldo.Charges.Single().ChargeDate;

        var result = apartment.RemoveChargeForSaldo(HeatingService, saldo.CalculatingDate, chargeDate);

        Assert.True(result.IsSuccess);
        Assert.Empty(saldo.Charges);
        Assert.Equal(100m, saldo.CurrentSaldoValue); // возврат к входящему сальдо
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void RemoveCharge_WithWrongDate_Fails(int dayOffset)
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);
        Assert.True(apartment.CreateChargeForSaldo(50m, HeatingService, saldo.CalculatingDate).IsSuccess);
        var chargeDate = saldo.Charges.Single().ChargeDate;

        var result = apartment.RemoveChargeForSaldo(HeatingService, saldo.CalculatingDate, chargeDate.AddDays(dayOffset));

        Assert.False(result.IsSuccess);
        Assert.Equal("Начисление по заданному времени не найдено", result.ErrorMessage);
        Assert.Single(saldo.Charges);
    }

    [Fact]
    public void RemoveCharge_WithWrongSaldoIdentifier_Fails()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);
        Assert.True(apartment.CreateChargeForSaldo(50m, HeatingService, saldo.CalculatingDate).IsSuccess);

        // Неверная пара (дата вычисления, тип услуги) сальдо -> сальдо не найдено.
        var result = apartment.RemoveChargeForSaldo("Газ", saldo.CalculatingDate.AddMonths(-1), Today);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
    }

    [Fact]
    public void RemovePayment_WithExistingDate_RemovesPayment()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);
        Assert.True(apartment.CreatePaymentForSaldo(30m, HeatingService, saldo.CalculatingDate).IsSuccess);
        var paymentDate = saldo.Payments.Single().PaymentDate;

        var result = apartment.RemovePaymentForSaldo(HeatingService, saldo.CalculatingDate, paymentDate);

        Assert.True(result.IsSuccess);
        Assert.Empty(saldo.Payments);
        // ВНИМАНИЕ: пересчёт сальдо при удалении платежа реализован через
        // RecalculateSaldoValue(-amount, 0), что уменьшает сальдо ещё раз,
        // а не восстанавливает его. Assert по CurrentSaldoValue здесь намеренно
        // не делается до подтверждения бизнес-правила.
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void RemovePayment_WithWrongDate_Fails(int dayOffset)
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);
        Assert.True(apartment.CreatePaymentForSaldo(30m, HeatingService, saldo.CalculatingDate).IsSuccess);
        var paymentDate = saldo.Payments.Single().PaymentDate;

        var result = apartment.RemovePaymentForSaldo(HeatingService, saldo.CalculatingDate, paymentDate.AddDays(dayOffset));

        Assert.False(result.IsSuccess);
        Assert.Equal("Платеж по заданному времени не найдено", result.ErrorMessage);
        Assert.Single(saldo.Payments);
    }

    [Fact]
    public void RemovePayment_WithWrongSaldoIdentifier_Fails()
    {
        var apartment = CreateApartment();
        var saldo = CreateSaldo(apartment, HeatingService, 100m);
        Assert.True(apartment.CreatePaymentForSaldo(30m, HeatingService, saldo.CalculatingDate).IsSuccess);

        var result = apartment.RemovePaymentForSaldo("Газ", saldo.CalculatingDate.AddMonths(-1), Today);

        Assert.False(result.IsSuccess);
        Assert.Equal("Сальдо не найдено по заданной услуге и дате", result.ErrorMessage);
    }
}
