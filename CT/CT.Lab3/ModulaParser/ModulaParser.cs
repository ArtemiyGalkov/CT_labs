using CT.Lab3.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CT.Lab3
{    
    public class ModulaParser
    {
        private Lexem[] lexems;
        private string[] basicTypes = new string[]
        {
            "INTEGER", "CARDINAL", "REAL", "LONGREAL", "CARDINAL", "BOOLEAN",
        };

        public ModulaParser(Lexem[] lexems)
        {
            this.lexems = lexems;
        }

        int currentIndex = 0;

        public List<ASTNode> ParseLexemas()
        {
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

            if (lexems[currentIndex].Type == LexemType.Identifier)
            {
                do
                {
                    var variableNames = new List<string>();

                    var identifierNode = lexems[currentIndex];
                    variableNames.Add(identifierNode.Code.ToString());

                    while (lexems[currentIndex + 1].Code.ToString() == ",")
                    {
                        SkipPunctuation(",");
                        identifierNode = lexems[currentIndex];
                        variableNames.Add(identifierNode.Code.ToString());
                    }

                    SkipOperator(":");
                    var typeNode = lexems[currentIndex];

                    if (!IsTypeLexem(typeNode))
                    {
                        throw new Exception();
                    }

                    var declaration = new VariableDeclarationNode(variableNames.ToArray(), typeNode.Code.ToString());

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

        private ProgramBlockNode ParseProgramBlock(bool methodBody = true)
        {
            var result = new List<ASTNode>();

            //while (!(lexems[++currentIndex].Type == LexemType.Keyword && lexems[currentIndex].Code.ToString() == "END"))
            //currentIndex++;

            while (!ProgramBlockEnd())
            {
                result.Add(ParseNode());
                SkipPunctuation(";", false);
            }

            if (methodBody)
            {
                SkipKeyword("END");

                if (lexems[currentIndex++].Type != LexemType.Identifier)
                {
                    throw new Exception();
                }

                if (!CheckLexem(LexemType.Punctuation, ";") && !CheckLexem(LexemType.Punctuation, "."))
                {
                    throw new Exception();
                }

                currentIndex++;
            }

            return new ProgramBlockNode(result.ToArray());
        }

        private ConditionNode ParseCondition(bool parsingElseIf = false)
        {
            var expression = ParseExpression();

            SkipThenKeyword(false);

            var thenNode = ParseProgramBlock(false);

            ASTNode elseNode = null;

            //while(lexems[++currentIndex].Type == LexemType.Keyword && lexems[currentIndex].Code.ToString() == "END")
            //currentIndex++;

            if (CheckLexem(LexemType.Keyword, "ELSIF"))
            {
                SkipKeyword("ELSIF");
                elseNode = ParseCondition(true);
            }
            else if (CheckLexem(LexemType.Keyword, "ELSE"))
            {
                SkipKeyword("ELSE");
                elseNode = ParseProgramBlock(false);
            }

            if (!parsingElseIf)
            {
                SkipEndKeyword(false);
            }

            //SkipSemicolon(false);

            return new ConditionNode(expression, thenNode, elseNode);
        }

        private ForLoopNode ParseForLoop()
        {
            throw new NotImplementedException();
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
            else
            {
                //currentIndex++;
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

        private void SkipThenKeyword(bool skipNext = true)
        {
            SkipKeyword("THEN");
        }

        private void SkipEndKeyword(bool skipNext = true)
        {
            SkipKeyword("END");
        }

        private void SkipKeyword(string keyword)
        {
            SkipLexem(LexemType.Keyword, keyword, false);
        }

        private void SkipOperator(string operatorName)
        {
            SkipLexem(LexemType.Operator, operatorName);
        }

        private void SkipSemicolon(bool skipNext = true)
        {
            SkipPunctuation(";", skipNext);
        }

        private void SkipComma()
        {
            SkipPunctuation(",");
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

        private bool IsTypeLexem(Lexem lexem)
        {
            return basicTypes.Contains(lexem.Code.ToString());
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
