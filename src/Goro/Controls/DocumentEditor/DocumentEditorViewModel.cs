using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using Thismaker.Core;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using Thismaker.Goro.Commands;

namespace Thismaker.Goro.Controls
{
    internal class DocumentEditorViewModel:BindableBase
    {
        private bool? _isBold, _isItalic, _isUnderline;

        private double? _fontSize;
        private FontFamily _font;
        private RichTextBox _document;
        private ObservableCollection<FontFamily> _fonts;
        private ObservableCollection<double> _fontSizes;

        #region Lists
        public ObservableCollection<FontFamily> Fonts
        {
            get { return _fonts; }
            set { SetProperty(ref _fonts, value); }
        }

        public ObservableCollection<double> FontSizes
        {
            get { return _fontSizes; }
            set { SetProperty(ref _fontSizes, value); }
        }
        #endregion

        public FontFamily Font
        {
            get { return _font; }
            set {  SetProperty(ref _font, value); SetFont(_font); }
        }

        public double? FontSize
        {
            get { return _fontSize; }
            set { SetProperty(ref _fontSize, value); SetFontSize(_fontSize); }
        }

        public bool? IsBold
        {
            get { return _isBold; }
            set { SetProperty(ref _isBold, value); if (value != null) SetBold(_isBold); }
        }

        public bool? IsItalic
        {
            get { return _isItalic; }
            set { SetProperty(ref _isItalic, value); if (value != null) SetItalic(_isItalic); }
        }

        public bool? IsUnderline
        {
            get { return _isUnderline; }
            set { SetProperty(ref _isUnderline, value); SetUnderline(_isUnderline); }
        }

        public DocumentEditorViewModel()
        {
            Fonts = new ObservableCollection<FontFamily>(System.Windows.Media.Fonts.SystemFontFamilies);

            FontSizes = new ObservableCollection<double>()
            {
                8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72
            };
        }

        public void Attach(RichTextBox document)
        {
            _document = document;
            _document.SelectionChanged += Document_SelectionChanged;
        }

        private void Document_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Font = CheckFont();
            FontSize = CheckFontSize();
            IsBold = CheckBold();
            IsItalic = CheckItalic();
            IsUnderline = CheckUnderline();

        }

        private FontFamily CheckFont()
        {
            if (_document == null) return null;
            var font = _document.Selection.GetPropertyValue(Control.FontFamilyProperty);
            if (font != DependencyProperty.UnsetValue)
            {
                return (FontFamily)font;
            }
            return null;
        }

        private void SetFont(FontFamily font)
        {
            if (font == null) return;
            _document.Selection.ApplyPropertyValue(Control.FontFamilyProperty, font);
        }

        private double? CheckFontSize()
        {
            if (_document == null) return 8;
            var fontSize = _document.Selection.GetPropertyValue(Control.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue)
            {
                return (double)fontSize;
            }
            return null;
        }

        private void SetFontSize(double? value)
        {
            if (value == null) return;
            _document.Selection.ApplyPropertyValue(Control.FontSizeProperty, value);
        }

        private bool? CheckBold()
        {
            if (_document == null) return null;
            var bold = _document.Selection.GetPropertyValue(Inline.FontWeightProperty);
            if (bold != DependencyProperty.UnsetValue)
            {
                return (FontWeight)bold == FontWeights.Bold;
            }
            return null;
        }
        
        private void SetBold(bool? value)
        {
            _document.Selection.ApplyPropertyValue(Inline.FontWeightProperty,
                    value.HasValue ? (IsBold.Value ? FontWeights.Bold : FontWeights.Normal) : FontWeights.Normal);
        }

        private bool? CheckItalic()
        {
            if (_document == null) return null;
            var italics = _document.Selection.GetPropertyValue(Control.FontStyleProperty);
            if (italics != DependencyProperty.UnsetValue)
            {
                return (FontStyle)italics == FontStyles.Italic;
            }
            return null;
        }

        private void SetItalic(bool? value)
        {
            _document.Selection.ApplyPropertyValue(Control.FontStyleProperty,
                   value.HasValue ? (IsItalic.Value ? FontStyles.Italic : FontStyles.Normal) : FontStyles.Normal);
        }

        private bool? CheckUnderline()
        {
            if (_document == null) return null;
            return CheckDeco(TextDecorationLocation.Underline);
        }

        private void SetUnderline(bool? value)
        {
            SetDeco(TextDecorations.Underline, value);
        }

        private bool? CheckDeco(TextDecorationLocation loc)
        {
            if (_document == null) return null;
            var decos = _document.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (decos != DependencyProperty.UnsetValue)
            {
                return ((TextDecorationCollection)decos)
                    .Any(x => x.Location == loc);
            }
            return null;
        }

        private void SetDeco(TextDecorationCollection deco, bool? value)
        {
            if (value.HasValue && value.Value)
            {
                if (!(_document.Selection.GetPropertyValue(Inline.TextDecorationsProperty) is TextDecorationCollection decos)) decos = new TextDecorationCollection();
                if (!decos.IsFrozen) decos.Add(deco);

                _document.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, decos);
            }
            else
            {
                if (!(_document.Selection.GetPropertyValue(Inline.TextDecorationsProperty) is TextDecorationCollection decoCollection)) return;
                decoCollection.TryRemove(deco, out TextDecorationCollection decos);
                _document.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, decos);
            }
        }
    }
}
