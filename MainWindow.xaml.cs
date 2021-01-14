using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Calculator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        List<string> infixEquation = new List<string>();
        List<string> postfixEquation = new List<string>();

        public MainWindow() {
            InitializeComponent();
        }

        private void BtnAC_Click(object sender, RoutedEventArgs e) {
            tbResult.Text = "";
            infixEquation.Clear();
        }

        private void BtnParenth_Click(object sender, RoutedEventArgs e) {
            string parenth = (sender as Button).Content.ToString();
            //Note: no parenthese balance validation
            tbResult.Text += parenth;
            infixEquation.Add(parenth);
        }

        private void BtnNumber_Click(object sender, RoutedEventArgs e) {
            string number = (sender as Button).Content.ToString();
            tbResult.Text += number;

            int index = infixEquation.Count - 1;

            if (index >= 0 && double.TryParse(infixEquation[index], out _))
                infixEquation[index] += number;
            else
                infixEquation.Add(number);
        }

        private void btnDecimal_Click(object sender, RoutedEventArgs e) {
            if (infixEquation.Count > 0 && double.TryParse(infixEquation[^1], out _)) {
                if (!infixEquation[^1].Contains('.')) {
                    infixEquation[^1] += '.';
                    tbResult.Text += '.';
                }
            }
            else {
                infixEquation.Add("0.");
                tbResult.Text += "0.";
            }
        }

        private void BtnSign_Click(object sender, RoutedEventArgs e) {
            if (infixEquation.Count > 0 && double.TryParse(infixEquation[^1], out double value)) {
                value *= -1;
                infixEquation[^1] = value.ToString();

                tbResult.Text = "";
                foreach(string s in infixEquation) {
                    tbResult.Text += s;
                }
            }
        }

        private void BtnOperator_Click(object sender, RoutedEventArgs e) {
            if (infixEquation.Count > 0 && (double.TryParse(infixEquation[^1], out double _) || infixEquation[^1] == ")") ) {
                string op = (sender as Button).Content.ToString();
                tbResult.Text += op;
                infixEquation.Add(op);
            }
        }

        private void BtnEquals_Click(object sender, RoutedEventArgs e) {
            if (tbResult.Text != "") {
                postfixEquation = InfixToPostfix();
                if (postfixEquation[0] != "Error") {
                    tbResult.Text = "";
                    foreach (string s in postfixEquation)
                        tbResult.Text += $"{s} ";

                    btnEquals.Click += SolvePostFix;
                    btnEquals.Content = "=";
                }
                else
                    tbResult.Text = "Error";
            }
        }

        private void SolvePostFix(object sender, RoutedEventArgs e) {
            ///TODO : Check for divide by 0 error
            Stack<string> stk = new Stack<string>();
            double x;
            double y;
            foreach (string s in postfixEquation) {
                if (double.TryParse(s, out _))
                    stk.Push(s);
                else {
                    y = double.Parse(stk.Pop());
                    x = double.Parse(stk.Pop());
                    switch (s) {
                        case "*":
                            stk.Push( (x * y).ToString() );
                            break;
                        case "/":
                            stk.Push((x / y).ToString());
                            break;
                        case "+":
                            stk.Push((x + y).ToString());
                            break;
                        case "-":
                            stk.Push((x - y).ToString());
                            break;
                    }
                }
            }

            tbResult.Text = stk.Pop();
            btnEquals.Click += BtnEquals_Click;
            btnEquals.Content = "=>";
        }

        private List<string> InfixToPostfix() {
            Stack<string> stk = new Stack<string>();
            stk.Push("#");
            List<string> postfixEquation = new List<string>();

            foreach (string s in infixEquation) {
                if (double.TryParse(s, out _)) 
                    postfixEquation.Add(s);
                else if(s == "(")
                    stk.Push("(");
                else if(s == ")") {
                    while(stk.Peek() != "#" && stk.Peek() != "(") 
                        postfixEquation.Add(stk.Pop());

                    if(stk.Peek() != "(") { //More right parentheses than left 
                        postfixEquation.Clear();
                        postfixEquation.Add("Error");
                        return postfixEquation;                        
                    }
                    else
                        stk.Pop();
                }
                else {
                    if (Precedence(s) > Precedence(stk.Peek()))
                        stk.Push(s);
                    else {
                        while (stk.Peek() != "#" && Precedence(s) <= Precedence(stk.Peek()))
                            postfixEquation.Add(stk.Pop());

                        stk.Push(s);
                    }                   
                }
            }

            while(stk.Peek() != "#") {
                if(stk.Peek() != "(")
                    postfixEquation.Add(stk.Pop());
                else { //More left paranthese than right
                    postfixEquation.Clear();
                    postfixEquation.Add("Error");
                    return postfixEquation;
                }
            }

            return postfixEquation;
        }

        private int Precedence(string op) {
            if (op == "+" || op == "-")
                return 1;
            if (op == "*" || op == "/")
                return 2;
            else
                return 0;
        }
    }
}
