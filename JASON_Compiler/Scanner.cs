using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    Else, ElseIF, EndUntil, EndWhile, IF, INT_DATA_TYPE,
    Read, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, Left_Paranthesis,
    Right_Paranthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp,
    MinusOp, MultiplyOp, DivideOp,
    Idenifier, NUMBER, STRING, STRING_DATA_TYPE, FLOAT_DATA_TYPE,
    ASSIGN, RETURN, left_Curly, Right_curly, REPEAT,
    ENDLINE, Comment, Main, OR, AND, INCREMENT, DECREMENT, NOT,End
}
namespace JASON_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> SpicalChars = new Dictionary<string, Token_Class>();
        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("repeat", Token_Class.REPEAT);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.ElseIF);
            ReservedWords.Add("return", Token_Class.RETURN);
            ReservedWords.Add("int", Token_Class.INT_DATA_TYPE);
            ReservedWords.Add("string", Token_Class.STRING_DATA_TYPE);
            ReservedWords.Add("float", Token_Class.FLOAT_DATA_TYPE);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("end", Token_Class.End);
            //
            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.Left_Paranthesis);
            Operators.Add(")", Token_Class.Right_Paranthesis);
            Operators.Add("{", Token_Class.left_Curly);
            Operators.Add("}", Token_Class.Right_curly);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add(":=", Token_Class.ASSIGN);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("++", Token_Class.INCREMENT);
            Operators.Add("--", Token_Class.DECREMENT);
            Operators.Add("!=", Token_Class.DECREMENT);
            //
            SpicalChars.Add("||", Token_Class.OR);
            SpicalChars.Add("&&", Token_Class.AND);
        }

        public void StartScanning(string SourceCode)
        {
            // i: Outer loop to check on lexemes.
            for (int i = 0; i < SourceCode.Length; i++)
            {
                // j: Inner loop to check on each character in a single lexeme.
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;
                //idintifire
                if (char.IsLetter(CurrentChar))
                {
                    CurrentLexeme = "";
                    while (Char.IsDigit(CurrentChar) || Char.IsLetter(CurrentChar))
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                //number
                else if (char.IsDigit(CurrentChar))
                {
                    CurrentLexeme = "";
                    while (Char.IsDigit(CurrentChar) || CurrentChar == '.')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j >= SourceCode.Length)
                            break;
                        CurrentChar = SourceCode[j];
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }
                //string
                else if (CurrentChar == '"')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += SourceCode[j].ToString();
                        if (CurrentChar == '"')
                            break;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                //comment
                else if (SourceCode[i] == '/' && SourceCode[i + 1] == '*')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (SourceCode[j] == '*' && SourceCode[j + 1] == '/')
                        {
                            j++;
                            CurrentLexeme += SourceCode[j];
                            break;
                        }
                        i = j + 2;

                    }
                    FindTokenClass(CurrentLexeme);
                }
                //||
                else if (SourceCode[i] == '|' && SourceCode[i + 1] == '|')
                {

                    CurrentLexeme += SourceCode[i].ToString();
                    i++;
                    FindTokenClass(CurrentLexeme);
                }
                //&&
                else if (SourceCode[i] == '&' && SourceCode[i + 1] == '&')
                {
                    CurrentLexeme += SourceCode[i].ToString();
                    i++;
                    FindTokenClass(CurrentLexeme);
                }
                //symbole
                else
                {
                    CurrentLexeme = SourceCode[i].ToString();
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (Char.IsSymbol(CurrentChar))
                            CurrentLexeme += SourceCode[j].ToString();
                        else
                            break;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
            }
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (isReservedWord(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //comment
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            //string
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.STRING;
                Tokens.Add(Tok);
            }
            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.NUMBER;
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            //Is it an isSymbol?
            else if (isSymbol(Lex))
            {
                if (Lex == "+")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "-")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "*")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "/")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == ".")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == ";")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == ",")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == ")")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "(")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "}")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "{")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "=")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == ":=")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "<>")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == ">")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "<")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "++")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "--")
                {
                    Tok.token_type = Operators[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "!=")
                {
                    FindTokenClass("=");
                    Errors.Error_List.Add("!");
                }
                else
                {
                    Errors.Error_List.Add(Lex);
                }
            }
            //Is it isSpicalChars?
            else if (isSpicalChars(Lex))
            {
                if (Lex == "||")
                {
                    Tok.token_type = SpicalChars[Lex];
                    Tokens.Add(Tok);
                }
                else if (Lex == "&&")
                {
                    Tok.token_type = SpicalChars[Lex];
                    Tokens.Add(Tok);
                }
                else
                {
                    Errors.Error_List.Add(Lex);
                }
            }
            //errors
            else
                Errors.Error_List.Add(Lex);
        }
        bool isComment(string lex)
        {
            Regex reg = new Regex(@"/\*[\w\W\s]*\*/", RegexOptions.Compiled);
            if (reg.IsMatch(lex))
                return true;
            return false;
        }
        bool isString(string lex)
        {
            Regex reg = new Regex(@"[""][\w\W\s]*[""]", RegexOptions.Compiled);
            if (reg.IsMatch(lex))
                return true;
            return false;
        }
        bool isReservedWord(string lex)
        {
            if (ReservedWords.ContainsKey(lex))
                return true;
            return false;

        }
        bool isSymbol(string lex)
        {
            if (Operators.ContainsKey(lex))
                return true;
            return false;

        }
        bool isSpicalChars(string lex)
        {
            if (SpicalChars.ContainsKey(lex))
                return true;
            return false;

        }
        bool isIdentifier(string lex)
        {

            Regex reg = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*", RegexOptions.Compiled);
            if (reg.IsMatch(lex))
                return true;
            return false;
        }
        bool isConstant(string lex)
        {
            Regex reg = new Regex(@"^[+|-]?[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            if (reg.IsMatch(lex))
                return true;
            return false;
        }

    }
}