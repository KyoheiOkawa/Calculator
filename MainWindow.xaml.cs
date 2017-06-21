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

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _isCalculated = false;

        string _calcStr;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var str = "button";

            if (_isCalculated)
            {
                calcText.Text = "";
                _isCalculated = false;
            }

            for(int i = 0; i <= 9; i++)
            {
                var newStr = str + i.ToString();

                if(button.Name == newStr)
                {
                    if (!(i == 0 && calcText.Text == ""))
                    {
                        calcText.Text += i.ToString();
                        _calcStr += i.ToString();
                    }
                }
            }

            //string before = calcText.Text;
            //string after = before.Replace(",", "");
            //calcText.Text = string.Format("{0:#,0}", int.Parse(after));
        }

        private void OperatorClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var buttonName = button.Name;
            const string str = "operator_";

            if (buttonName == str + "plus")
                _calcStr += "+";
            else if (buttonName == str + "minus")
                _calcStr += "-";
            else if (buttonName == str + "mul")
                _calcStr += "*";
            else if (buttonName == str + "div")
                _calcStr += "/";

            calcText.Text = "";
        }

        private void EqualClick(object sender, RoutedEventArgs e)
        {
            var root = new Node(_calcStr);

            root.Parse();

            calcText.Text = root.Calculate().ToString();

            _calcStr = "";

            _isCalculated = true;
        }
    }

    //参考
    //http://smdn.jp/programming/tips/polish/
    class Node
    {
        public string _expression;
        public Node _left = null;
        public Node _right = null;

        public Node(string expression)
        {
            this._expression = expression;
        }

        public void Parse()
        {
            var posOperator = GetOperatorPos(_expression);

            if(posOperator < 0)
            {
                _left = null;
                _right = null;
                return;
            }

            _left = new Node(RemoveBracket(this._expression.Substring(0, posOperator)));
            _left.Parse();

            _right = new Node(RemoveBracket(this._expression.Substring(posOperator + 1)));
            _right.Parse();

            this._expression = this._expression.Substring(posOperator, 1);
        }

        private static string RemoveBracket(string str)
        {
            if (!(str.StartsWith("(") && str.EndsWith(")")))
                return str;

            var nest = 1;

            for (var i = 1; i < str.Length - 1; i++)
            {
                if (str[i] == '(')
                    nest++;
                else if (str[i] == ')')
                    nest--;

                if (nest == 0)
                    return str;
            }

            if (nest != 1)
                throw new Exception(string.Format("unbalanced bracket : {0}", str));

            str = str.Substring(1, str.Length - 2);

            if (str.StartsWith("("))
                return RemoveBracket(str);
            else
                return str;
        }

        private static int GetOperatorPos(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return -1;

            var pos = -1;
            var nest = 0;
            var priority = 0;
            var lowestPriority = 4;

            for(var i = 0; i < expression.Length; i++)
            {
                switch (expression[i])
                {
                    case '=': priority = 1; break;
                    case '+': priority = 2; break;
                    case '-': priority = 2; break;
                    case '*': priority = 3; break;
                    case '/': priority = 3; break;
                    case '(': nest++; continue;
                    case ')': nest--; continue;
                    default: continue;
                }

                if(nest == 0 && priority <= lowestPriority)
                {
                    lowestPriority = priority;
                    pos = i;
                }
            }

            return pos;
        }

        public float Calculate()
        {
            if(_left != null && _right != null)
            {
                var leftOperand = _left.Calculate();
                var rightOperand = _right.Calculate();

                switch(this._expression)
                {
                    case "+": return leftOperand + rightOperand;
                    case "-": return leftOperand - rightOperand;
                    case "*": return leftOperand * rightOperand;
                    case "/": return leftOperand / rightOperand;
                    default: return 0.0f;
                }
            }
            else
            {
                return float.Parse(_expression);
            }
        }
    }
}
