using CT.Lab3.AST;
using CT.Lab3.BASIC.SpecificASTNodes;
using CT.Lab3.CommonCode;
using CT.Lab3.CommonCode.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.BASIC
{
    public class BASICParser
    {
        private Lexem[] lexems;
        int currentIndex = 0;
        ExpressionHelper expressionHelper = new ExpressionHelper(new BASICSyntaxProvider());

        public List<ASTNode> ParseLexemas(Lexem[] lexems)
        {
            this.lexems = lexems;

            List<ASTNode> nodes = new List<ASTNode>();
            while (currentIndex < lexems.Length)
            {
                nodes.Add(ParseNode());
            }

            return nodes;
        }

        private ASTNode ParseNode()
        {
            var currentLexem = lexems[currentIndex];

            switch (currentLexem.Type)
            {
                case LexemType.Keyword:
                    return ParseKeyword();
                default:
                    return ParseExpression();
            }
        }

        private ASTNode ParseKeyword()
        {
            var currentLexem = lexems[currentIndex++];

            if (currentLexem.Code.ToString() == "LET")
            {
                return ParseVariableInitializing();
            }
            else if (currentLexem.Code.ToString() == "INPUT")
            {
                return ParseInput();
            }
            else if (currentLexem.Code.ToString() == "PRINT")
            {
                return ParsePrint();
            }
            else if (currentLexem.Code.ToString() == "GOTO")
            {
                return ParseGoTo();
            }
            else if (currentLexem.Code.ToString() == "IF")
            {
                return ParseCondition();
            }
            else if (currentLexem.Code.ToString() == "END")
            {
                return new EndNode();
            }

            throw new Exception();
        }

        #region keyword parsing

        private VariableInitializationNode ParseVariableInitializing()
        {
            var variable = ParseIdentifier();
            SkipOperator("=");
            var assigningExpression = ParseExpression();
            SkipSemicolon();

            return new VariableInitializationNode(new string[] { variable.Code.ToString() }, "", new ExpressionNode[] { assigningExpression });
        }

        private VariableInputNode ParseInput()
        {
            string assigmentMessage = null;

            if (lexems[currentIndex].Type == LexemType.String)
            {
                assigmentMessage = lexems[currentIndex++].Code.ToString();
                SkipSemicolon();
            }

            var variable = ParseIdentifier();

            return new VariableInputNode(assigmentMessage, variable.Code.ToString());
        }

        private PrintNode ParsePrint()
        {
            string printingMessage = null;
            Lexem variable = null;

            if (lexems[currentIndex].Type == LexemType.String)
            {
                printingMessage = lexems[currentIndex++].Code.ToString();
                SkipSemicolon();
            }

            if (lexems[currentIndex].Type == LexemType.Identifier)
            {
                variable = ParseIdentifier();
            }

            return new PrintNode(printingMessage, variable?.Code?.ToString());
        }

        private GoToNode ParseGoTo()
        {
            var lineNumber = lexems[currentIndex++];

            if (lineNumber.Type != LexemType.Number)
            {
                throw new Exception();
            }

            return new GoToNode(int.Parse(lineNumber.Code.ToString()));
        }

        private ConditionNode ParseCondition()
        {
            var expression = ParseExpression();

            SkipThenKeyword();

            var thenNode = ParseNode();

            ASTNode elseNode = null;

            if (CheckLexem(LexemType.Keyword, "ELSE"))
            {
                SkipKeyword("ELSE");
                elseNode = ParseNode();
            }

            return new ConditionNode(expression, thenNode, elseNode);
        }

        #endregion

        private ExpressionNode ParseExpression()
        {
            var operandsList = new List<object>();
            ParseExpression(operandsList);
            var result = expressionHelper.ParseToExpressionTree(operandsList);

            return result;
        }

        private void ParseExpression(List<object> operandsList)
        {
            ExpressionNode expression;

            if (CheckLexem(LexemType.Punctuation, "("))
            {
                operandsList.Add(lexems[currentIndex]);
                SkipPunctuation("(", false);


                ParseExpression(operandsList);

                operandsList.Add(lexems[currentIndex]);
                SkipPunctuation(")", false);

                expression = null;
            }
            else if (CheckLexem(LexemType.Number))
            {
                expression = ParseLiteralExpression();
                operandsList.Add(expression);
            }
            else if (CheckLexem(LexemType.String))
            {
                expression = ParseLiteralExpression();
                operandsList.Add(expression);
            }
            else if (CheckLexem(LexemType.Identifier))
            {
                var identifier = ParseIdentifier();

                if (CheckLexem(LexemType.Punctuation, "("))
                {
                    expression = ParseMethodCall(identifier);
                }
                else
                {
                    var variable = ParseIdentifier(identifier);
                    expression = new Variable(variable.Code.ToString());
                }

                operandsList.Add(expression);
            }
            else
            {
                expression = null;
            }

            while (IsOperator(out Lexem operation))
            {
                operandsList.Add(operation);

                ParseExpression(operandsList);
            }
        }

        private LiteralExpression ParseLiteralExpression()
        {
            var literal = this.lexems[this.currentIndex++];
            return new LiteralExpression(literal.Code);
        }

        private MethodCallExpression ParseMethodCall(Lexem methodIdentifier)
        {
            var method = ParseIdentifier(methodIdentifier);
            SkipPunctuation("(", false);
            var parameters = new List<ExpressionNode>();

            while (!CheckLexem(LexemType.Punctuation, ")"))
            {
                parameters.Add(ParseExpression());

                if (!CheckLexem(LexemType.Punctuation, ")"))
                {
                    SkipPunctuation(",", false);
                }
            }

            SkipPunctuation(")", false);

            return new MethodCallExpression(method.Code.ToString(), parameters.ToArray());
        }

        private Lexem ParseIdentifier()
        {
            var variableIdentifier = lexems[currentIndex++];

            if (variableIdentifier.Type != LexemType.Identifier)
            {
                throw new Exception();
            }

            return ParseIdentifier(variableIdentifier);
        }

        private Lexem ParseIdentifier(Lexem identifier)
        {
            string variableName = identifier.Code.ToString();

            if (lexems[currentIndex].Code.ToString() == "$")
            {
                currentIndex++;
                variableName = variableName + "$";
            }

            return new Lexem(variableName) { Type = LexemType.Identifier };
        }

        #region auxiliary

        private void SkipThenKeyword()
        {
            SkipKeyword("THEN");
        }

        private void SkipKeyword(string keyword)
        {
            SkipLexem(LexemType.Keyword, keyword, false);
        }

        private void SkipOperator(string operatorName)
        {
            SkipLexem(LexemType.Operator, operatorName, false);
        }

        private void SkipSemicolon()
        {
            SkipPunctuation(";", false);
        }

        private void SkipPunctuation(string symbol, bool skipNext = true)
        {
            SkipLexem(LexemType.Punctuation, symbol, skipNext);
        }

        private void SkipLexem(LexemType lexemType, string code, bool skipNext = true)
        {
            if (skipNext)
            {
                currentIndex++;
            }

            if (CheckLexem(lexemType, code))
            {
                currentIndex++;
            }
            else
            {
                throw new Exception();
            }
        }

        private bool CheckLexem(LexemType lexemType, string code)
        {
            return CheckLexem(lexemType) && lexems[currentIndex].Code.ToString() == code;
        }

        private bool CheckLexem(LexemType lexemType)
        {
            return lexems[currentIndex].Type == lexemType;
        }

        private bool IsOperator(out Lexem currentOperation)
        {
            currentOperation = null;

            if (CheckLexem(LexemType.Keyword, "OR"))
            {
                currentOperation = new Lexem("OR") { Type = LexemType.Operator };
                currentIndex++;
            }
            else if (CheckLexem(LexemType.Operator))
            {
                currentOperation = lexems[currentIndex++];
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
