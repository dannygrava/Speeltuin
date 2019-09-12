grammar Pdn;
// Portable Draughts Notation grammar
options
{
	language='CSharp3';	
	k=1;
}
@header{
using System;
using System.Linq;
using System.Text;
using PdnDatabase;
}

@members {	
	public List<PdnGame> Games = new List<PdnGame>();
	private PdnGame _game;
}


public pdnFile 	:	game+;
game		
@init
{
	_game = new PdnGame();
	Games.Add(_game);	
}
:	gameHeader gameBody GAMERESULT;
gameHeader	:	pdnTag+;
gameBody	:	(gameMove | variation | comment | NAG)+;
comment		
@after
{
	_game.Body.Add(new PdnComment($comment.text));
}
:	(NESTED_ML_COMMENT | LINECOMMENT);
pdnTag          :	LBRACKET IDENTIFIER STRING RBRACKET 
{
	_game.Tags.Add (new PdnTag ($IDENTIFIER.text, $STRING.text));
}
;
gameMove        :	(MOVENUMBER? move moveStrength?)
{
	_game.Body.Add(new PdnMove($MOVENUMBER.text, $move.text, $moveStrength.text));
};
variation       :	LPAREN gameBody RPAREN;
move            :	numericMove | ALPHANUMERICMOVE | ELLIPSES;
moveStrength    :	MOVESTRENGTH1 | MOVESTRENGTH2;

GAMERESULT      :	('1-0'|'1/2-1/2'|'0-1'|'2-0'|'1-1'|'0-2'|'0-0'|'*') (WHITESPACE)+;
ELLIPSES:	'...';

MOVENUMBER  	
	:	INT+ DOT;

numericMove
	:	SQUARE (('-'|'x') SQUARE)+;
SQUARE	:	INT INT?;
ALPHANUMERICMOVE 
	:	('a'..'h')('1'..'8') (('-'|'x')('a'..'h')('1'..'8'))+;
MOVESTRENGTH1
	:	('!'|'?')+;
MOVESTRENGTH2
	:	 LPAREN MOVESTRENGTH1 RPAREN;

NAG	:	 '$' INT+;
LPAREN 	:	'(';
RPAREN	:	')';
LBRACKET: 	'[';
RBRACKET:	']';

STRING  :  '"' ( ~('\\'|'"') )* '"';
LINECOMMENT :   '%' ~('\n'|'\r')* '\r'? '\n';
//ML_COMMENT : '{' (options {greedy=false;} : .)* '}';
NESTED_ML_COMMENT	:	'{' (options {greedy=false;} : (NESTED_ML_COMMENT | .))* '}';
IDENTIFIER
	:	('A'..'Z') ('a'..'z'|'A'..'Z'| INT)*;


fragment
INT	:	'0'..'9';
fragment
DOT 	:	'.';

WHITESPACE
	:	(' ' | '\t'| '\r'| '\n') {$channel=Hidden;};