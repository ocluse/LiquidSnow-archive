using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thismaker.Core;
using Thismaker.Goro.Commands;
using Thismaker.Thoth;

namespace Test.Thoth.Wpf
{
    public class DataContext : BindableBase
    {
        private string headerText;
        private string nameText;
        private string buttonText;
        public string HeaderText
        {
            get => headerText;
            set => SetProperty(ref headerText, value);
        }
        public string NameText
        {
            get => nameText;
            set => SetProperty(ref nameText, value);
        }
        public string ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value);
        }
        public DataContext()
        {
            Start();
            ChangeLocaleCommand = new RelayCommand(OnChangeLocale);
        }
        private async void Start()
        {
            try 
            {
                await LocalizationManager.LoadData("localdata");
            }
            catch(Exception ex)
            {
                var x = 0;
            }
            
            LocalizationManager.BindProperty(this, nameof(HeaderText), "login_scene", "headerInfoSI")
                .BindObservableTarget(this, nameof(NameText));
        }
        private void OnChangeLocale()
        {
           var index = LocalizationManager.Locales.IndexOf(LocalizationManager.CurrentLocale);
            if (index == -1) index = 0;
            index++;
            if (index >= LocalizationManager.Locales.Count) index -= LocalizationManager.Locales.Count;
            LocalizationManager.CurrentLocale = LocalizationManager.Locales[index];
            ButtonText = LocalizationManager.CurrentLocale.Name;
        }

        public RelayCommand ChangeLocaleCommand { get; private set; }
    }
}