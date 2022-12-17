using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace CT.Lab3
{
    public class ModulaParser
    {
        private Lexem[] lexems;
        int currentIndex = 0;

        public void ParseLexemas()
        {
            for (int currentIndex = 0; currentIndex < lexems.Length; currentIndex++)
            {
                ParseNode();
            }
        }

        private ASTNode ParseNode()
        {
            var currentLexem = lexems[currentIndex];

            switch (currentLexem.Type)
            {
                case LexemType.Keyword:
                    break;
            }
        }

        private ASTNode ParseKeyword()
        {
            var currentLexem = lexems[currentIndex++];

            if (currentLexem.Code.ToString() == "MODULE")
            {
                return ParseModuleDeclaration();
            }
            else if (currentLexem.Code.ToString() == "VAR")
            {
                return ParseVariablesSection();
            }
            else if (currentLexem.Code.ToString() == "BEGIN")
            {
                return ParseProgramBlock();
            }
            else if (currentLexem.Code.ToString() == "IF")
            {
                return ParseCondition();
            }
            else if (currentLexem.Code.ToString() == "FOR")
            {
                return ParseForLoop();
            }

            throw new Exception();
        }

        #region keyword parsing

        private ASTNode ParseModuleDeclaration()
        {
            var moduleIdentifier = lexems[currentIndex];
            if (moduleIdentifier.Type == LexemType.Identifier)
            {
                var node = new ModuleDeclarationNode(moduleIdentifier.Code.ToString());
                SkipSemicolon();
                return node;
            }

            throw new Exception();
        }

        private VariablesSectionNode ParseVariablesSection()
        {
            var declarations = new List<VariableDeclarationNode>();

            if (lexems[currentIndex + 1].Type == LexemType.Identifier)
            {
                do
                {
                    var identifierNode = lexems[++currentIndex];
                    SkipOperator(":");
                    var typeNode = lexems[currentIndex];

                    if (typeNode.Type != LexemType.Keyword || !IsTypeLexem(typeNode))
                    {
                        throw new Exception();
                    }

                    var declaration = new VariableDeclarationNode(identifierNode.Code.ToString(), typeNode.Code.ToString());

                    SkipSemicolon();

                    declarations.Add(declaration);

                } while (lexems[currentIndex].Type == LexemType.Identifier);
            }
            else if (!(lexems[++currentIndex].Type == LexemType.Punctuation && lexems[currentIndex].Code.ToString() == ";"))
            {
                throw new Exception();
            }

            return new VariablesSectionNode(declarations.ToArray());
        }

        private ProgramBlockNode ParseProgramBlock()
        {
            var result = new List<ASTNode>();

            //while (!(lexems[++currentIndex].Type == LexemType.Keyword && lexems[currentIndex].Code.ToString() == "END"))
            currentIndex++;

            while (!ProgramBlockEnd())
            {
                result.Add(ParseNode());
            }

            return new ProgramBlockNode(result.ToArray());
        }

        private ConditionNode ParseCondition()
        {
            var expression = ParseExpression();
            SkipThenKeyword();
            var thenNode = ParseProgramBlock();

            ASTNode elseNode = null;

            //while(lexems[++currentIndex].Type == LexemType.Keyword && lexems[currentIndex].Code.ToString() == "END")
            currentIndex++;

            if (CheckLexem(LexemType.Keyword, "ELSIF"))
            {
                SkipKeyword("ELSIF");
                elseNode = ParseCondition();
            }
            else if (CheckLexem(LexemType.Keyword, "ELSE"))
            {
                SkipKeyword("ELSE");
                elseNode = ParseProgramBlock();
            }

            SkipEndKeyword();
            SkipSemicolon();

            return new ConditionNode(expression, thenNode, elseNode);
        }

        private ForLoopNode ParseForLoop()
        {
        }

        #endregion

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
                    expression = new UnaryExpression(UnaryExpressionType.Variable, new IdentifierNode(identifier.Code.ToString()));
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

                var operation = new Lexem(":=");
            }

            if (CheckLexem(LexemType.Operator))
            {
                var operation = lexems[currentIndex++];
                var rightExpression = ParseExpression();

                if (!(expression is UnaryExpression unary && unary.Type is UnaryExpressionType.Variable))
                {
                    throw new Exception("Left operator of assigment must be a variable");
                }

                return new BinaryExpressionNode(expression, rightExpression, operation.Code);
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
            SkipPunctuation("(");
            var parameters = new List<ExpressionNode>();

            while (!CheckLexem(LexemType.Punctuation, ")"))
            {
                parameters.Add(ParseExpression());

                if (!CheckLexem(LexemType.Punctuation, ")"))
                {
                    SkipPunctuation(",");
                }
            }

            SkipPunctuation(")");

            return new MethodCallExpression(methodIdentifier.Code.ToString(), parameters.ToArray());
        }

        #endregion

        #region auxiliary

        private void SkipThenKeyword()
        {
            SkipKeyword("THEN");
        }

        private void SkipEndKeyword()
        {
            SkipKeyword("END");
        }

        private void SkipKeyword(string keyword)
        {
            //if (lexems[++currentIndex].Type == LexemType.Keyword && lexems[currentIndex].Code.ToString() == keyword)
            currentIndex++;

            if (CheckLexem(LexemType.Keyword, keyword))
            {
                currentIndex++;
            }
            else
            {
                throw new Exception();
            }
        }

        private void SkipOperator(string operatorName)
        {
            SkipLexem(LexemType.Operator, operatorName);
        }

        private void SkipSemicolon()
        {
            SkipPunctuation(";");
        }

        private void SkipComma()
        {
            SkipPunctuation(",");
        }

        private void SkipPunctuation(string symbol)
        {
            SkipLexem(LexemType.Punctuation, symbol);
        }

        private void SkipLexem(LexemType lexemType, string code)
        {
            //if (lexems[++currentIndex].Type == lexemType && lexems[currentIndex].Code.ToString() == code)
            currentIndex++;

            if (CheckLexem(lexemType, code))
            {
                currentIndex++;
            }
            else
            {
                throw new Exception();
            }
        }

        private bool IsTypeLexem(Lexem lexem)
        {
            throw new NotImplementedException();
        }

        private bool CheckLexem(LexemType lexemType, string code)
        {
            return CheckLexem(lexemType) && lexems[currentIndex].Code.ToString() == code;
        }

        private bool CheckLexem(LexemType lexemType)
        {
            return lexems[currentIndex].Type == lexemType;
        }

        private bool ProgramBlockEnd()
        {
            return CheckLexem(LexemType.Keyword, "END") || CheckLexem(LexemType.Keyword, "THEN") || CheckLexem(LexemType.Keyword, "ELSE") || CheckLexem(LexemType.Keyword, "ELSIF");
        }

        #endregion
    }
}
