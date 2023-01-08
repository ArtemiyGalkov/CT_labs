using CT.Lab3.AST;
using CT.Lab3.BASIC.SpecificASTNodes;
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

        private VariableInitializingNode ParseVariableInitializing()
        {
            var variable = ParseIdentifier();
            SkipOperator("=");
            var assigningExpression = ParseExpression();
            SkipSemicolon();

            return new VariableInitializingNode(new string[] { variable.Code.ToString() }, "", assigningExpression);
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
            ExpressionNode expression;

            if (CheckLexem(LexemType.Punctuation, "("))
            {
                SkipPunctuation("(");
                expression = ParseExpression();
                SkipPunctuation(")");
            }
            else if (CheckLexem(LexemType.Number))
            {
                expression = ParseLiteralExpression();
            }
            else if (CheckLexem(LexemType.String))
            {
                expression = ParseLiteralExpression();
            }
            else if (CheckLexem(LexemType.Identifier))
            {
                var identifier = ParseIdentifier();//this.lexems[this.currentIndex++];

                if (CheckLexem(LexemType.Punctuation, "("))
                {
                    expression = ParseMethodCall(identifier);
                }
                else
                {
                    var variable = ParseIdentifier(identifier);
                    expression = new Variable(variable.Code.ToString());
                }
            }
            else
            {
                expression = null;
            }

            while (CheckLexem(LexemType.Operator) || CheckLexem(LexemType.Keyword, "OR"))
            {
                var operation = lexems[currentIndex++];
                var rightExpression = ParseExpression();

                expression = new BinaryExpressionNode(expression, rightExpression, operation.Code);
            }

            return expression;
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

        #endregion
    }
}
