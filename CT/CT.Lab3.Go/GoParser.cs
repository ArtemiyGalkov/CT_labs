using CT.Lab3.AST;
using CT.Lab3.CommonCode.AST;
using CT.Lab3.Go.SpecificASTNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CT.Lab3.Go
{
    public class GoParser
    {
        private Lexem[] lexems;
        int currentIndex = 0;
        bool insideProgramBlock = false;

        private string[] basicTypes = new string[]
        {
            "int", "float", "string", "bool",
        };

        private string[] typeKeywords = new string[]
        {
            "interface", "struct",
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
                    CheckIfInsideProgramBlock();
                    return ParseExpression();
            }
        }

        private ASTNode ParseKeyword()
        {
            var currentLexem = lexems[currentIndex++];

            if (currentLexem.Code.ToString() == "func")
            {
                return ParseMethodDeclaration();
            }
            else if (currentLexem.Code.ToString() == "type")
            {
                return ParseTypeDeclaration();
            }
            else if (currentLexem.Code.ToString() == "var")
            {
                return ParseVariableInitialization();
            }
            else if (currentLexem.Code.ToString() == "package")
            {
                return ParsePackage();
            }
            else if (currentLexem.Code.ToString() == "import")
            {
                return ParseImport();
            }
            else if (currentLexem.Code.ToString() == "if")
            {
                CheckIfInsideProgramBlock();
                return ParseCondition();
            }
            else if (currentLexem.Code.ToString() == "return")
            {
                return ParseReturn();
            }

            throw new Exception();
        }

        private ProgramBlockNode ParseProgramBlock()
        {
            var shouldChangeProgramBlockStatus = !insideProgramBlock;

            insideProgramBlock = true;
            SkipPunctuation("{", false);

            var result = new List<ASTNode>();

            while (!CheckLexem(LexemType.Punctuation, "}"))
            {
                result.Add(ParseNode());
            }

            if (shouldChangeProgramBlockStatus)
            {
                insideProgramBlock = false;
            }

            SkipPunctuation("}", false);

            return new ProgramBlockNode(result.ToArray());
        }

        private Lexem ParseComplexIdentifier()
        {
            var identifierCode = this.lexems[this.currentIndex++].Code.ToString();

            while(CheckLexem(LexemType.Punctuation, "."))
            {
                SkipPunctuation(".", false);
                identifierCode = identifierCode + "." + this.lexems[this.currentIndex++].Code.ToString();
            }

            return new Lexem(identifierCode) { Type = LexemType.Identifier };
        }

        #region keyword parsing

        private ReturnNode ParseReturn()
        {
            var expressionToReturn = ParseExpression();

            return new ReturnNode(expressionToReturn);
        }

        private ASTNode ParseCondition()
        {
            var expression = ParseExpression();

            var thenNode = ParseProgramBlock();

            ASTNode elseNode = null;

            if (CheckLexem(LexemType.Keyword, "else"))
            {
                SkipKeyword("else");

                if (CheckLexem(LexemType.Keyword, "if"))
                {
                    SkipKeyword("if");
                    elseNode = ParseCondition();
                }
                else
                {
                    elseNode = ParseProgramBlock();
                }
            }

            return new ConditionNode(expression, thenNode, elseNode);
        }

        private ImportNode ParseImport()
        {
            var importPath = lexems[currentIndex++];

            if (importPath.Type != LexemType.String)
            {
                throw new Exception();
            }

            return new ImportNode(importPath.Code.ToString());
        }

        private PackageNode ParsePackage()
        {
            return new PackageNode(GetIdentifier().Code.ToString());
        }

        private VariableInitializationNode ParseVariableInitialization()
        {
            var variableNames = new List<string>();

            var identifierNode = lexems[currentIndex++];
            variableNames.Add(identifierNode.Code.ToString());

            while (lexems[currentIndex].Code.ToString() == ",")
            {
                SkipPunctuation(",", false);
                identifierNode = lexems[currentIndex++];
                variableNames.Add(identifierNode.Code.ToString());
            }

            var typeNode = GetIdentifier();

            var variableValues = new List<ExpressionNode>();

            if (CheckLexem(LexemType.Operator, "="))
            {
                variableValues.Add(ParseExpression());

                while (lexems[currentIndex + 1].Code.ToString() == ",")
                {
                    SkipPunctuation(",");
                    variableValues.Add(ParseExpression());
                }
            }

            return new VariableInitializationNode(variableNames.ToArray(), typeNode.Code.ToString(), variableValues.ToArray());
        }

        private ASTNode ParseTypeDeclaration()
        {
            var identifier = GetIdentifier();
            var typeKeyword = lexems[currentIndex];
            var typeKeyowrdString = typeKeyword.Code.ToString();

            if (typeKeyword.Type != LexemType.Keyword || !typeKeywords.Contains(typeKeyowrdString))
            {
                throw new Exception("Invalid type declaration");
            }

            SkipPunctuation("{");

            switch (typeKeyowrdString)
            {
                case "interface":
                    var methodInterfaces = new List<MethodInterface>();

                    while (!CheckLexem(LexemType.Punctuation, "}"))
                    {
                        methodInterfaces.Add(ParseMethodInterface());
                    }

                    SkipPunctuation("}", false);

                    return new InterfaceDeclaration(identifier.Code.ToString(), methodInterfaces.ToArray());

                case "struct":
                    var variableInitializations = new List<VariableInitializationNode>();

                    while (!CheckLexem(LexemType.Punctuation, "}"))
                    {
                        variableInitializations.Add(ParseVariableInitialization());
                    }

                    SkipPunctuation("}", false);

                    return new StructDeclarationNode(identifier.Code.ToString(), variableInitializations.ToArray());

                default:
                    throw new Exception();
            }
        }

        private MethodInterface ParseMethodInterface()
        {
            var functionIdentifier = lexems[currentIndex];
            SkipPunctuation("(");

            var parameters = new List<Parameter>();

            while (!CheckLexem(LexemType.Punctuation, ")"))
            {
                parameters.Add(ParseParameter());

                if (!CheckLexem(LexemType.Punctuation, ")"))
                {
                    SkipPunctuation(",", false);
                }
            }

            SkipPunctuation(")", false);

            string returnType = null;
            var currentLexem = lexems[currentIndex];

            if (currentLexem.Type is LexemType.Identifier)
            {
                returnType = currentLexem.Code.ToString();
                currentIndex++;
            }

            return new MethodInterface(functionIdentifier.Code.ToString(), returnType, parameters.ToArray());
        }

        private MethodOfObjectDeclaration ParseMethodDeclaration()
        {
            (string objectName, string typeName)? objectToAppendTo = null;

            if (CheckLexem(LexemType.Punctuation, "("))
            {
                var objectName = GetIdentifier().Code.ToString();
                var objectType = GetIdentifier().Code.ToString();

                objectToAppendTo = (objectName, objectType);
                SkipPunctuation(")");
            }

            var methodInterface = ParseMethodInterface();

            var methodBody = ParseProgramBlock();

            return new MethodOfObjectDeclaration(methodInterface.MethodName, methodInterface.ReturnType, objectToAppendTo, methodInterface.Parameters, methodBody);
        }

        private Parameter ParseParameter()
        {
            var identifier = GetIdentifier();

            var type = lexems[currentIndex];
            string variableType = null;

            if(type.Type is LexemType.Identifier)
            {
                variableType = type.Code.ToString();
                currentIndex++;
            }

            return new Parameter(identifier.Code.ToString(), variableType);
        }

        #endregion

        #region expression parsing

        private ExpressionNode ParseExpression()
        {
            ExpressionNode expression;

            if (CheckLexem(LexemType.Punctuation, "("))
            {
                SkipPunctuation("(", false);
                expression = ParseExpression();
                SkipPunctuation(")", false);
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
                var identifier = GetIdentifier(true);

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

        private void CheckIfInsideProgramBlock()
        {
            if (!insideProgramBlock)
            {
                throw new Exception("Current statement allowed only inside method body");
            }
        }

        private Lexem GetIdentifier(bool allowComplexIdentifier = false)
        {
            var identifier = allowComplexIdentifier ? ParseComplexIdentifier() : lexems[currentIndex++];

            if (identifier.Type != LexemType.Identifier)
            {
                throw new Exception();
            }

            return identifier;
        }

        private void SkipKeyword(string keyword)
        {
            SkipLexem(LexemType.Keyword, keyword, false);
        }

        private void SkipOperator(string operatorName)
        {
            SkipLexem(LexemType.Operator, operatorName, false);
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
