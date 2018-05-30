using System;

namespace PayerAccount.Models.Remote
{
    public class PayerState
    {
        public decimal Balance { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public string CounterName { get; private set; }
        public DateTime CounterCheckDate { get; private set; }
        public DateTime CounterMountDate { get; private set; }

        // Customer name (4, CUSTOMER_NAME)
        public string Name { get; private set; }

        // Cutomer address (5, CUSTOMER_ADDRESS)
        public string Address { get; private set; }

        // Общая площадь помещения (8, TOTAL_AREA)
        public decimal TotalFloorSpace { get; private set; }

        // количество проживающих (9, REGISTRATED_COUNT)
        public int RegistratedCount { get; private set; }

        // Количество комнат (10, ROOM_COUNT)
        public int RoomCount { get; private set; }

        // Zip code (11, ZIP_CODE)
        public int ZipCode { get; private set; }

        // Норматив 104 кВт * ч на 1 чел (12, RATE_VOLUME)
        public int RateVolume { get; private set; }

        // Долг / переплата (13, BEGIN_BALANCE)
        public decimal BeginBalance { get; private set; }

        // К оплате (32, END_BALANCE)
        public decimal EndBalance { get; private set; }

        // Объем по умолчанию, инд.потребитель (14, DEFAULT_DELTA_KWH)
        public int DefaultDeltaVolume { get; private set; }

        // Дневной объем, инд.потребитель (16, DAY_DELTA_KWH)
        public int DayDeltaVolume { get; private set; }

        // Ночной объем, инд.потребитель (18, NIGHT_DELTA_KWH)
        public int NightDeltaVolume { get; private set; }

        // Тариф по умолчанию, инд.потребитель (15, DEFAULT_TARIFF_VALUE)
        public decimal DefaultTariff { get; private set; }

        // Дневной тариф, инд.потребитель (17, DAY_TARIFF_VALUE)
        public decimal DayTariff { get; private set; }

        // Ночной тариф, инд.потребитель (19, NIGHT_TARIFF_VALUE)
        public decimal NightTariff { get; private set; }

        // Объем по умолчанию, ЭСОИ (20, PUBLICSPACE_DEFAULT_KWH)
        public int DefaultPublicspaceVolume { get; private set; }

        // Дневной объем, ЭСОИ (22, PUBLICSPACE_DAY_KWH)
        public int DayPublicspaceVolume { get; private set; }

        // Ночной объем, ЭСОИ (24, PUBLICSPACE_NIGHT_KWH)
        public int NightPublicspaceVolume { get; private set; }

        // Тариф по умолчанию, ЭСОИ (21, PUBLICSPACE_DEFAULT_TARIFF)
        public decimal DefaultPublicspaceTariff { get; private set; }

        // Дневной тариф, ЭСОИ (23, PUBLICSPACE_DAY_TARIFF)
        public decimal DayPublicspaceTariff { get; private set; }

        // Ночной тариф, ЭСОИ (25, PUBLICSPACE_NIGHT_TARIFF)
        public decimal NightPublicspaceTariff { get; private set; }

        // Инд. потребитель, размер платы (27, ESTIMATE_VALUE)
        public decimal EstimateTotal { get; private set; }

        // ЭСОИ, размер платы (28, PUBLICSPACE_ESTIMATE)
        public decimal EstimatePublicspaceTotal { get; private set; }

        // Переерасчет (30, ADJUSTMENT_VALUE)
        public decimal AdjustmentTotal { get; private set; }

        // Оплачено (31, PAYMENT_VALUE)
        public decimal PaymentTotal { get; private set; }

        // Суммарная площадь (46, GROUP_TOTAL_AREA)
        public decimal GroupTotalFloorSpace { get; private set; }

        // (37, DEFAULT_ENERGY_TARIFF)
        public decimal DefaultEnergyTariff { get; private set; }

        // (40, DAY_ENERGY_TARIFF)
        public decimal DayEnergyTariff { get; private set; }

        // (43, NIGHT_ENERGY_TARIFF)
        public decimal NightEnergyTariff { get; private set; }

        // (38, DEFAULT_TRANSFER_TARIFF)
        public decimal DefaultTransferTariff { get; private set; }

        // (41, DAY_TRANSFER_TARIFF)
        public decimal DayTransferTariff { get; private set; }

        // (44, NIGHT_TRANSFER_TARIFF)
        public decimal NightTransferTariff { get; private set; }

        public PayerState(
            decimal balance,
            int dayValue,
            int nightValue,
            string counterName, 
            DateTime counterCheckDate,
            DateTime counterMountDate,
            string name, 
            string address,
            decimal totalFloorSpace, 
            int registratedCount, 
            int roomCount,
            int zipCode,
            int rateVolume,
            decimal beginBalance,
            decimal endBalance,
            int defaultDeltaVolume,
            int dayDeltaVolume,
            int nightDeltaVolume, 
            decimal defaultTariff,
            decimal dayTariff, 
            decimal nightTariff, 
            int defaultPublicspaceVolume,
            int dayPublicspaceVolume, 
            int nightPublicspaceVolume, 
            decimal defaultPublicspaceTariff, 
            decimal dayPublicspaceTariff, 
            decimal nightPublicspaceTariff,
            decimal estimateTotal, 
            decimal estimatePublicspaceTotal,
            decimal adjustmentTotal,
            decimal paymentTotal, 
            decimal groupTotalFloorSpace,
            decimal defaultEnergyTariff,
            decimal dayEnergyTariff, 
            decimal nightEnergyTariff, 
            decimal defaultTransferTariff, 
            decimal dayTransferTariff,
            decimal nightTransferTariff)
        {
            Balance = balance;
            DayValue = dayValue;
            NightValue = nightValue;
            CounterName = counterName;
            CounterCheckDate = counterCheckDate;
            CounterMountDate = counterMountDate;
            Name = name;
            Address = address;
            TotalFloorSpace = totalFloorSpace;
            RegistratedCount = registratedCount;
            RoomCount = roomCount;
            ZipCode = zipCode;
            RateVolume = rateVolume;
            BeginBalance = beginBalance;
            EndBalance = endBalance;
            DefaultDeltaVolume = defaultDeltaVolume;
            DayDeltaVolume = dayDeltaVolume;
            NightDeltaVolume = nightDeltaVolume;
            DefaultTariff = defaultTariff;
            DayTariff = dayTariff;
            NightTariff = nightTariff;
            DefaultPublicspaceVolume = defaultPublicspaceVolume;
            DayPublicspaceVolume = dayPublicspaceVolume;
            NightPublicspaceVolume = nightPublicspaceVolume;
            DefaultPublicspaceTariff = defaultPublicspaceTariff;
            DayPublicspaceTariff = dayPublicspaceTariff;
            NightPublicspaceTariff = nightPublicspaceTariff;
            EstimateTotal = estimateTotal;
            EstimatePublicspaceTotal = estimatePublicspaceTotal;
            AdjustmentTotal = adjustmentTotal;
            PaymentTotal = paymentTotal;
            GroupTotalFloorSpace = groupTotalFloorSpace;
            DefaultEnergyTariff = defaultEnergyTariff;
            DayEnergyTariff = dayEnergyTariff;
            NightEnergyTariff = nightEnergyTariff;
            DefaultTransferTariff = defaultTransferTariff;
            DayTransferTariff = dayTransferTariff;
            NightTransferTariff = nightTransferTariff;
        }
    }
}
