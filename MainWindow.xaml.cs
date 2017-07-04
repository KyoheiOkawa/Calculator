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
        /// <summary>
        /// 計算を実行したかのフラグ
        /// </summary>
        bool _isCalculated = false;
        /// <summary>
        /// 実際に計算するのに使用する文字列
        /// </summary>
        string _calcStr;

        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 数字ボタンを押したときに呼ばれる関数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var str = "button";

            //計算後に数字ボタンが押されたらリセットする
            if (_isCalculated)
            {
                calcText.Text = "";
                NumericalFormula.Text = "";
                _calcStr = "";
                _isCalculated = false;
            }

            //前回入力した物が数字以外であったら表示をリセット
            if(!IsDigit(calcText.Text))
                calcText.Text = "";

            for(int i = 0; i <= 9; i++)
            {
                var newStr = str + i.ToString();

                if(button.Name == newStr)
                {
                    //0ボタンが押されたときに前回入力した数字がなかったら
                    //以下の処理を飛ばす
                    if (!(i == 0 && calcText.Text == ""))
                    {
                        calcText.Text += i.ToString();
                        _calcStr += i.ToString();
                        NumericalFormula.Text = _calcStr;
                    }
                }
            }
        }
        /// <summary>
        ///　演算子ボタン(+,-,*,/,(,))が押されたときに呼ばれる関数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            else if (buttonName == str + "LeftBracket")
                _calcStr += "(";
            else if (buttonName == str + "RightBracket")
                _calcStr += ")";

            //入力表示用テキストボックスに押された演算子の記号を表示する
            calcText.Text = _calcStr[_calcStr.Length-1].ToString();
            NumericalFormula.Text = _calcStr;
        }
        /// <summary>
        /// 小数点ボタンを押したときに呼ばれ、小数点を追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointClick(object sender, RoutedEventArgs e)
        {
            //現在入力途中の数に小数点が含まれていない場合に小数点を追加する
            if (calcText.Text.Contains("."))
            {
                _calcStr += ".";
                NumericalFormula.Text += ".";
                calcText.Text += ".";
            }
        }
        /// <summary>
        /// イコールボタンが押されたときに呼ばれる関数
        /// 計算を実行しその結果を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqualClick(object sender, RoutedEventArgs e)
        {
            //計算用２分木ルートノード作成
            var root = new Node(_calcStr);
            //計算用文字列を計算用２分木に展開
            root.Parse();

            //計算を実行し、その結果を表示する
            try
            {
                calcText.Text = root.Calculate().ToString();
            }
            //計算ができなかったらERRORを表示する
            catch(System.FormatException)
            {
                calcText.Text = "ERROR";
            }

            _isCalculated = true;
        }
        /// <summary>
        /// リセット（CE）ボタンが押されたときに呼ばれる
        /// 表示文字列、計算用文字列をリセットする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear(object sender, RoutedEventArgs e)
        {
            _calcStr = "";
            calcText.Text = "";
            NumericalFormula.Text = "";
            _isCalculated = false;
        }
        /// <summary>
        /// 文字列が数値であるか判定する
        /// </summary>
        /// <param name="str">判定する文字列</param>
        /// <returns>数値であったらtrueそうでなかったらfalse</returns>
        private bool IsDigit(string str)
        {
            float res;
            return float.TryParse(str,out res);
        }
    }

    //参考
    //http://smdn.jp/programming/tips/polish/
    /// <summary>
    /// 計算用の２分木ノード
    /// </summary>
    class Node
    {
        /// <summary>
        /// 自分のノードが持つ文字列
        /// </summary>
        public string _expression;
        /// <summary>
        /// 左子孫ノード
        /// </summary>
        public Node _left = null;
        /// <summary>
        /// 右子孫ノード
        /// </summary>
        public Node _right = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="expression">計算にかける文字列</param>
        public Node(string expression)
        {
            this._expression = expression;
        }
        /// <summary>
        /// 計算用２分木に展開する
        /// </summary>
        public void Parse()
        {
            _expression = RemoveBracket(_expression);
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

        /// <summary>
        /// 与えられた文字列の最も外側で囲われている括弧を外す
        /// </summary>
        /// <param name="str">括弧を外す文字列</param>
        /// <returns>最も外側の括弧が外された文字列</returns>
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
        /// <summary>
        /// 与えられた文字列の中で一番優先度の低い演算子の位置を調べる
        /// </summary>
        /// <param name="expression">調べる文字列</param>
        /// <returns>一番優先度の低い演算子があった位置</returns>
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
        /// <summary>
        /// 計算を実行する
        /// </summary>
        /// <returns>計算結果</returns>
        public float Calculate()
        {
            if (_left != null && _right != null)
            {
                var leftOperand = _left.Calculate();
                var rightOperand = _right.Calculate();

                switch (this._expression)
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
