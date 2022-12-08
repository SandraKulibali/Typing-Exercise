using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
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
using static System.Net.WebRequestMethods;
using Color = System.Windows.Media.Color;
using File = System.IO.File;
using Path = System.IO.Path;

namespace Typing_Exercise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        #region global variables
        SolidColorBrush aqua = new SolidColorBrush(Colors.MediumAquamarine);
        SolidColorBrush paleRed = new SolidColorBrush(Colors.PaleVioletRed);
        SolidColorBrush goldenRod = new SolidColorBrush(Colors.LightGoldenrodYellow);
        SolidColorBrush blueViolet = new SolidColorBrush(Colors.BlueViolet);
        SolidColorBrush gray = new SolidColorBrush(Colors.LightGray);
        SolidColorBrush flowerBlue = new SolidColorBrush(Colors.CornflowerBlue);
        SolidColorBrush buttonColorWhenClicked = new SolidColorBrush(Colors.Thistle);
        SolidColorBrush correctColor = new SolidColorBrush(Colors.ForestGreen);
        SolidColorBrush missedColor = new SolidColorBrush(Colors.Tomato);
        SolidColorBrush textColor = new SolidColorBrush(Colors.WhiteSmoke);
        SolidColorBrush textBlockColor = new SolidColorBrush(Colors.AliceBlue);
        Stopwatch timer = new Stopwatch();

        bool isShiftPressed = false;
        bool isCapitalPressed = false;
        int index = 0;
        int buttonsClickedCounter = 0;
        int failsCounter = 0;
        string temp = string.Empty;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            StartButtonClicked();
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            StopButtonClicked();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            buttonsClickedCounter++;
            // counter for calculating speed of typing
            temp = generatedTextField.Text; // variable to pass to the Method to check the correctness of typed key

            #region checking if Shift or Capital is pressed

            //If Lshift is pressed and startButton is still enabled generate text
            if (e.Key == Key.LeftShift && startButton.IsEnabled)
            {
                StartButtonClicked();
                return;
            }

            //Remembering state of Lshiftbutton
            if (e.Key == Key.LeftShift)
                isShiftPressed = true;

            //Remembering state of CapsLock
            else if (e.Key == Key.Capital)
                isCapitalPressed = true;

            //Changing Keyboard depending on the pressed key
            ChangeKeyboardLayout();
            #endregion

            if (!startButton.IsEnabled) // if start button was clicked
            {
                foreach (Button button in FindVisualChilds<Button>(this)) // getting all Buttons in current Window, iterating through each
                {
                    if (IsButtonMatchesKey(button, e)) // checking if pressed key in both states (upper,lower) equals to specific button in layout
                    {
                        
                        button.Background = buttonColorWhenClicked; //changing color of the button

                        FillTypingTextFiled(temp, button, e); // filling textblock

                        if (index >= temp.Length) //If exercise is done show results and reset
                        {
                            MessageBox.Show("Done!");
                            StopButtonClicked();
                            return;
                        }


                        if (failsCount.Text != "")
                            failsCount.Text = "Fails: " + failsCounter.ToString(); // refreshing mistakes count text
                    }

                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            #region checking if Shift or Capital is pressed

            if (e.Key == Key.LeftShift)
            {
                isShiftPressed = false;
            }
            else if (e.Key == Key.Capital)
            {
                isCapitalPressed = false;
            }
            #endregion

            ChangeKeyboardLayout();

            if (!startButton.IsEnabled)
            {
                // getting all Buttons in current Window  and changing button color depending on key
                foreach (Button button in FindVisualChilds<Button>(this))                                                          
                {
                    bool buttonMatchesKey = IsButtonMatchesKey(button, e);
                    
                    if (buttonMatchesKey && button.Tag.ToString() == "aquamarine")
                        button.Background = aqua;

                    else if (buttonMatchesKey && button.Tag.ToString() == "paleRed")
                        button.Background = paleRed;

                    else if (buttonMatchesKey && button.Tag.ToString() == "goldenRod")
                        button.Background = goldenRod;

                    else if (buttonMatchesKey && button.Tag.ToString() == "blueViolet")
                        button.Background = blueViolet;

                    else if (buttonMatchesKey && button.Tag.ToString() == "gray" || button.Name =="Return")
                        button.Background = gray;

                    else if (buttonMatchesKey && button.Tag.ToString() == "flowerBlue")
                        button.Background = flowerBlue;

                }

            }

        }

        //Method of generating text from file to textblock field
        public static List<Run> GenerateRandomText(double difficultyLevel)
        {
            Random random = new();
            List<Run> charSigns = new List<Run>();
            var fileRandome = random.Next(1, 5);
            string pathEasy = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @$"Files\easy.txt");
            string pathMedium = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Files\medium.txt");
            string pathHard = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @$"Files\hard{fileRandome}.txt");
            string generatedText = string.Empty;

            switch (difficultyLevel)
            {
                case 1:
                    string temp = File.ReadAllText(pathEasy);
                    generatedText = new string(pathEasy.Select(c => temp[random.Next(temp.Length)]).Take(130).ToArray()).ToLower().TrimStart();
                    for (int i = 0; i < generatedText.Length; i++)
                    {
                        charSigns.Add(new Run() { Text = generatedText[i].ToString() });
                    }
                    break;

                case 2:
                    temp = File.ReadAllText(pathEasy);
                    generatedText = new string(temp.Select(c => temp[random.Next(temp.Length)]).Take(130).ToArray()).TrimStart();
                    for (int i = 0; i < generatedText.Length; i++)
                    {
                        charSigns.Add(new Run() { Text = generatedText[i].ToString() });
                    }
                    break;

                case 3:
                    temp = File.ReadAllText(pathMedium);
                    generatedText = new string(temp.Select(c => temp[random.Next(temp.Length)]).Take(130).ToArray()).ToLower().TrimStart();
                    for (int i = 0; i < generatedText.Length; i++)
                    {
                        charSigns.Add(new Run() { Text = generatedText[i].ToString() });
                    }
                    break;

                case 4:
                    temp = File.ReadAllText(pathMedium);
                    generatedText = new string(temp.Select(c => temp[random.Next(temp.Length)]).Take(130).ToArray()).TrimStart();
                    for (int i = 0; i < generatedText.Length; i++)
                    {
                        charSigns.Add(new Run() { Text = generatedText[i].ToString() });
                    }
                    break;

                case 5:
                    temp = File.ReadAllText(pathHard);
                    generatedText = temp;
                    for (int i = 0; i < generatedText.Length; i++)
                    {
                        charSigns.Add(new Run() { Text = generatedText[i].ToString() });
                    }
                    break;

            }
            return charSigns;
        }

        //Method to set difficulty level
        private void difficultySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double.TryParse(difficultySlider.Value.ToString(), out double lvl);
            lvl = Math.Floor(lvl);

            if (difficultyLvl.Text != "")
                difficultyLvl.Text = "Difficulty: " + lvl.ToString();
        }

        //Method to fill textField and check for failed entry
        private void FillTypingTextFiled(string text, Button b, KeyEventArgs e)
        {
            
            while (index < text.Length)
            {
                //If Case sensitive box checkbox was not checked
                if (caseSensativeBox.IsChecked.Value == false)
                {
                    if (e.Key == Key.LeftShift || e.Key == Key.Capital) // Do nothing
                        return;

                    else if (e.Key == Key.Enter || e.Key == Key.Return) // Stop typing, refresh data
                    {
                        timer.Stop();
                        speed.Text = $"Speed: {CalculateSpeed()} WPM";
                        typingTextField.Text = " ";
                        caseSensativeBox.IsChecked = false;
                        stopButton.IsEnabled = false;
                        startButton.IsEnabled = true;
                        ChangeStartAndStopButtonsColor();
                        
                        return;
                    }

                    else if (e.Key == Key.Tab || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.RightShift) // Wrong key pressed
                    {
                        typingTextField.Inlines.Add(new Run(" ") { Foreground = missedColor });
                        index++;
                        failsCounter++;
                        return;
                    }

                    else if (e.Key == Key.Back) //Removing character, reducing index of the string
                    {
                        typingTextField.Inlines.Remove(typingTextField.Inlines.LastInline);
                       
                        if (index > 0)
                            index--;

                        generatedTextField.Inlines.ElementAt(index).Background = textBlockColor;
                        return;
                    }

                    else if ((b.Content.ToString().ToLower() == text[index].ToString().ToLower())) //correct key entry
                    {
                        typingTextField.Inlines.Add(new Run(b.Content.ToString()) { Foreground = correctColor });
                        generatedTextField.Inlines.ElementAt(index).Background = flowerBlue;
                        index++;
                        return;
                    }

                    else if ((e.Key == Key.Space)) // If space button was pressed
                    {
                        if (text[index].ToString() == " ") //if correct space key entry
                        {
                            typingTextField.Inlines.Add(new Run(" ") { Foreground = correctColor });
                            generatedTextField.Inlines.ElementAt(index).Background = flowerBlue;
                            index++;
                            return;
                        }

                        else // incorrect space key entry
                        {
                            typingTextField.Inlines.Add(new Run(" ") { Foreground = missedColor });
                            index++;
                            failsCounter++;
                            return;
                        }
                    }


                    else // any other cases
                    {
                        typingTextField.Inlines.Add(new Run(b.Content.ToString()) { Foreground = missedColor });
                        index++;
                        failsCounter++;
                        return;
                    }
                }

                // if case sensitive box was checked (should do separate method for all of this cause it's grotesque)
                else
                {
                    if (e.Key == Key.LeftShift || e.Key == Key.Capital)
                    {
                        return;
                    }

                    else if (e.Key == Key.Enter)
                    {
                        timer.Stop();
                        speed.Text = $"Speed: {CalculateSpeed()} WPM";
                        typingTextField.Text = " ";
                        stopButton.IsEnabled = false;
                        startButton.IsEnabled = true;
                        ChangeStartAndStopButtonsColor();
                        return;
                    }

                    else if (e.Key == Key.Back)
                    {
                        typingTextField.Inlines.Remove(typingTextField.Inlines.LastInline);
                        if (index > 0)
                            index--;

                        generatedTextField.Inlines.ElementAt(index).Background = textBlockColor;
                        return;
                    }

                    else if ((b.Content.ToString() == text[index].ToString()))
                    {
                        typingTextField.Inlines.Add(new Run(b.Content.ToString()) { Foreground = correctColor });
                        generatedTextField.Inlines.ElementAt(index).Background = flowerBlue;
                        index++;
                        return;
                    }

                    else if ((e.Key == Key.Space))
                    {
                        if (text[index].ToString() == " ")
                        {
                            typingTextField.Inlines.Add(new Run(" ") { Foreground = correctColor });
                            generatedTextField.Inlines.ElementAt(index).Background = flowerBlue;
                            index++;
                            return;
                        }

                        else
                        {
                            typingTextField.Inlines.Add(new Run(" ") { Foreground = missedColor });
                            index++;
                            failsCounter++;
                            return;
                        }
                    }


                    else
                    {
                        typingTextField.Inlines.Add(new Run(b.Content.ToString()) { Foreground = missedColor });
                        index++;
                        failsCounter++;
                        return;

                    }
                }

            }

        }

        //Method to calculate speed of typing
        private string CalculateSpeed()
        {
            double time = timer.Elapsed.TotalMinutes;
            double speed = Math.Round((buttonsClickedCounter / 5) / time); //formula found in the net prob wrong ;)
            return speed.ToString();
        }

        //Method of checking if pressed key matches button content
        private bool IsButtonMatchesKey(Button b, KeyEventArgs e)
        {
            return b.Name.ToString() == e.Key.ToString();
        }

        //Method to get difficulty value
        private double GetDifficultyValue()
        {
            double.TryParse(difficultySlider.Value.ToString(), out double lvl);
            lvl = Math.Floor(lvl);
            return lvl;
        }

        //Method to toggle start and stop buttons style
        private void ChangeStartAndStopButtonsColor()
        {
            if (startButton.IsEnabled)
            {
                startButton.Background = correctColor;
                startButton.Foreground = textColor;
                stopButton.Background = textColor;
                stopButton.Foreground = missedColor;
            }
            else if (stopButton.IsEnabled)
            {
                stopButton.Background = missedColor;
                stopButton.Foreground = textColor;
                startButton.Background = textColor;
                startButton.Foreground = correctColor;
            }
        }

        //Generic method to find and return children of an object
        private static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }

        //Method to change settings when start button/shift was clicked or pressed
        private void StartButtonClicked()
        {
            index = 0;
            failsCounter = 0;
            buttonsClickedCounter = 0;
            timer.Start();
            typingTextField.IsEnabled = true;
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            ChangeStartAndStopButtonsColor();
            double lvl = GetDifficultyValue();
            generatedTextField.Text = string.Empty;
            failsCount.Text = "Fails: ";
            speed.Text = "Speed: ";
            generatedTextField.Inlines.AddRange(GenerateRandomText(lvl));
        }

        //Method to change settings when start button/shift was clicked or pressed
        private void StopButtonClicked()
        {
            
            typingTextField.IsEnabled = false;
            timer.Stop();
            stopButton.IsEnabled = false;
            startButton.IsEnabled = true;
            caseSensativeBox.IsChecked = false;
            ChangeStartAndStopButtonsColor();
            speed.Text = $"Speed: {CalculateSpeed()} WPM";
            typingTextField.Text = string.Empty;
            generatedTextField.Text = string.Empty;
           
        }

        //Awful method to change keyboard layout cause i don't know better
        private void ChangeKeyboardLayout()
        {
            #region a lot of variables
            if (isCapitalPressed)
            {
                Oem3.Content = "~";
                D1.Content = "!";
                D2.Content = "@";
                D3.Content = "#";
                D4.Content = "$";
                D5.Content = "%";
                D6.Content = "^";
                D7.Content = "&";
                D8.Content = "*";
                D9.Content = "(";
                D0.Content = ")";
                OemMinus.Content = "_";
                OemPlus.Content = "+";

                Q.Content = "Q";
                W.Content = "W";
                E.Content = "E";
                R.Content = "R";
                T.Content = "T";
                Y.Content = "Y";
                U.Content = "U";
                I.Content = "I";
                O.Content = "O";
                P.Content = "P";
                OemOpenBrackets.Content = "{";
                Oem6.Content = "}";
                Oem5.Content = "|";

                A.Content = "A";
                S.Content = "S";
                D.Content = "D";
                F.Content = "F";
                G.Content = "G";
                H.Content = "H";
                J.Content = "J";
                K.Content = "K";
                L.Content = "L";
                Oem1.Content = ":";
                OemQuotes.Content = "\"";

                Z.Content = "Z";
                X.Content = "X";
                C.Content = "C";
                V.Content = "V";
                B.Content = "B";
                N.Content = "N";
                M.Content = "M";
                OemComma.Content = "<";
                OemPeriod.Content = ">";
                OemQuestion.Content = "?";

            }
            else if (isShiftPressed)
            {
                Oem3.Content = "`";
                D1.Content = "1";
                D2.Content = "2";
                D3.Content = "3";
                D4.Content = "4";
                D5.Content = "5";
                D6.Content = "6";
                D7.Content = "7";
                D8.Content = "8";
                D9.Content = "9";
                D0.Content = "0";
                OemMinus.Content = "-";
                OemPlus.Content = "=";

                Q.Content = "q";
                W.Content = "w";
                E.Content = "e";
                R.Content = "r";
                T.Content = "t";
                Y.Content = "y";
                U.Content = "u";
                I.Content = "i";
                O.Content = "o";
                P.Content = "p";
                OemOpenBrackets.Content = "[";
                Oem6.Content = "]";
                Oem5.Content = @"\";

                A.Content = "a";
                S.Content = "s";
                D.Content = "d";
                F.Content = "f";
                G.Content = "g";
                H.Content = "h";
                J.Content = "j";
                K.Content = "k";
                L.Content = "l";
                Oem1.Content = ";";
                OemQuotes.Content = "'";

                Z.Content = "z";
                X.Content = "x";
                C.Content = "c";
                V.Content = "v";
                B.Content = "b";
                N.Content = "n";
                M.Content = "m";
                OemComma.Content = ",";
                OemPeriod.Content = ".";
                OemQuestion.Content = "/";
            }
            #endregion
        }
    }
}

