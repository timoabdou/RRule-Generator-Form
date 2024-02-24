using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RRule_Generator_Form
{
    public class RRuleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        /*public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        protected static void OnStaticPropertyChanged([CallerMemberName] string propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }*/

        protected virtual void OnPropertyChanged(bool needsRefresh = true, [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (needsRefresh)
            {
                if (DataLoaded) RRule = GenerateRRule();
            }
        }

        bool DataLoaded = false;
        public RRuleViewModel()
        {
            StartDate = new DateTime(2019, 02, 01);

            SelectedFrequency = "Yearly";

            IsYearlySpecifiedOption = true;
            //IsYearlyDescribedOption = false; // No need for this line
            YearlySpecifiedOptionMonth = "Jan";
            YearlySpecifiedOptionDay = 1;
            YearlyDescribedOptionPosition = "First";
            YearlyDescribedOptionDayOfWeek = "Monday";
            YearlyDescribedOptionMonth = "Jan";

            IsMonthlySpecifiedOption = true;
            //IsMonthlyDescribedOption = false; // No need for this line
            MonthlyInterval = 1;
            MonthlySpecifiedOptionDay = 1;
            MonthlyDescribedOptionPosition = "First";
            MonthlyDescribedOptionDayOfWeek = "Monday";

            WeeklyInterval = 1;

            DailyInterval = 1;

            HourlyInterval = 1;

            SelectedDaysOfWeek = new ObservableCollection<DayOfWeekViewModel>
            {
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[0] },
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[1] },
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[2] },
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[3] },
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[4] },
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[5] },
                new DayOfWeekViewModel { IsDaySelected = false, Value = WeeklyDays[6] }
            };
            foreach (var dayViewModel in SelectedDaysOfWeek)
            {
                dayViewModel.ItemModified += DaysOfWeekChanged;
            }

            EndOption = "Never";
            ExecutionTimes = 1;
            EndDate = DateTime.Now;
            DataLoaded = true;

            RRule = GenerateRRule();
        }

        // ***********************************************************************
        // **************************** Start Section ****************************
        // ***********************************************************************

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value; OnPropertyChanged();
                if (YearlySpecifiedOptionMonth != null) YearlySpecifiedOptionMonth = YearlySpecifiedOptionMonth;
                PushDaysToMonthlyDays();
            }
        }

        // *********************************************************************
        // ************************* Frequency Section *************************
        // *********************************************************************

        private bool _spYearly = false;
        public bool SPYearly { get { return _spYearly; } set { _spYearly = value; OnPropertyChanged(); } }

        private bool _spMonthly = false;
        public bool SPMonthly { get { return _spMonthly; } set { _spMonthly = value; OnPropertyChanged(); } }

        private bool _spWeekly = false;
        public bool SPWeekly { get { return _spWeekly; } set { _spWeekly = value; OnPropertyChanged(); } }

        private bool _spDaily = false;
        public bool SPDaily { get { return _spDaily; } set { _spDaily = value; OnPropertyChanged(); } }

        private bool _spHourly = false;
        public bool SPHourly { get { return _spHourly; } set { _spHourly = value; OnPropertyChanged(); } }

        private string _selectedFrequency;
        public string SelectedFrequency
        {
            get { return _selectedFrequency; }
            set
            {
                _selectedFrequency = value;
                OnPropertyChanged();

                // Show/Hide corresponding options based on frequency
                SPYearly = false; SPMonthly = false; SPWeekly = false; SPDaily = false; SPHourly = false;
                switch (SelectedFrequency)
                {
                    case "Yearly": SPYearly = true; break;
                    case "Monthly": SPMonthly = true; break;
                    case "Weekly": SPWeekly = true; break;
                    case "Daily": SPDaily = true; break;
                    case "Hourly": SPHourly = true; break;
                    default: break;
                }
            }
        }

        // **********************************************************************
        // *************************** Yearly Section ***************************
        // **********************************************************************

        private bool _isYearlySpecifiedOption;
        public bool IsYearlySpecifiedOption
        {
            get { return _isYearlySpecifiedOption; }
            set
            {
                _isYearlySpecifiedOption = value;
                OnPropertyChanged();
            }
        }

        private bool _isYearlyDescribedOption;
        public bool IsYearlyDescribedOption
        {
            get { return _isYearlyDescribedOption; }
            set
            {
                _isYearlyDescribedOption = value;
                OnPropertyChanged();
            }
        }

        private string _yearlySpecifiedOptionMonth;
        public string YearlySpecifiedOptionMonth
        {
            get { return _yearlySpecifiedOptionMonth; }
            set
            {
                _yearlySpecifiedOptionMonth = value;
                OnPropertyChanged();
                PushDaysToYearlyDay();
            }
        }

        private int _yearlySpecifiedOptionDay;
        public int YearlySpecifiedOptionDay
        {
            get { return _yearlySpecifiedOptionDay; }
            set
            {
                _yearlySpecifiedOptionDay = value;
                OnPropertyChanged();
            }
        }

        private string _yearlyDescribedOptionPosition;
        public string YearlyDescribedOptionPosition
        {
            get { return _yearlyDescribedOptionPosition; }
            set
            {
                _yearlyDescribedOptionPosition = value;
                OnPropertyChanged();
            }
        }

        private string _yearlyDescribedOptionDayOfWeek;
        public string YearlyDescribedOptionDayOfWeek
        {
            get { return _yearlyDescribedOptionDayOfWeek; }
            set
            {
                _yearlyDescribedOptionDayOfWeek = value;
                OnPropertyChanged();
            }
        }

        private string _yearlyDescribedOptionMonth;
        public string YearlyDescribedOptionMonth
        {
            get { return _yearlyDescribedOptionMonth; }
            set
            {
                _yearlyDescribedOptionMonth = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<int> _yearlyDaysOfMonth = new ObservableCollection<int>();
        public ObservableCollection<int> YearlyDaysOfMonth
        {
            get { return _yearlyDaysOfMonth; }
            set
            {
                _yearlyDaysOfMonth = value;
                OnPropertyChanged();
            }
        }
        private void PushDaysToYearlyDay()
        {
            YearlyDaysOfMonth.Clear();
            for (int i = 1; i <= DateTime.DaysInMonth(StartDate.Year, DateTime.ParseExact(YearlySpecifiedOptionMonth, "MMM", CultureInfo.InvariantCulture).Month); i++) YearlyDaysOfMonth.Add(i);
            YearlySpecifiedOptionDay = Math.Min(YearlySpecifiedOptionDay, YearlyDaysOfMonth.Last());
        }

        // ***********************************************************************
        // *************************** Monthly Section ***************************
        // ***********************************************************************

        private bool _isMonthlySpecifiedOption;
        public bool IsMonthlySpecifiedOption
        {
            get { return _isMonthlySpecifiedOption; }
            set
            {
                _isMonthlySpecifiedOption = value;
                OnPropertyChanged();
            }
        }

        private bool _isMonthlyDescribedOption;
        public bool IsMonthlyDescribedOption
        {
            get { return _isMonthlyDescribedOption; }
            set
            {
                _isMonthlyDescribedOption = value;
                OnPropertyChanged();
            }
        }

        private int _monthlyInterval;
        public int MonthlyInterval
        {
            get { return _monthlyInterval; }
            set
            {
                _monthlyInterval = value;
                OnPropertyChanged();
            }
        }

        private int _monthlySpecifiedOptionDay;
        public int MonthlySpecifiedOptionDay
        {
            get { return _monthlySpecifiedOptionDay; }
            set
            {
                _monthlySpecifiedOptionDay = value;
                OnPropertyChanged();
            }
        }

        private string _monthlyDescribedOptionPosition;
        public string MonthlyDescribedOptionPosition
        {
            get { return _monthlyDescribedOptionPosition; }
            set
            {
                _monthlyDescribedOptionPosition = value;
                OnPropertyChanged();
            }
        }

        private string _monthlyDescribedOptionDayOfWeek;
        public string MonthlyDescribedOptionDayOfWeek
        {
            get { return _monthlyDescribedOptionDayOfWeek; }
            set
            {
                _monthlyDescribedOptionDayOfWeek = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<int> _monthlyDaysOfMonth = new ObservableCollection<int>();
        public ObservableCollection<int> MonthlyDaysOfMonth
        {
            get { return _monthlyDaysOfMonth; }
            set
            {
                _monthlyDaysOfMonth = value;
                OnPropertyChanged();
            }
        }
        private void PushDaysToMonthlyDays()
        {
            MonthlyDaysOfMonth.Clear();
            for (int i = 1; i <= DateTime.DaysInMonth(StartDate.Year, StartDate.Month); i++) MonthlyDaysOfMonth.Add(i);

            MonthlySpecifiedOptionDay = Math.Min(MonthlySpecifiedOptionDay, MonthlyDaysOfMonth.Last());
        }

        // **********************************************************************
        // *************************** Weekly Section ***************************
        // **********************************************************************

        private int _weeklyInterval;
        public int WeeklyInterval
        {
            get { return _weeklyInterval; }
            set
            {
                _weeklyInterval = value;
                OnPropertyChanged();
            }
        }
        
        private List<string> _weeklyDays = new List<string>() { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };
        public List<string> WeeklyDays
        {
            get { return _weeklyDays; }
            set
            {
                _weeklyDays = value;
                OnPropertyChanged();
            }
        }
        
        public class DayOfWeekViewModel : INotifyPropertyChanged
        {
            private event EventHandler _itemModified;
            public event EventHandler ItemModified
            {
                add { _itemModified += value; }
                remove { _itemModified -= value; }
            }

            private string _value;

            public string Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnPropertyChanged();
                    }
                }
            }
            private bool _isDaySelected;

            public bool IsDaySelected
            {
                get { return _isDaySelected; }
                set
                {
                    if (_isDaySelected != value)
                    {
                        _isDaySelected = value;
                        OnPropertyChanged();
                        OnItemModified();
                    }
                }
            }

            private void OnItemModified()
            {
                _itemModified?.Invoke(this, EventArgs.Empty);
            }


            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<DayOfWeekViewModel> _selectedDaysOfWeek;
        public ObservableCollection<DayOfWeekViewModel> SelectedDaysOfWeek
        {
            get { return _selectedDaysOfWeek; }
            set
            {
                _selectedDaysOfWeek = value;
                OnPropertyChanged();
            }
        }

        private void DaysOfWeekChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < SelectedDaysOfWeek.Count; i++)
            {
                WeeklyDays[i] = SelectedDaysOfWeek[i].IsDaySelected ? SelectedDaysOfWeek[i].Value : "";
            }
            OnPropertyChanged();
        }

        // *********************************************************************
        // *************************** Daily Section ***************************
        // *********************************************************************

        private int _dailyInterval = 1;
        public int DailyInterval
        {
            get { return _dailyInterval; }
            set
            {
                _dailyInterval = value;
                OnPropertyChanged();
            }
        }
        // ********************************************************************
        // ************************** Hourly Section **************************
        // ********************************************************************

        private int _hourlyInterval = 1;
        public int HourlyInterval
        {
            get { return _hourlyInterval; }
            set
            {
                _hourlyInterval = value;
                OnPropertyChanged();
            }
        }

        // ***********************************************************************
        // ***************************** End Section *****************************
        // ***********************************************************************

        private bool _after = false;
        public bool After { get { return _after; } set { _after = value; OnPropertyChanged(); } }

        private bool _onDate = false;
        public bool OnDate { get { return _onDate; } set { _onDate = value; OnPropertyChanged(); } }

        private int _executionTimes;
        public int ExecutionTimes
        {
            get { return _executionTimes; }
            set
            {
                if (value > 0) _executionTimes = value;
                else _executionTimes = 1;

                OnPropertyChanged();
            }
        }

        private string _endOption;
        public string EndOption
        {
            get { return _endOption; }
            set
            {
                _endOption = value;
                OnPropertyChanged();

                // Show/Hide corresponding options based on endOption
                switch (EndOption.ToLower())
                {
                    case "never": After = false; OnDate = false; break;
                    case "after": After = true; OnDate = false; break;
                    case "on date": After = false; OnDate = true; break;
                    default: break;
                }

                OnPropertyChanged();
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value; OnPropertyChanged();
            }
        }

        // ************************************************************************
        // *********************** RRule Generation Section ***********************
        // ************************************************************************

        private string _rrule;
        public string RRule
        {
            get { return _rrule; }
            set
            {
                _rrule = value;
                OnPropertyChanged(false);
            }
        }

        private string GenerateRRule()
        {
            StringBuilder rrule = new StringBuilder("DTSTART:");
            rrule.Append(StartDate.AddHours(-1).ToString("yyyyMMddTHHmmssZ"));

            switch (SelectedFrequency)
            {
                case "Yearly":
                    if (IsYearlySpecifiedOption)
                    {
                        rrule.Append($"\nRRULE:FREQ=YEARLY;BYMONTH={DateTime.ParseExact(YearlySpecifiedOptionMonth, "MMM", CultureInfo.InvariantCulture).Month};BYMONTHDAY={YearlySpecifiedOptionDay}");
                    }
                    else
                    {
                        string position = MonthPositionToRRuleFormat(YearlyDescribedOptionPosition);
                        string dayOfWeek = DayOfWeekToRRuleFormat(YearlyDescribedOptionDayOfWeek);
                        rrule.Append($"\nRRULE:FREQ=YEARLY;BYSETPOS={position};BYDAY={dayOfWeek};BYMONTH={DateTime.ParseExact(YearlyDescribedOptionMonth, "MMM", CultureInfo.InvariantCulture).Month}");
                    }
                    break;

                case "Monthly":
                    if (IsMonthlySpecifiedOption)
                    {
                        rrule.Append($"\nRRULE:FREQ=MONTHLY;INTERVAL={MonthlyInterval};BYMONTHDAY={MonthlySpecifiedOptionDay}");
                    }
                    else
                    {
                        string position = MonthPositionToRRuleFormat(MonthlyDescribedOptionPosition);
                        string dayOfWeek = DayOfWeekToRRuleFormat(MonthlyDescribedOptionDayOfWeek);
                        rrule.Append($"\nRRULE:FREQ=MONTHLY;INTERVAL={MonthlyInterval};BYSETPOS={position};BYDAY={dayOfWeek}");
                    }
                    break;

                case "Weekly":
                    rrule.Append($"\nRRULE:FREQ=WEEKLY;INTERVAL={WeeklyInterval}");
                    string days = GetSelectedDaysOfWeekRRuleFormat();
                    if (days != "") rrule.Append($";BYDAY={days}");
                    break;

                case "Daily":
                    rrule.Append($"\nRRULE:FREQ=DAILY;INTERVAL={DailyInterval}");
                    break;

                case "Hourly":
                    rrule.Append($"\nRRULE:FREQ=HOURLY;INTERVAL={HourlyInterval}");
                    break;

                default:
                    break;
            }

            if (EndOption.ToLower() == "after")
            {
                rrule.Append($";COUNT={ExecutionTimes}");
            }
            else if (EndOption.ToLower() == "on date")
            {
                DateTime endDT = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day).AddHours(-1);
                rrule.Append($";UNTIL={endDT.ToString("yyyyMMddTHHmmssZ")}");
            }

            return rrule.ToString();
        }

        private string MonthPositionToRRuleFormat(string position)
        {
            switch (position)
            {
                case "First":
                    return "1";
                case "Second":
                    return "2";
                case "Third":
                    return "3";
                case "Fourth":
                    return "4";
                case "Last":
                    return "-1";
                default:
                    return "";
            }
        }

        private string DayOfWeekToRRuleFormat(string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Monday":
                    return "MO";
                case "Tuesday":
                    return "TU";
                case "Wednesday":
                    return "WE";
                case "Thursday":
                    return "TH";
                case "Friday":
                    return "FR";
                case "Saturday":
                    return "SA";
                case "Sunday":
                    return "SU";
                case "Day":
                    return "SU,MO,TU,WE,TH,FR,SA";
                case "Weekday":
                    return "MO,TU,WE,TH,FR";
                case "Weekend day":
                    return "SA,SU";
                default:
                    return "";
            }
        }

        private string GetSelectedDaysOfWeekRRuleFormat()
        {
            List<string> selectedDays = new List<string>();

            for (int i = 0; i < SelectedDaysOfWeek.Count; i++)
            {
                if (SelectedDaysOfWeek[i].IsDaySelected)
                {
                    selectedDays.Add(SelectedDaysOfWeek[i].Value);
                }
            }

            return string.Join(",", selectedDays);
        }

        // *************************************************************************
        // ********************* Fill Model From RRule Section *********************
        // *************************************************************************

        public void FillFromRRule(string rruleString)
        {
            Dictionary<string, string> parsedRRule = ParseRRule(rruleString);
            bool thereIsEndOption = false;

            // Fill DTSTART
            if (parsedRRule.TryGetValue("DTSTART", out string dtstart))
            {
                // Assuming dtstart is in the format: "yyyyMMddTHHmmssZ"
                if (DateTime.TryParseExact(dtstart, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
                {
                    StartDate = startDate;
                }
                else
                {
                    // Handle invalid DTSTART format
                }
            }
            else
            {
                // Handle missing DTSTART
            }

            // Fill RRULE
            if (parsedRRule.TryGetValue("RRULE", out string rrule))
            {
                var rruleComponents = SortRRuleComponents(rrule.Split(';'));
                foreach (var component in rruleComponents)
                {
                    var keyValue = component.Split('=');
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();

                        switch (key)
                        {
                            case "FREQ":
                                SelectedFrequency = Capitalize(value); // Custom extension method to capitalize the first character
                                switch (SelectedFrequency)
                                {
                                    case "Yearly": 
                                        IsYearlySpecifiedOption = true;
                                        YearlySpecifiedOptionMonth = "Jan";
                                        YearlySpecifiedOptionDay = 1;

                                        YearlyDescribedOptionPosition = "First";
                                        YearlyDescribedOptionDayOfWeek = "Monday";
                                        YearlyDescribedOptionMonth = "Jan";
                                        break;
                                    case "Monthly":
                                        MonthlyInterval = 1;
                                        IsMonthlySpecifiedOption = true;
                                        MonthlySpecifiedOptionDay = 1;

                                        MonthlyDescribedOptionPosition = "First";
                                        MonthlyDescribedOptionDayOfWeek = "Monday";
                                        break;
                                    case "Weekly":
                                        WeeklyInterval = 1;
                                        foreach (DayOfWeekViewModel d in SelectedDaysOfWeek) d.IsDaySelected = false;
                                        break;
                                    case "Daily":
                                        DailyInterval = 1;
                                        break;
                                    case "Hourly":
                                        HourlyInterval = 1;
                                        break;
                                }
                                break;

                            case "INTERVAL":
                                if (int.TryParse(value, out int interval))
                                {
                                    switch (SelectedFrequency)
                                    {
                                        case "Monthly":
                                            MonthlyInterval = interval;
                                            break;
                                        case "Weekly":
                                            WeeklyInterval = interval;
                                            break;
                                        case "Daily":
                                            DailyInterval = interval;
                                            break;
                                        case "Hourly":
                                            HourlyInterval = interval;
                                            break;
                                    }
                                }
                                else
                                {
                                    // Handle invalid interval format
                                }
                                break;

                            case "BYMONTH":
                                if (int.TryParse(value, out int byMonth))
                                {
                                    if(IsYearlySpecifiedOption)
                                    {
                                        YearlySpecifiedOptionMonth = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(byMonth).Substring(0, 3);
                                    }
                                    else
                                    {
                                        YearlyDescribedOptionMonth = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(byMonth).Substring(0, 3);
                                    }
                                }
                                else
                                {
                                    // Handle invalid byMonth format
                                }
                                break;

                            case "BYMONTHDAY":
                                if (int.TryParse(value, out int byMonthDay))
                                {
                                    switch (SelectedFrequency)
                                    {
                                        case "Yearly": YearlySpecifiedOptionDay = byMonthDay; break;
                                        case "Monthly": MonthlySpecifiedOptionDay = byMonthDay; break;
                                    }
                                }
                                else
                                {
                                    // Handle invalid byMonthDay format
                                }
                                break;

                            case "BYSETPOS":
                                switch (SelectedFrequency)
                                {
                                    case "Yearly":
                                        IsYearlyDescribedOption = true;
                                        break;
                                    case "Monthly":
                                        IsMonthlyDescribedOption = true;
                                        break;
                                }

                                if (int.TryParse(value, out int bySetPos))
                                {
                                    switch (SelectedFrequency)
                                    {
                                        case "Yearly":
                                            YearlyDescribedOptionPosition = bySetPos switch
                                            {
                                                1 => "First",
                                                2 => "Second",
                                                3 => "Third",
                                                4 => "Fourth",
                                                -1 => "Last"
                                            };
                                            break;
                                        case "Monthly":
                                            MonthlyDescribedOptionPosition = bySetPos switch
                                            {
                                                1 => "First",
                                                2 => "Second",
                                                3 => "Third",
                                                4 => "Fourth",
                                                -1 => "Last"
                                            };
                                            break;
                                    }
                                }
                                else
                                {
                                    // Handle invalid bySetPos format
                                }
                                break;

                            case "BYDAY":
                                switch (SelectedFrequency)
                                {
                                    case "Yearly":
                                        YearlyDescribedOptionDayOfWeek = value switch
                                        {
                                            "MO" => "Monday",
                                            "TU" => "Tuesday",
                                            "WE" => "Wednesday",
                                            "TH" => "Thursday",
                                            "FR" => "Friday",
                                            "SA" => "Saturday",
                                            "SU" => "Sunday",
                                            "MO,TU,WE,TH,FR,SA,SU" => "Day",
                                            "MO,TU,WE,TH,FR" => "Weekday",
                                            "SA,SU" => "Weekend day",
                                            _ => ""
                                        };
                                        break;
                                    case "Monthly":
                                        MonthlyDescribedOptionDayOfWeek = value switch
                                        {
                                            "MO" => "Monday",
                                            "TU" => "Tuesday",
                                            "WE" => "Wednesday",
                                            "TH" => "Thursday",
                                            "FR" => "Friday",
                                            "SA" => "Saturday",
                                            "SU" => "Sunday",
                                            "MO,TU,WE,TH,FR,SA,SU" => "Day",
                                            "MO,TU,WE,TH,FR" => "Weekday",
                                            "SA,SU" => "Weekend day",
                                            _ => ""
                                        };
                                        break;
                                    case "Weekly":
                                        var days = value.Split(',');
                                        foreach (string day in days)
                                        {
                                            switch (day)
                                            {
                                                case "MO": SelectedDaysOfWeek[0].IsDaySelected = true; break;
                                                case "TU": SelectedDaysOfWeek[1].IsDaySelected = true; break;
                                                case "WE": SelectedDaysOfWeek[2].IsDaySelected = true; break;
                                                case "TH": SelectedDaysOfWeek[3].IsDaySelected = true; break;
                                                case "FR": SelectedDaysOfWeek[4].IsDaySelected = true; break;
                                                case "SA": SelectedDaysOfWeek[5].IsDaySelected = true; break;
                                                case "SU": SelectedDaysOfWeek[6].IsDaySelected = true; break;
                                            }
                                        }
                                        break;
                                }
                        break;
                            case "UNTIL":
                                // Assuming UNTIL is in the format: "yyyyMMddTHHmmssZ"
                                if (DateTime.TryParseExact(value, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                                {
                                    EndOption = "On date";
                                    EndDate = endDate;
                                    thereIsEndOption = true;
                                }
                                else
                                {
                                    // Handle invalid UNTIL format
                                }
                                break;
                            case "COUNT":
                                if (int.TryParse(value, out int count))
                                {
                                    EndOption = "After";
                                    ExecutionTimes = count;
                                    thereIsEndOption = true;
                                }
                                else
                                {
                                    // Handle invalid count format
                                }
                                break;
                        }
                    }
                }
                if (!thereIsEndOption) EndOption = "Never";
            }
            else
            {
                // Handle missing RRULE
            }
        }

        private List<string> SortRRuleComponents(string[] rruleComponents)
        {
            // Initialize a list to store sorted components
            List<string> sortedComponents = new List<string>();

            // Add FREQ parameter first if it exists
            var freqComponent = rruleComponents.FirstOrDefault(c => c.StartsWith("FREQ="));
            if (!string.IsNullOrEmpty(freqComponent))
            {
                sortedComponents.Add(freqComponent);
            }
            else sortedComponents.Add("FREQ=YEARLY");

            // Add BYSETPOS parameter next if it exists
            var bySetPosComponent = rruleComponents.FirstOrDefault(c => c.StartsWith("BYSETPOS="));
            if (!string.IsNullOrEmpty(bySetPosComponent))
            {
                sortedComponents.Add(bySetPosComponent);
            }

            // Add the remaining components in their original order
            foreach (var component in rruleComponents)
            {
                if (!component.StartsWith("FREQ=") && !component.StartsWith("BYSETPOS="))
                {
                    sortedComponents.Add(component);
                }
            }

            return sortedComponents;
        }

        private Dictionary<string, string> ParseRRule(string rruleString)
        {
            var components = rruleString.Split('\n');
            var parsedComponents = new Dictionary<string, string>();

            foreach (var component in components)
            {
                var keyValue = component.Split(':');
                if (keyValue.Length == 2)
                {
                    keyValue[0] = keyValue[0].Trim().ToUpper();
                    parsedComponents[keyValue[0]] = keyValue[1].Trim().ToUpper();
                }
            }

            return parsedComponents;
        }

        private string Capitalize(string input)
        {
            string result = input.ToLower();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return char.ToUpper(result[0]) + result.Substring(1);
        }

    }
}
