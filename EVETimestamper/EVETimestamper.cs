namespace EVETimestamper
{
    public partial class EVETimestamper : Form
    {
        private string ShortTime = "h:mm tt";
        private string LongTime = "h:mm:ss tt";
        private string ShortDate = "M/d/yy";
        private string LongDate = "MMMM d, yyyy";
        private string LongDateShortTime = "MMMM d, yyyy 'at' h:mm tt";
        private string LongDateWithDay = "dddd, MMMM d, yyyy 'at' h:mm tt";
        enum DiscordFormat
        {
            RELATIVE,
            SHORT_TIME,
            LONG_TIME,
            SHORT_DATE,
            LONG_DATE,
            LONG_DATE_SHORT_TIME,
            LONG_DATE_WITH_DAY
        }

        public EVETimestamper()
        {
            InitializeComponent();
            InitalizeForm();
        }

        private void InitalizeForm()
        {
            dateTimePicker.ShowCheckBox = false;
            dateTimePicker.ShowUpDown = false;
            UpdateDiscordButtons();
        }

        private void SetEVETime(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int hour = Convert.ToInt32(button.Tag);
            DateTime currentDateTime = dateTimePicker.Value.ToUniversalTime();
            DateTime modifiedDateTime = new(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, hour, 0, 0);
            dateTimePicker.Value = modifiedDateTime.ToLocalTime();
        }

        private void AddTime(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int minutes = Convert.ToInt32(button.Tag);
            dateTimePicker.Value = dateTimePicker.Value.AddMinutes(minutes);
        }

        private void RoundTime(object sender, EventArgs e)
        {
            Button button = sender as Button;
            decimal minutes = Convert.ToInt32(button.Tag);
            DateTime currentDateTime = dateTimePicker.Value;
            decimal currentMinutes = (currentDateTime.Hour * 60) + currentDateTime.Minute;
            decimal newMinutes = Math.Round(currentMinutes / minutes, MidpointRounding.ToPositiveInfinity) * minutes;
            if (newMinutes == currentMinutes)
                newMinutes = Math.Round((currentMinutes + 1) / minutes, MidpointRounding.ToPositiveInfinity) * minutes;
            DateTime modifiedDateTime = new(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
            modifiedDateTime = modifiedDateTime.AddMinutes(Convert.ToInt32(newMinutes));
            dateTimePicker.Value = modifiedDateTime;
        }

        private void btnCurrent_Click(object sender, EventArgs e)
        {
            dateTimePicker.Value = DateTime.Now;
        }

        private void btnRelative_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.RELATIVE);
        }

        private void SetClipboard(DiscordFormat format)
        {
            TimeSpan t = dateTimePicker.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            string clipboard = "";
            switch (format)
            {
                case DiscordFormat.RELATIVE:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":R>";
                    break;
                case DiscordFormat.SHORT_TIME:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":t>";
                    break;
                case DiscordFormat.LONG_TIME:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":T>";
                    break;
                case DiscordFormat.SHORT_DATE:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":d>";
                    break;
                case DiscordFormat.LONG_DATE:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":D>";
                    break;
                case DiscordFormat.LONG_DATE_SHORT_TIME:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":f>";
                    break;
                case DiscordFormat.LONG_DATE_WITH_DAY:
                    clipboard = "<t:" + secondsSinceEpoch.ToString() + ":F>";
                    break;
                default:
                    break;
            }
            Clipboard.SetText(clipboard);
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateDiscordButtons();
        }

        private string RelativeFormatter(DateTime dateTime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(dateTime.Ticks - DateTime.Now.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);
            if (ts >= TimeSpan.Zero)
            {
                if (delta < 1 * MINUTE)
                    return ts.Seconds == 1 ? "in 1 second" : "in " + ts.Seconds + " seconds";

                if (delta < 2 * MINUTE)
                    return "in 1 minute";

                if (delta < 45 * MINUTE)
                    return "in " + ts.Minutes + " minutes";

                if (delta < 90 * MINUTE)
                    return "in 1 hour";

                if (delta < 24 * HOUR)
                    return "in " + ts.Hours + " hours";

                if (delta < 48 * HOUR)
                    return "tomorrow";

                if (delta < 30 * DAY)
                    return "in " + ts.Days + " days";

                if (delta < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return months <= 1 ? "in 1 month" : "in " + months + " months";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return years <= 1 ? "in 1 year" : "in " + years + " years";
                }
            }
            else
            {
                ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
                if (delta < 1 * MINUTE)
                    return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

                if (delta < 2 * MINUTE)
                    return "a minute ago";

                if (delta < 45 * MINUTE)
                    return ts.Minutes + " minutes ago";

                if (delta < 90 * MINUTE)
                    return "an hour ago";

                if (delta < 24 * HOUR)
                    return ts.Hours + " hours ago";

                if (delta < 48 * HOUR)
                    return "yesterday";

                if (delta < 30 * DAY)
                    return ts.Days + " days ago";

                if (delta < 12 * MONTH)
                {
                    int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return months <= 1 ? "one month ago" : months + " months ago";
                }
                else
                {
                    int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return years <= 1 ? "one year ago" : years + " years ago";
                }
            }
        }

        private void AddDays(object sender, EventArgs e)
        {
            Button button = sender as Button;
            int days = Convert.ToInt32(button.Tag);
            dateTimePicker.Value = dateTimePicker.Value.AddDays(days);
        }

        private void UpdateDiscordButtons()
        {
            //string relativeTime = 
            //string shortTime = dateTimePicker.Value.ToString(ShortTime);
            //string longTime = dateTimePicker.Value.ToString(LongTime);
            //string shortDate = dateTimePicker.Value.ToString(ShortDate);
            //string longDate = dateTimePicker.Value.ToString(LongDate);
            //string longDateShortTime = dateTimePicker.Value.ToString(LongDateShortTime);
            //string longDateWithDay = dateTimePicker.Value.ToString(LongDateWithDay);
            btnRelative.Text = RelativeFormatter(dateTimePicker.Value);
            btnShortTime.Text = dateTimePicker.Value.ToString(ShortTime);
            btnLongTime.Text = dateTimePicker.Value.ToString(LongTime);
            btnShortDate.Text = dateTimePicker.Value.ToString(ShortDate);
            btnLongDate.Text = dateTimePicker.Value.ToString(LongDate);
            btnLongDateShortTime.Text = dateTimePicker.Value.ToString(LongDateShortTime);
            btnLongDateWeekDay.Text = dateTimePicker.Value.ToString(LongDateWithDay);
        }

        private void AddMonth(object sender, EventArgs e)
        {
            Button button = sender as Button;
            //int days = Convert.ToInt32(button.Tag);
            dateTimePicker.Value = dateTimePicker.Value.AddMonths(1);
        }

        private void AddYear(object sender, EventArgs e)
        {
            Button button = sender as Button;
            //int days = Convert.ToInt32(button.Tag);
            dateTimePicker.Value = dateTimePicker.Value.AddYears(1);
        }

        private void btnShortTime_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.SHORT_TIME);
        }

        private void btnLongTime_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.LONG_TIME);
        }

        private void btnShortDate_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.SHORT_DATE);
        }

        private void btnLongDate_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.LONG_DATE);
        }

        private void btnLongDateShortTime_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.LONG_DATE_SHORT_TIME);
        }

        private void btnLongDateWeekDay_Click(object sender, EventArgs e)
        {
            SetClipboard(DiscordFormat.LONG_DATE_WITH_DAY);
        }
    }
}
