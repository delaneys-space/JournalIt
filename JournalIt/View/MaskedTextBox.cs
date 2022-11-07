using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace JournalIt.View
{
    public class MaskedTextBox : TextBox
    {

        public static readonly DependencyProperty InputMaskProperty;

        private List<InputMaskChar> _maskChars;
        private int _caretIndex;

        static MaskedTextBox()
        {
            TextProperty.OverrideMetadata(typeof(MaskedTextBox),
                new FrameworkPropertyMetadata(null, new CoerceValueCallback(Text_CoerceValue)));
            InputMaskProperty = DependencyProperty.Register("InputMask", typeof(string), typeof(MaskedTextBox),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(InputMask_Changed)));
        }

        public MaskedTextBox()
        {
            _maskChars = new List<InputMaskChar>();
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(MaskedTextBox_Paste));
        }

        /// <summary>
        /// Get or Set the input mask.
        /// </summary>
        public string InputMask
        {
            get => GetValue(InputMaskProperty) as string;
            set { SetValue(InputMaskProperty, value); }
        }

        [Flags]
        protected enum InputMaskValidationFlags
        {
            None = 0,
            AllowInteger = 1,
            AllowDecimal = 2,
            AllowAlphabet = 4,
            AllowAlphanumeric = 8
        }

        /// <summary>
        /// Returns a value indicating if the current text value is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsTextValid()
        {
            return ValidateTextInternal(Text, out var value);
        }

        private class InputMaskChar
        {

            private InputMaskValidationFlags _validationFlags;
            private char _literal;

            public InputMaskChar(InputMaskValidationFlags validationFlags)
            {
                _validationFlags = validationFlags;
                _literal = (char)0;
            }

            public InputMaskChar(char literal)
            {
                _literal = literal;
            }

            public InputMaskValidationFlags ValidationFlags => _validationFlags;

            public char Literal => _literal;

            public bool IsLiteral()
            {
                return (_literal != (char)0);
            }

            public char GetDefaultChar()
            {
                return (IsLiteral()) ? Literal : '_';
            }

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            //DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(TextBox));
            //if (dpd != null)
            //{
            //    dpd.AddValueChanged(this, delegate
            //    {
            //        UpdateInputMask();
            //    });
            //}

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            _caretIndex = CaretIndex;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            //no mask specified, just function as a normal textbox
            if (_maskChars.Count == 0)
                return;

            switch (e.Key)
            {
                case Key.Delete:
                    //delete key pressed: delete all text
                    Text = GetDefaultText();
                    _caretIndex = CaretIndex = 0;
                    e.Handled = true;
                    break;
                //backspace key pressed
                case Key.Back:
                {
                    if (_caretIndex > 0 || SelectionLength > 0)
                    {
                        if (SelectionLength > 0)
                        {
                            //if one or more characters selected, delete them
                            DeleteSelectedText();
                        }
                        else
                        {
                            //if no characters selected, shift the caret back to the previous non-literal char and delete it
                            MoveBack();

                            var characters = Text.ToCharArray();
                            characters[_caretIndex] = _maskChars[_caretIndex].GetDefaultChar();
                            Text = new string(characters);
                        }

                        //update the base class caret index, and swallow the event
                        CaretIndex = _caretIndex;
                        e.Handled = true;
                    }

                    break;
                }
                case Key.Left:
                    //move back to the previous non-literal character
                    MoveBack();
                    e.Handled = true;
                    break;
                case Key.Right:
                case Key.Space:
                    //move forwards to the next non-literal character
                    MoveForward();
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {

            base.OnPreviewTextInput(e);

            //no mask specified, just function as a normal textbox
            if (_maskChars.Count == 0)
                return;

            _caretIndex = CaretIndex = SelectionStart;

            if (_caretIndex == _maskChars.Count)
            {
                //at the end of the character count defined by the input mask- no more characters allowed
                e.Handled = true;
            }
            else
            {
                //validate the character against its validation scheme
                bool isValid = ValidateInputChar(char.Parse(e.Text),
                    _maskChars[_caretIndex].ValidationFlags);

                if (isValid)
                {
                    //delete any selected text
                    if (SelectionLength > 0)
                    {
                        DeleteSelectedText();
                    }

                    //insert the new character
                    var characters = Text.ToCharArray();
                    characters[_caretIndex] = char.Parse(e.Text);
                    Text = new string(characters);

                    //move the caret on 
                    MoveForward();
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Validates the specified character against all selected validation schemes.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationFlags"></param>
        /// <returns></returns>
        protected virtual bool ValidateInputChar(char input, InputMaskValidationFlags validationFlags)
        {
            var valid = (validationFlags == InputMaskValidationFlags.None);

            if (valid) 
                return true;

            var values = Enum.GetValues(typeof(InputMaskValidationFlags));

            //iterate through the validation schemes
            foreach (var o in values)
            {
                var instance = (InputMaskValidationFlags)(int)o;
                if ((instance & validationFlags) == 0)
                    continue;


                if (!ValidateCharInternal(input, instance))
                    continue;

                valid = true;
                break;
            }

            return valid;
        }

        /// <summary>
        /// Returns a value indicating if the current text value is valid.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateTextInternal(string text, out string displayText)
        {
            if (_maskChars.Count == 0)
            {
                displayText = text;
                return true;
            }

            var displayTextBuilder = new StringBuilder(GetDefaultText());

            var valid = (!string.IsNullOrEmpty(text) &&
                         text.Length <= _maskChars.Count);

            if (valid)
            {
                for (var i = 0; i < text.Length; i++)
                {
                    if (_maskChars[i].IsLiteral())
                        continue;

                    if (ValidateInputChar(text[i], _maskChars[i].ValidationFlags))
                        displayTextBuilder[i] = text[i];
                    else
                        valid = false;
                }
            }

            displayText = displayTextBuilder.ToString();

            return valid;
        }

        /// <summary>
        /// Deletes the currently selected text.
        /// </summary>
        protected virtual void DeleteSelectedText()
        {
            var text = new StringBuilder(Text);
            var defaultText = GetDefaultText();
            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;

            text.Remove(selectionStart, selectionLength);
            text.Insert(selectionStart, defaultText.Substring(selectionStart, selectionLength));
            Text = text.ToString();

            //reset the caret position
            CaretIndex = _caretIndex = selectionStart;
        }

        /// <summary>
        /// Returns a value indicating if the specified input mask character is a placeholder.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="validationFlags">If the character is a placeholder, returns the relevant validation scheme.</param>
        /// <returns></returns>
        protected virtual bool IsPlaceholderChar(char character, out InputMaskValidationFlags validationFlags)
        {
            validationFlags = InputMaskValidationFlags.None;

            switch (character.ToString().ToUpper())
            {
                case "I":
                    validationFlags = InputMaskValidationFlags.AllowInteger;
                    break;
                case "D":
                    validationFlags = InputMaskValidationFlags.AllowDecimal;
                    break;
                case "A":
                    validationFlags = InputMaskValidationFlags.AllowAlphabet;
                    break;
                case "W":
                    validationFlags = (InputMaskValidationFlags.AllowAlphanumeric);
                    break;
            }

            return (validationFlags != InputMaskValidationFlags.None);
        }

        /// <summary>
        /// Invoked when the coerce value callback is invoked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static object Text_CoerceValue(DependencyObject obj, object value)
        {
            var mtb = (MaskedTextBox)obj;

            if (value == null || value.Equals(string.Empty))
                value = mtb.GetDefaultText();
            else if (value.ToString().Length > 0)
            {
                mtb.ValidateTextInternal(value.ToString(), out var displayText);
                value = displayText;
            }

            return value;
        }

        /// <summary>
        /// Invoked when the InputMask dependency property reports a change.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void InputMask_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as MaskedTextBox).UpdateInputMask();
        }

        /// <summary>
        /// Invokes when a paste event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskedTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            //TODO: play nicely here?

            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var value = e.DataObject.GetData(typeof(string)).ToString();

                if (ValidateTextInternal(value, out var displayText))
                    Text = displayText;
            }

            e.CancelCommand();
        }

        /// <summary>
        /// Rebuilds the InputMaskChars collection when the input mask property is updated.
        /// </summary>
        private void UpdateInputMask()
        {

            var text = Text;
            _maskChars.Clear();

            Text = string.Empty;

            var mask = InputMask;

            if (string.IsNullOrEmpty(mask))
                return;

            for (var i = 0; i < mask.Length; i++)
            {
                var isPlaceholder = IsPlaceholderChar(mask[i], out var validationFlags);

                _maskChars.Add(isPlaceholder ? 
                    new InputMaskChar(validationFlags) : 
                    new InputMaskChar(mask[i]));
            }

            if (text.Length > 0 && ValidateTextInternal(text, out var displayText))
                Text = displayText;
            else
                Text = GetDefaultText();
        }

        /// <summary>
        /// Validates the specified character against its input mask validation scheme.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationType"></param>
        /// <returns></returns>
        private bool ValidateCharInternal(char input, InputMaskValidationFlags validationType)
        {
            var valid = false;

            switch (validationType)
            {
                case InputMaskValidationFlags.AllowInteger:
                case InputMaskValidationFlags.AllowDecimal:
                    int i;
                    if (validationType == InputMaskValidationFlags.AllowDecimal &&
                        input == '.' && !Text.Contains('.'))
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = int.TryParse(input.ToString(), out i);
                    }
                    break;
                case InputMaskValidationFlags.AllowAlphabet:
                    valid = char.IsLetter(input);
                    break;
                case InputMaskValidationFlags.AllowAlphanumeric:
                    valid = (char.IsLetter(input) || char.IsNumber(input));
                    break;
            }

            return valid;
        }

        /// <summary>
        /// Builds the default display text for the control.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultText()
        {
            var text = new StringBuilder();
            foreach (var maskChar in _maskChars)
                text.Append(maskChar.GetDefaultChar());

            return text.ToString();
        }

        /// <summary>
        /// Moves the caret forward to the next non-literal position.
        /// </summary>
        private void MoveForward()
        {
            var pos = _caretIndex;
            while (pos < _maskChars.Count)
            {
                if (++pos != _maskChars.Count && _maskChars[pos].IsLiteral())
                    continue;

                _caretIndex = CaretIndex = pos;
                break;
            }
        }

        /// <summary>
        /// Moves the caret backward to the previous non-literal position.
        /// </summary>
        private void MoveBack()
        {
            var pos = _caretIndex;
            while (pos > 0)
            {
                if (--pos != 0 && _maskChars[pos].IsLiteral())
                    continue;

                _caretIndex = CaretIndex = pos;
                break;
            }
        }
    }
}
