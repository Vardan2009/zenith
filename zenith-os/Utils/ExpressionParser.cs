using System;
using System.Collections.Generic;

namespace zenithos.Utils
{
    class ExpressionParser
    {
        public static double ParseExpression(string expression)
        {
            Queue<string> outputQueue = new Queue<string>();
            Stack<char> operatorStack = new Stack<char>();

            string numBuffer = "";
            bool unaryFlag = true;

            foreach (char c in expression)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    numBuffer += c;
                    unaryFlag = false;
                }
                else if (IsOperator(c))
                {
                    if (unaryFlag && (c == '+' || c == '-'))
                    {
                        numBuffer += c;
                    }
                    else
                    {
                        if (numBuffer != "")
                        {
                            outputQueue.Enqueue(numBuffer);
                            numBuffer = "";
                        }

                        while (operatorStack.Count > 0 && Precedence(operatorStack.Peek()) >= Precedence(c))
                        {
                            outputQueue.Enqueue(operatorStack.Pop().ToString());
                        }

                        operatorStack.Push(c);
                        unaryFlag = true;
                    }
                }
                else if (c == '(')
                {
                    if (unaryFlag && (c == '+' || c == '-'))
                    {
                        numBuffer += c;
                    }
                    else
                    {
                        operatorStack.Push(c);
                        unaryFlag = true;
                    }
                }
                else if (c == ')')
                {
                    if (numBuffer != "")
                    {
                        outputQueue.Enqueue(numBuffer);
                        numBuffer = "";
                    }

                    while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                    {
                        outputQueue.Enqueue(operatorStack.Pop().ToString());
                    }

                    if (operatorStack.Count == 0)
                    {
                        throw new ArgumentException("Mismatched parentheses");
                    }

                    operatorStack.Pop(); // Discard '('
                    unaryFlag = false;
                }
                else if (c != ' ')
                {
                    throw new ArgumentException("Invalid character: " + c);
                }
            }

            if (numBuffer != "")
            {
                outputQueue.Enqueue(numBuffer);
            }

            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == '(')
                {
                    throw new ArgumentException("Mismatched parentheses");
                }
                outputQueue.Enqueue(operatorStack.Pop().ToString());
            }

            return EvaluateRPN(outputQueue);
        }

        static double EvaluateRPN(Queue<string> outputQueue)
        {
            Stack<double> stack = new Stack<double>();

            while (outputQueue.Count > 0)
            {
                string token = outputQueue.Dequeue();

                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                else if (IsOperator(token[0]))
                {
                    double b = stack.Pop();
                    double a = stack.Pop();

                    switch (token[0])
                    {
                        case '+':
                            stack.Push(a + b);
                            break;
                        case '-':
                            stack.Push(a - b);
                            break;
                        case '*':
                            stack.Push(a * b);
                            break;
                        case '/':
                            stack.Push(a / b);
                            break;
                        default:
                            throw new ArgumentException("Unknown operator: " + token);
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid token: " + token);
                }
            }

            if (stack.Count != 1)
            {
                throw new ArgumentException("Invalid expression");
            }

            return stack.Pop();
        }

        static bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/';
        }

        static int Precedence(char op)
        {
            switch (op)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                default:
                    return 0;
            }
        }
    }
}