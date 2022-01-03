using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            for (; TokenStream[InputPointer].token_type == Token_Class.Comment;)
            {
                program.Children.Add(match(Token_Class.Comment));
            }

            for (;
            TokenStream[InputPointer + 1].token_type == Token_Class.Idenifier && (
            TokenStream[InputPointer].token_type == Token_Class.FLOAT_DATA_TYPE ||
            TokenStream[InputPointer].token_type == Token_Class.INT_DATA_TYPE ||
            TokenStream[InputPointer].token_type == Token_Class.STRING_DATA_TYPE
            );)
            {
                if (TokenStream[InputPointer + 2].token_type == Token_Class.Left_Paranthesis)
                    program.Children.Add(K());
                else
                    program.Children.Add(Declaration_stat());
            }
 
            for (; TokenStream[InputPointer].token_type == Token_Class.Comment;)
            {
                program.Children.Add(match(Token_Class.Comment));
            }

            program.Children.Add(Main_Function());
            if (TokenStream.Count > InputPointer)
                for (; TokenStream[InputPointer].token_type == Token_Class.Comment;)
                {
                     program.Children.Add(match(Token_Class.Comment));
                }

            if (TokenStream.Count>InputPointer)
            for (;
            TokenStream[InputPointer + 1].token_type == Token_Class.Idenifier && (
            TokenStream[InputPointer].token_type == Token_Class.FLOAT_DATA_TYPE ||
            TokenStream[InputPointer].token_type == Token_Class.INT_DATA_TYPE ||
            TokenStream[InputPointer].token_type == Token_Class.STRING_DATA_TYPE
            );)
            {
                if (TokenStream[InputPointer + 2].token_type == Token_Class.Left_Paranthesis)
                    program.Children.Add(K());
                else
                    program.Children.Add(Declaration_stat());
            }

            if (TokenStream.Count > InputPointer)
                for (; TokenStream[InputPointer].token_type == Token_Class.Comment;)
                {
                    program.Children.Add(match(Token_Class.Comment));
                }
            MessageBox.Show("Success");
            return program;
        }
        Node Comment()
        {
            Node k = new Node("Comment");
            for (; TokenStream[InputPointer].token_type == Token_Class.Comment;)
            {
                k.Children.Add(match(Token_Class.Comment));
            }
           
            return k;
        }
        Node K()
        {
            Node k = new Node("No Main Function");
            k.Children.Add(Function_stat());
            // k.Children.Add(K());
            return k;
        }
        Node Main_Function()
        {
            Node mainFunction = new Node("Main_Function");
            mainFunction.Children.Add(Data_Type());
            mainFunction.Children.Add(match(Token_Class.Main));
            mainFunction.Children.Add(match(Token_Class.Left_Paranthesis));
            mainFunction.Children.Add(match(Token_Class.Right_Paranthesis));
            mainFunction.Children.Add(Function_body());
            return mainFunction;
        }
        Node Function_stat()
        {
            Node functionStat = new Node("Function_Stat");
            functionStat.Children.Add(Function_declaration());
            functionStat.Children.Add(Function_body());
            return functionStat;
        }
        Node Function_body()
        {
            Node functionBody = new Node("Function_body");
            functionBody.Children.Add(match(Token_Class.left_Curly));
            
            functionBody.Children.Add(Statement());
            // functionBody.Children.Add(Return_stat());
            functionBody.Children.Add(match(Token_Class.Right_curly));
            return functionBody;
        }
        Node Return_stat()
        {
            Node returnStat = new Node("Return_stat");
            returnStat.Children.Add(match(Token_Class.RETURN));
            returnStat.Children.Add(Expression());
            returnStat.Children.Add(match(Token_Class.Semicolon));
            return returnStat;
        }
        Node Expression()
        {
            Node expression = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.STRING)
            {
                expression.Children.Add(match(Token_Class.STRING));
                return expression;
            }

            if (TokenStream[InputPointer].token_type == Token_Class.NUMBER ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp ||
                    TokenStream[InputPointer + 1].token_type == Token_Class.NUMBER)
                {
                    expression.Children.Add(Equation());
                    return expression;
                }

            }
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.NUMBER)
            {

                expression.Children.Add(Term());
                return expression;

            }
            return null;

        }
        Node Data_Type()
        {
            Node dataType = new Node("Data_Type");
            if (TokenStream[InputPointer].token_type == Token_Class.INT_DATA_TYPE)
            {
                dataType.Children.Add(match(Token_Class.INT_DATA_TYPE));
                return dataType;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.FLOAT_DATA_TYPE)
            {
                dataType.Children.Add(match(Token_Class.FLOAT_DATA_TYPE));
                return dataType;
            }

            else if (TokenStream[InputPointer].token_type == Token_Class.STRING_DATA_TYPE)
            {
                dataType.Children.Add(match(Token_Class.STRING_DATA_TYPE));
                return dataType;
            }
            else
                return null;
        }
        Node Arguments()
        {
            Node arguments = new Node("Arguments");


            arguments.Children.Add(Term());
            arguments.Children.Add(Arg());
            return arguments;


        }
        Node Arg()
        {
            Node arg = new Node("Arg");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arg.Children.Add(match(Token_Class.Comma));
                arg.Children.Add(Term());
                arg.Children.Add(Arg());
            }
            else return null;

            return arg;
        }
        Node ArgList()
        {
            Node argList = new Node("ArgList");
            if (TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                argList.Children.Add(match(Token_Class.Left_Paranthesis));
                argList.Children.Add(Arguments());
                argList.Children.Add(match(Token_Class.Right_Paranthesis));

            }
            else
                return null;

            return argList;
        }
        Node Arguments1()
        {
            Node arguments = new Node("Arguments");
            if (TokenStream[InputPointer].token_type == Token_Class.FLOAT_DATA_TYPE ||
                TokenStream[InputPointer].token_type == Token_Class.INT_DATA_TYPE ||
                (TokenStream[InputPointer].token_type == Token_Class.STRING_DATA_TYPE))
            {
                arguments.Children.Add(Data_Type());
                arguments.Children.Add(match(Token_Class.Idenifier));
                arguments.Children.Add(Arg1());
                return arguments;
            }
            return null;
        }
        Node Arg1()
        {
            Node arg = new Node("Arg");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                arg.Children.Add(match(Token_Class.Comma));
                arg.Children.Add(Data_Type());
                arg.Children.Add(match(Token_Class.Idenifier));
                arg.Children.Add(Arg1());
            }
            else return null;

            return arg;
        }
        Node ArgList1()
        {
            Node argList = new Node("ArgList");
            if (TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                argList.Children.Add(match(Token_Class.Left_Paranthesis));
                argList.Children.Add(Arguments1());
                argList.Children.Add(match(Token_Class.Right_Paranthesis));
            }
            else
                return null;
            return argList;
        }
        Node Function_declaration()
        {
            Node functionDeclaration = new Node("Function_declaration");
            functionDeclaration.Children.Add(Data_Type());
            functionDeclaration.Children.Add(match(Token_Class.Idenifier));
            functionDeclaration.Children.Add(ArgList1());
            return functionDeclaration;
        }
        Node Function_Call()
        {
            Node functionCall = new Node("Function Call");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(ArgList());
            return functionCall;
        }
        Node Term()
        {
            Node term = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.NUMBER)
            {
                term.Children.Add(match(Token_Class.NUMBER));
                return term;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.Left_Paranthesis)
                {
                    term.Children.Add(Function_Call());
                    return term;
                }
                term.Children.Add(match(Token_Class.Idenifier));
                return term;
            }
            return null;
        }
        Node Equation()
        {
            Node equation = new Node("Equation");

            equation.Children.Add(G());
            //equation.Children.Add(L());

            return equation;

        }
        Node G()
        {
            Node g = new Node("G");
            if (TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                g.Children.Add(match(Token_Class.Left_Paranthesis));
                g.Children.Add(L());
                g.Children.Add(match(Token_Class.Right_Paranthesis));
                g.Children.Add(G());
                return g;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NUMBER ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                g.Children.Add(Term());
                g.Children.Add(Arthmatic_Operator());
                g.Children.Add(G());
                return g;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.PlusOp ||
                TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                g.Children.Add(Arthmatic_Operator());
                g.Children.Add(G());
                return g;
            }
            return null;
        }
        Node L()
        {
            Node l = new Node("L");
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                l.Children.Add(match(Token_Class.PlusOp));
                l.Children.Add(L());
                return l;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                l.Children.Add(match(Token_Class.MinusOp));
                l.Children.Add(L());
                return l;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                l.Children.Add(match(Token_Class.MultiplyOp));
                l.Children.Add(L());
                return l;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                l.Children.Add(match(Token_Class.DivideOp));
                l.Children.Add(L());
                return l;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NUMBER ||
                 TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                l.Children.Add(Term());
                l.Children.Add(L());
                return l;
            }
            else
            {
                return null;
            }

        }
        Node Arthmatic_Operator()
        {
            Node arthmatic_Operator = new Node("Arthmatic_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                arthmatic_Operator.Children.Add(match(Token_Class.PlusOp));
                return arthmatic_Operator;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                arthmatic_Operator.Children.Add(match(Token_Class.MinusOp));
                return arthmatic_Operator;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                arthmatic_Operator.Children.Add(match(Token_Class.MultiplyOp));
                return arthmatic_Operator;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                arthmatic_Operator.Children.Add(match(Token_Class.DivideOp));
                return arthmatic_Operator;
            }
            return null;

        }
        Node D()
        {
            Node d = new Node("D");
            d.Children.Add(match(Token_Class.Left_Paranthesis));
            d.Children.Add(Y());
            d.Children.Add(match(Token_Class.Right_Paranthesis));
            d.Children.Add(Y());
            return d;
        }
        Node Y()
        {
            Node y = new Node("Y");
            if (Arthmatic_Operator() != null)
            {
                y.Children.Add(Arthmatic_Operator());
                y.Children.Add(Y());
                return y;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                y.Children.Add(D());
                return y;
            }
            else if (Term() != null)
            {
                y.Children.Add(Term());
                y.Children.Add(Y());
                return y;
            }
            return null;
        }
        Node Assigment_stat()
        {
            Node assigmentStat = new Node("Assigment stat");
            assigmentStat.Children.Add(match(Token_Class.Idenifier));
            assigmentStat.Children.Add(match(Token_Class.ASSIGN));

            assigmentStat.Children.Add(Expression());
            return assigmentStat;
        }
        Node Declaration_stat()
        {
            Node declarationStat = new Node("Declaration stat");
            declarationStat.Children.Add(Data_Type());
            declarationStat.Children.Add(A());
            declarationStat.Children.Add(match(Token_Class.Semicolon));
            return declarationStat;
        }
        Node A()
        {
            Node a = new Node("A");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.ASSIGN)
                {
                    a.Children.Add(Assigment_stat());
                    a.Children.Add(A());
                    return a;
                }
                else
                {
                    a.Children.Add(match(Token_Class.Idenifier));
                    a.Children.Add(A());
                    return a;
                }

            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                a.Children.Add(match(Token_Class.Comma));
                a.Children.Add(A());
                return a;
            }
            return null;
        }
        Node Write_stat()
        {
            Node writeStat = new Node("Write_stat");
            writeStat.Children.Add(match(Token_Class.Write));
            writeStat.Children.Add(M());
            writeStat.Children.Add(match(Token_Class.Semicolon));
            return writeStat;
        }
        Node M()
        {
            Node m = new Node("M");

            /*if (TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                m.Children.Add(Expression());
                return m;
            }*/

            if (TokenStream[InputPointer].token_type == Token_Class.ENDLINE)
            {
                m.Children.Add(match(Token_Class.ENDLINE));
                return m;
            }
            else
            {
                m.Children.Add(Expression());
                return m;
            }

        }
        Node Read_stat()
        {
            Node readStat = new Node("Read_stat");
            readStat.Children.Add(match(Token_Class.Read));
            readStat.Children.Add(match(Token_Class.Idenifier));
            readStat.Children.Add(match(Token_Class.Semicolon));
            return readStat;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(Condition_operator());
            condition.Children.Add(Term());
            return condition;
        }
        Node Boolean_operator()
        {
            Node booleanOperator = new Node("Boolean_operator");
            if (TokenStream[InputPointer].token_type == Token_Class.AND)
            {
                booleanOperator.Children.Add(match(Token_Class.AND));
            }
            else
            {
                booleanOperator.Children.Add(match(Token_Class.OR));
            }
            return booleanOperator;
        }
        Node Condition_operator()
        {
            Node condition_operator = new Node("Condition_operator");
            if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                condition_operator.Children.Add(match(Token_Class.LessThanOp));
                return condition_operator;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                condition_operator.Children.Add(match(Token_Class.GreaterThanOp));
                return condition_operator;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                condition_operator.Children.Add(match(Token_Class.EqualOp));
                return condition_operator;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                condition_operator.Children.Add(match(Token_Class.NotEqualOp));
                return condition_operator;
            }

            return null;
        }
        Node Condition_stat()
        {
            Node conditionStat = new Node("Condition_stat");
            conditionStat.Children.Add(Condition());
            conditionStat.Children.Add(Z());
            return conditionStat;
        }
        Node Z()
        {
            Node z = new Node("Z");
            if (TokenStream[InputPointer].token_type == Token_Class.AND ||
                TokenStream[InputPointer].token_type == Token_Class.OR
                )
            {
                z.Children.Add(Boolean_operator());
                z.Children.Add(Condition());
                z.Children.Add(Z());
                return z;
            }
            return null;
        }
        Node Statement()
        {
            if (InputPointer == TokenStream.Count) {
                InputPointer -= 1;
            }
            Node statement = new Node("Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    statement.Children.Add(match(Token_Class.Comment));
                    statement.Children.Add(Statement());
                }

                else
                {
                    statement.Children.Add(Statement());
                    return null;
                }

                return statement;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.ASSIGN)
                {
                    statement.Children.Add(Assigment_stat());
                    statement.Children.Add(match(Token_Class.Semicolon));
                    statement.Children.Add(Statement());
                    return statement;
                }
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.Left_Paranthesis)
                {
                    statement.Children.Add(Function_Call());
                    statement.Children.Add(Statement());
                    return statement;
                }

            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NUMBER || TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp || TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp || TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp)
                {
                    statement.Children.Add(Equation());
                    statement.Children.Add(Statement());
                    return statement;
                }
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Left_Paranthesis)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.NUMBER || TokenStream[InputPointer + 1].token_type == Token_Class.Idenifier)
                {
                    if (TokenStream[InputPointer + 2].token_type == Token_Class.MinusOp || TokenStream[InputPointer + 2].token_type == Token_Class.PlusOp || TokenStream[InputPointer + 2].token_type == Token_Class.MultiplyOp || TokenStream[InputPointer + 2].token_type == Token_Class.DivideOp)
                    {
                        statement.Children.Add(Equation());
                        statement.Children.Add(Statement());
                        return statement;
                    }
                }
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.ASSIGN)
                {
                    statement.Children.Add(Assigment_stat());
                    statement.Children.Add(Statement());
                    return statement;
                }
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.FLOAT_DATA_TYPE || TokenStream[InputPointer].token_type == Token_Class.INT_DATA_TYPE || (TokenStream[InputPointer].token_type == Token_Class.STRING_DATA_TYPE))
            {
                if (TokenStream[InputPointer + 1].token_type == Token_Class.Idenifier)
                {
                    statement.Children.Add(Declaration_stat());
                    statement.Children.Add(Statement());
                    return statement;
                }
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statement.Children.Add(Write_stat());
                statement.Children.Add(Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statement.Children.Add(Read_stat());
                statement.Children.Add(Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.RETURN)
            {
                statement.Children.Add(Return_stat());
                statement.Children.Add(Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                statement.Children.Add(If_stat());
                statement.Children.Add(Statement());

                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.REPEAT)
            {
                statement.Children.Add(Repeat_stat());
                statement.Children.Add(Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                statement.Children.Add(Else());
                statement.Children.Add(Statement());
                return statement;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.ElseIF)
            {
                statement.Children.Add(Else_If_Stat());
                statement.Children.Add(Statement());
                return statement;
            }

            return null;
        }
        Node If_stat()
        {
            Node ifStat = new Node("If_stat");
            if (TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                ifStat.Children.Add(match(Token_Class.IF));
                ifStat.Children.Add(H());
                ifStat.Children.Add(U());
                return ifStat;
            }
            return null;
        }
        Node H()
        {
            Node h = new Node("H");
            h.Children.Add(Condition_stat());
            h.Children.Add(match(Token_Class.Then));
            h.Children.Add(Statement());
            return h;
        }
        Node U()
        {
            Node u = new Node("U");
            if (TokenStream[InputPointer].token_type == Token_Class.ElseIF)
            {
                u.Children.Add(Else_If_Stat());
                u.Children.Add(U());
                return u;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                u.Children.Add(Else());
                return u;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.End)
            {
                u.Children.Add(match(Token_Class.End));
                return u;
            }
            return null;
        }
        Node Else_If_Stat()
        {
            Node else_If_Stat = new Node("Else_If_Stat");
            if (TokenStream[InputPointer].token_type == Token_Class.ElseIF)
            {
                else_If_Stat.Children.Add(match(Token_Class.ElseIF));
                else_If_Stat.Children.Add(H());
                else_If_Stat.Children.Add(U());
                return else_If_Stat;
            }
            return null;
        }
        Node Else()
        {
            Node elsee = new Node("Else");
            if (TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                elsee.Children.Add(match(Token_Class.Else));
                elsee.Children.Add(Statement());
                elsee.Children.Add(match(Token_Class.End));
                return elsee;
            }
            return null;
        }
        Node Repeat_stat()
        {
            Node repeat_stat = new Node("Repeat_stat");
            repeat_stat.Children.Add(match(Token_Class.REPEAT));
            repeat_stat.Children.Add(Statement());
            repeat_stat.Children.Add(match(Token_Class.Until));
            repeat_stat.Children.Add(Condition_stat());
            return repeat_stat;
        }
        // Implement your logic here
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }
        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
