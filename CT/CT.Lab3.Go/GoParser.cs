using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3.Go
{
    public class GoParser
    {
        private Lexem[] lexems;
        int currentIndex = 0;

        private string[] basicTypes = new string[]
        {
            "int", "float", "string", "bool",
        };

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

            if (currentLexem.Code.ToString() == "func")
            {
                return ParseFunctionDeclaration();
            }
            else if (currentLexem.Code.ToString() == "type")
            {
                return ParseTypeDeclaration();
            }
            else if (currentLexem.Code.ToString() == "var")
            {
                return ParseVariableDeclaration();
            }

            throw new Exception();
        }

        #region expression parsing

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
                var identifier = this.lexems[this.currentIndex++];

                if (CheckLexem(LexemType.Punctuation, "("))
                {
                    expression = ParseMethodCall(identifier);
                }
                else
                {
                    expression = new Variable(identifier.Code.ToString());
                }
            }
            else
            {
                expression = null;
            }

            if (CheckLexem(LexemType.Operator, ":"))
            {
                SkipOperator(":");
                SkipOperator("=");

                if (!(expression is Variable))
                {
                    throw new Exception("Left operator of assigment must be a variable");
                }

                var rightExpression = ParseExpression();

                return new VariableAssigmentNode(expression as Variable, rightExpression);
            }
            else
            {
                //currentIndex++;
            }

            while (CheckLexem(LexemType.Operator))
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

            return new MethodCallExpression(methodIdentifier.Code.ToString(), parameters.ToArray());
        }

        #endregion

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
