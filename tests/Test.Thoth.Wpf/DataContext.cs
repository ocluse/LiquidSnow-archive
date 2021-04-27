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
        private string buttonText, buttonText2, buttonText3;
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
        public string ButtonText2
        {
            get => buttonText2;
            set => SetProperty(ref buttonText2, value);
        }
        public string ButtonText3
        {
            get => buttonText3;
            set => SetProperty(ref buttonText3, value);
        }
        public DataContext()
        {
            ChangeLocaleCommand = new RelayCommand(OnChangeLocale);
            BindCommand = new RelayCommand(OnBindTarget);
            ChangeKeyCommand = new RelayCommand(OnChangeKey, CanChangeKey);
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
        bool state;
        string id;
        private void OnBindTarget()
        {
            if (state)
            {
                var bind=LocalizationManager.BindProperty(this, nameof(HeaderText), "login_scene", "headerInfoSI")
                .BindObservableTarget(this, nameof(NameText));
                id = bind.Id;
                ButtonText2 = "Binding On";
            }
            else
            {
                id = null;
                LocalizationManager.UnbindAllProperties();
                ButtonText2 = "Binding Off";
            }
            state = !state;
        }
        private bool CanChangeKey()
        {
            return !string.IsNullOrEmpty(id);
        }

        bool isInfo;
        private void OnChangeKey()
        {
            if (isInfo)
            {
                LocalizationManager.ChangeBindingKey(id, "headerTextSI");
                ButtonText3 = "headerTextSI";
            }
            else
            {
                LocalizationManager.ChangeBindingKey(id, "headerInfoSI")
                    .BindObservableTarget(this, nameof(NameText));
                ButtonText3 = "headerInfoSI";
            }
            isInfo = !isInfo;
        }

        public RelayCommand ChangeLocaleCommand { get; private set; }
        public RelayCommand BindCommand { get; private set; }
        public RelayCommand ChangeKeyCommand { get; private set; }
    }
}