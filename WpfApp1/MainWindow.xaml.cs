using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OSAGOPolicy _policy = new OSAGOPolicy();
        private readonly OSAGOCalculator _calculator = new OSAGOCalculator();

        public MainWindow()
        {
            InitializeComponent();
        }

        public class OSAGOPolicy
        {
            public int DriverBirthYear { get; set; }
            public int DriverLicenseYear { get; set; }
            public string DriverResidence { get; set; }
            public string VehicleCategory { get; set; }
            public int VehicleEnginePower { get; set; }

            private readonly OSAGOCalculator _calculator = new OSAGOCalculator();

            public double CalculateOSAGOPrice(double baseRate)
            {
                var territoryCoefficient = _calculator.CalculateTerritoryCoefficient(DriverResidence);
                var ageAndExperienceCoefficient = _calculator.CalculateAgeAndExperienceCoefficient(DriverBirthYear, DriverLicenseYear);
                var bonusCoefficient = _calculator.CalculateBonusCoefficient(DriverLicenseYear);
                var powerCoefficient = _calculator.CalculatePowerCoefficient(VehicleEnginePower);

                return baseRate * territoryCoefficient * ageAndExperienceCoefficient * bonusCoefficient * powerCoefficient;
            }
        }

        public class OSAGOCalculator
        {
            public double CalculateTerritoryCoefficient(string driverResidence)
            {
                switch (driverResidence)
                {
                    case "Пенза":
                        return 1.4;
                    case "Кузнецк":
                    case "Пензенская область":
                        return 1.0;
                    case "Сызрань":
                        return 1.1;
                    case "Самара":
                        return 1.6;
                    case "Тольятти":
                        return 1.5;
                    case "Димитровград":
                        return 1.1;
                    case "Ульяновск":
                        return 1.4;
                    default:
                        return 0.7;
                }
            }

            public double CalculateAgeAndExperienceCoefficient(int driverBirthYear, int driverLicenseYear)
            {
                int driverAge = DateTime.Now.Year - driverBirthYear;
                int driverExperience = DateTime.Now.Year - driverLicenseYear;

                if (driverAge <= 22 && driverExperience <= 3)
                    return 1.8;
                else if (driverAge <= 22 && driverExperience > 3)
                    return 1.6;
                else if (driverAge > 22 && driverExperience <= 3)
                    return 1.7;
                else
                    return 1.0;
            }

            public double CalculateBonusCoefficient(int driverLicenseYear)
            {
                int driverExperience = DateTime.Now.Year - driverLicenseYear;
                return Math.Max(0.46, 3.92 - 0.173 * driverExperience);
            }

            public double CalculatePowerCoefficient(int vehicleEnginePower)
            {
                if (vehicleEnginePower <= 50)
                    return 0.6;
                else if (vehicleEnginePower <= 70)
                    return 1.0;
                else if (vehicleEnginePower <= 100)
                    return 1.1;
                else if (vehicleEnginePower <= 120)
                    return 1.2;
                else if (vehicleEnginePower <= 150)
                    return 1.4;
                else
                    return 1.6;
            }
        }

        private void CalculateOSAGO_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные из элементов управления
            if (int.TryParse(DriverBirthYearTextBox.Text, out int driverBirthYear))
                _policy.DriverBirthYear = driverBirthYear;
            else
            {
                ShowErrorMessage("Пожалуйста, введите корректный год рождения.");
                return;
            }

            if (int.TryParse(DriverLicenseYearTextBox.Text, out int driverLicenseYear))
                _policy.DriverLicenseYear = driverLicenseYear;
            else
            {
                ShowErrorMessage("Пожалуйста, введите корректный год получения прав.");
                return;
            }

            _policy.DriverResidence = DriverResidenceComboBox.SelectedValue.ToString();
            _policy.VehicleCategory = VehicleCategoryComboBox.SelectedValue.ToString();

            if (int.TryParse(VehicleEnginePowerTextBox.Text, out int vehicleEnginePower))
                _policy.VehicleEnginePower = vehicleEnginePower;
            else
            {
                ShowErrorMessage("Пожалуйста, введите корректную мощность двигателя.");
                return;
            }

            // Получаем базовый тариф в зависимости от категории ТС
            double baseRate = GetBaseRate(_policy.VehicleCategory);

            // Рассчитываем и выводим результат
            double osagoPriceResult = _policy.CalculateOSAGOPrice(baseRate);
            TerritoryCoefficient.Text = _policy.DriverResidence + ": " + _calculator.CalculateTerritoryCoefficient(_policy.DriverResidence).ToString("F2");
            AgeAndExperienceCoefficient.Text = _calculator.CalculateAgeAndExperienceCoefficient(_policy.DriverBirthYear, _policy.DriverLicenseYear).ToString("F2");
            BonusCoefficient.Text = _calculator.CalculateBonusCoefficient(_policy.DriverLicenseYear).ToString("F2");
            PowerCoefficient.Text = _calculator.CalculatePowerCoefficient(_policy.VehicleEnginePower).ToString("F2");
            OSAGOPrice.Text = osagoPriceResult.ToString("C2");
        }

        

        private double GetBaseRate(string vehicleCategory)
        {
            switch (vehicleCategory)
            {
                case "Мотоциклы, мотороллеры (категории А)":
                    return 1215;
                case "Легковые автомобили (категории В)":
                    return 1980;
                case "Грузовые автомобили (категории С)":
                    return 2025;
                default:
                    return 0;
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}





















    