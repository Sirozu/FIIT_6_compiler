%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
BOOL TRUE|FALSE
ID {Alpha}{AlphaDigit}* 

%%

{INTNUM} { 
  yylval.iVal = int.Parse(yytext); 
  return (int)Tokens.INUM; 
}

{BOOL} {
    return (int)Tokens.BOOL;
}

{REALNUM} { 
  yylval.dVal = double.Parse(yytext); 
  return (int)Tokens.RNUM;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

"{" { return (int)Tokens.BEGIN; }
"}" { return (int)Tokens.END; }

"==" { return (int)Tokens.EQUAL; }
"!=" { return (int)Tokens.NEQUAL; }
">" { return (int)Tokens.GREATER; }
"<" { return (int)Tokens.LESS; }
"&&" { return (int)Tokens.AND; }
"||" { return (int)Tokens.OR; }
"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"*" { return (int)Tokens.MULT; }
"/" { return (int)Tokens.DIV; }
"%" { return (int)Tokens.MOD; }

"=" { return (int)Tokens.ASSIGN; }
";"  { return (int)Tokens.SEMICOLON; }
"," { return (int)Tokens.COMMA; }
"("  { return (int)Tokens.LPAR; }
")" { return (int)Tokens.RPAR; }




[^ \r\n\t] {
	LexError();
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%}

%%

public override void yyerror(string format, params object[] args) // ��������� �������������� ������
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): ��������� {2}, � ��������� {3}", yyline, yycol, args[0], string.Join(" ��� ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
  string errorMsg = string.Format("({0},{1}): ����������� ������ {2}", yyline, yycol, yytext);
  throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("while",(int)Tokens.WHILE);
	keywords.Add("for",(int)Tokens.FOR);
	keywords.Add("var",(int)Tokens.VAR);

	keywords.Add("if",(int)Tokens.IF);
	keywords.Add("else",(int)Tokens.ELSE);
	keywords.Add("print",(int)Tokens.PRINT);
	keywords.Add("input",(int)Tokens.INPUT);
	keywords.Add("goto",(int)Tokens.GOTO);



  }
  public static int GetIDToken(string s)
  {
	if (keywords.ContainsKey(s.ToLower()))
	  return keywords[s];
	else
      return (int)Tokens.ID;
  }
  
}
