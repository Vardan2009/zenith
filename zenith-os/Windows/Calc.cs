﻿
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;
using zenithos.Utils;


namespace zenithos.Windows
{
    internal class Calc : Window
    {
        public List<List<Button>> buttons = new();

        public List<Button> additionalButtons = new();
        public Button plusButton, minusButton, mulButton,dotButton, divButton, eqButton, cButton, expandButton, sqrtButton,lParenButton,rParenButton;
        public Label result;

        public string expression;

        bool expanded = false;

        [ManifestResourceStream(ResourceName = "zenithos.Resource.Applogos.calc.bmp")]
        readonly static byte[] logoBytes;


        public Calc() : base(100, 100, 240, 270, "Calculator", Kernel.defFont)
        {
            logo = new Bitmap(logoBytes);
            int e = 1;
            for (int i = 0; i < 3; i++)
            {
                List<Button> cur = new List<Button>();
                for (int j = 0; j < 3; j++)
                {
                    Button nw = new(e.ToString(), 20 + j * 50, 50 + i * 50, Kernel.mainCol, Kernel.defFont, 15);
                    controls.Add(nw);
                    cur.Add(nw);
                    e++;
                }
                buttons.Add(cur);
            }
            Button zeroButton = new Button("0", 70, 200, Kernel.mainCol, Kernel.defFont, 15);
            controls.Add(zeroButton);
            List<Button> last = new()
            {
              zeroButton
            };
            buttons.Add(last);
            plusButton = new Button("+", 170, 50, Kernel.mainCol, Kernel.defFont, 15);
            minusButton = new Button("-", 170, 100, Kernel.mainCol, Kernel.defFont, 15);
            mulButton = new Button("*", 170, 150, Kernel.mainCol, Kernel.defFont, 15);
            divButton = new Button("/", 170, 200, Kernel.mainCol, Kernel.defFont, 15);
            expandButton = new Button(">", 190, 20, Kernel.mainCol, Kernel.defFont, 5);
            eqButton = new Button("=", 120, 200, Kernel.mainCol, Kernel.defFont, 15);
            cButton = new Button("C", 20, 200, Kernel.mainCol, Kernel.defFont, 15);
            result = new Label("", 20, 20, Kernel.defFont, Kernel.textColDark);
            sqrtButton = new Button("sqrt", 220, 50, Kernel.mainCol, Kernel.defFont, 15,null,50);
            lParenButton = new Button("(",220,100,Kernel.mainCol,Kernel.defFont, 15,null,50);
            rParenButton = new Button(")", 220, 150, Kernel.mainCol, Kernel.defFont, 15,null, 50);
            dotButton = new Button(".", 220, 200, Kernel.mainCol, Kernel.defFont, 15, null, 50);

            controls.AddRange( new Control[]
            {
                sqrtButton,
                lParenButton,
                rParenButton,
                plusButton,
                minusButton,
                mulButton,
                divButton,
                eqButton,
                cButton,
                result,
                expandButton,
                dotButton
            });

            additionalButtons.AddRange(new[] { sqrtButton, lParenButton, rParenButton,dotButton });


            foreach (Button b in additionalButtons)
            {
                b.Visible = false;
            }
        }
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            try
            {

                base.Update(canv, mX, mY, mD, dmX, dmY);

                for (int i = 0; i < buttons.Count; i++)
                {
                    for (int j = 0; j < buttons[i].Count; j++)
                    {
                        if (buttons[i][j].clickedOnce)
                        {
                            expression += buttons[i][j].Text;
                        }
                    }
                }

                if (expandButton.clickedOnce)
                {
                    expanded = !expanded;
                    if (expanded)
                    {
                        w = 240 + 50;
                        expandButton.Text = "<";
                        foreach (Button b in additionalButtons)
                        {
                            b.Visible = true;
                        }
                    }
                    else
                    {
                        w = 240;
                        expandButton.Text = ">";
                        foreach (Button b in additionalButtons)
                        {
                            b.Visible = false;
                        }
                    }
                }


                result.Text = expression;


                if (sqrtButton.clickedOnce)
                {
                    string expression_old = expression;
                    expression = ExpressionParser.ParseExpression(expression).ToString();
                    double sqrt = Math.Sqrt(Convert.ToDouble(expression));
                    if (double.IsNaN(sqrt))
                    {
                        Kernel.ShowMessage("Don't try doing this, you might break something...", "Real Numbers", MsgType.Info);
                        expression = expression_old;
                    }
                    else
                    {
                        expression = sqrt.ToString();
                    }
                }

                if (cButton.clickedOnce)
                {
                    expression = "";
                }

                if (plusButton.clickedOnce)
                {
                    if(!expression.EndsWith("+"))
                    {
                        if (ExpressionParser.IsOperator(expression[^1]))
                        {
                            expression = expression[..^1] + "+";
                        }
                        else
                        {
                            expression += '+';
                        }
                    }
                    
                }

                if (minusButton.clickedOnce)
                {
                    if (!expression.EndsWith("-"))
                    {
                        if (ExpressionParser.IsOperator(expression[^1]))
                        {
                            expression = expression[..^1] + "-";
                        }
                        else
                        {
                            expression += '-';
                        }
                    }
                }

                if (dotButton.clickedOnce)
                {
                    if (!expression.EndsWith("."))
                    {
                        if (ExpressionParser.IsOperator(expression[^1]))
                        {
                            expression = expression[..^1] + ".";
                        }
                        else
                        {
                            expression += '.';
                        }
                    }
                }

                if (mulButton.clickedOnce)
                {
                    if (!expression.EndsWith("*"))
                    {
                        if (ExpressionParser.IsOperator(expression[^1]))
                        {
                            expression = expression[..^1] + "*";
                        }
                        else
                        {
                            expression += '*';
                        }
                    }
                }

                if (divButton.clickedOnce)
                {
                    if (!expression.EndsWith("/"))
                    {
                        if (ExpressionParser.IsOperator(expression[^1]))
                        {
                            expression = expression[..^1] + "/";
                        }
                        else
                        {
                            expression += '/';
                        }
                    }
                }

                if (lParenButton.clickedOnce)
                {
                    expression += '(';
                }

                if (rParenButton.clickedOnce)
                {
                    expression += ')';
                }

                if (eqButton.clickedOnce)
                {
                    expression = ExpressionParser.ParseExpression(expression).ToString();
                }
            }
            catch (Exception ex)
            {
                Kernel.ShowMessage(ex.Message, "Calculator", MsgType.Error);
                Close();
            }
        }
    }
}
