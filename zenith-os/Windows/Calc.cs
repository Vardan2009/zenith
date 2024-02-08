
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using zenithos.Controls;

namespace zenithos.Windows
{
    internal class Calc : Window
    {
        public List<List<Button>> buttons = new();
        public List<Button> additionalButtons = new();
        public Button plusButton, minusButton, mulButton, divButton,eqButton,cButton,expandButton,sqrtButton;
        public Label result;
        double first, second;
        string operation = null;
        bool changingSecond = false;
        bool expanded = false;
        public Calc() : base(20, 20, 240, 270, "Calculator", Kernel.defFont)
        {
            int e = 1;
            for(int i =0;i < 3; i++)
            {
                List<Button> cur = new List<Button>();
                for (int j =0;j < 3; j++)
                {
                    Button nw = new(e.ToString(), 20+j * 50, 50+i * 50, Kernel.textColDark, Kernel.defFont, 15);
                    controls.Add(nw);
                    cur.Add(nw);
                    e++;
                }
                buttons.Add(cur);
            }
            plusButton = new Button("+",170,50,Kernel.textColDark, Kernel.defFont, 15);
            minusButton = new Button("-", 170, 100, Kernel.textColDark, Kernel.defFont, 15);
            mulButton = new Button("*", 170, 150,Kernel.textColDark, Kernel.defFont,15);
            divButton = new Button("/", 170, 200,Kernel.textColDark,Kernel.defFont,15);
            expandButton = new Button(">", 190, 20, Kernel.textColDark, Kernel.defFont, 5);
            eqButton = new Button("=", 120, 200, Kernel.textColDark, Kernel.defFont, 15);
            cButton = new Button("C", 20, 200, Kernel.textColDark, Kernel.defFont, 15);
            controls.Add(new Button("0", 70, 200, Kernel.textColDark, Kernel.defFont, 15));
            result = new Label("", 20, 20, Kernel.defFont, Kernel.textColDark);
            sqrtButton = new Button("SQRT", 220, 50, Kernel.textColDark, Kernel.defFont, 15);
            controls.Add(sqrtButton);
            additionalButtons.Add(sqrtButton);
            controls.Add(plusButton);
            controls.Add(minusButton);
            controls.Add(mulButton);
            controls.Add(divButton);
            controls.Add(eqButton);
            controls.Add(cButton);
            controls.Add(result);
            controls.Add(expandButton);
            foreach (Button b in additionalButtons)
            {
                b.Visible = false;
            }
        }
        public override void Update(VBECanvas canv, int mX, int mY, bool mD, int dmX, int dmY)
        {
            base.Update(canv, mX, mY, mD, dmX, dmY);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttons[i][j].clickedOnce)
                    {
                        if(changingSecond)
                        {
                            second = Convert.ToInt32(second.ToString() + buttons[i][j].Text);
                        }
                        else
                        {
                            first = Convert.ToInt32(first.ToString() + buttons[i][j].Text);
                        }
                    }
                }
            }

            if(expandButton.clickedOnce)
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

            if(changingSecond)
            {
                result.Text = second.ToString();
            }
            else
            {
                result.Text = first.ToString();
            }

            if(sqrtButton.clickedOnce)
            {
                if(changingSecond)
                {
                    second = Math.Sqrt(second);
                }
                else
                {
                    first = Math.Sqrt(first);
                }
            }

            if (cButton.clickedOnce)
            {
                first = 0;
                second = 0;
                operation = null;
                changingSecond = false;
            }

            if(plusButton.clickedOnce)
            {
                operation = "+";
                changingSecond = true;
            }

            if (minusButton.clickedOnce)
            {
                operation = "-";
                changingSecond = true;
            }

            if (mulButton.clickedOnce)
            {
                operation = "*";
                changingSecond = true;
            }
            if(divButton.clickedOnce)
            {
                operation = "/";
                changingSecond = true;
            }

            if (eqButton.clickedOnce)
            {
                if (operation != null)
                {
                    double result = 0;
                    switch (operation)
                    {
                        case "+":
                            result = first + second;
                            break;
                        case "-":
                            result = first - second;
                            break;
                        case "*":
                            result = first * second;
                            break;
                        case "/":
                            result = first / second;
                            break;
                    }
                    second = 0;
                    changingSecond = false;
                    first = result;
                }
            }
        }
    }
}
