using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdnLib
{
    // done skip comments
    // done skip line comments
    // done skip escapes
    // done split tag into name/value pair
    // done return data through events
    public class PdnParser
    {
        private int _currentPos;
        private bool _isOtherThanWhitespaceFound;
        private string _text;
        private int _lastRightBracketPos;
        private int _lastLeftBracketPos;
        private int _lastOpenQuotePos;
        private int _lastCloseQuotePos;

        public event EventHandler<EventArgs> OnStartNewGame;
        public event EventHandler<TagEventArgs> OnTagFound;
        public event EventHandler<BodyEventArgs> OnBodyFound;

        public void Parse(string text)
        {
            _text = text;
            _currentPos = -1;
            _isOtherThanWhitespaceFound = false;
            bool firstTag = true;
            while (_currentPos < _text.Length)
            {
                if (findTagOpen())
                {
                    if (_isOtherThanWhitespaceFound)
                    {
                        if (OnBodyFound != null)
                            OnBodyFound(this, new BodyEventArgs(){Value = _text.Substring(_lastRightBracketPos+1, _currentPos - _lastRightBracketPos-1).Trim()});
                        if (OnStartNewGame != null)
                            OnStartNewGame(this, new EventArgs());
                        _isOtherThanWhitespaceFound = false;
                    }

                    if (findTagClose())
                    {
                        if (firstTag)
                        {
                            if (OnStartNewGame != null)
                                OnStartNewGame(this, new EventArgs());
                            firstTag = false;
                        }

                        if (OnTagFound != null)
                            OnTagFound(this, new TagEventArgs() { Value = _text.Substring(_lastOpenQuotePos+1, _lastCloseQuotePos - _lastOpenQuotePos-1), Name = _text.Substring(_lastLeftBracketPos+1, _lastOpenQuotePos - _lastLeftBracketPos-1).Trim() });
                    }
                }
            }

            if (_isOtherThanWhitespaceFound)
            {
                if (OnBodyFound != null)
                    OnBodyFound(this, new BodyEventArgs() { Value = _text.Substring(_lastRightBracketPos+1, _currentPos - _lastRightBracketPos-1).Trim() });
            }

        }

        private bool findTagClose()
        {
            bool isInQuotes = false;
            while (true)
            {
                _currentPos++;
                if (_currentPos >= _text.Length)
                    return false;
                if (_text[_currentPos] == '"')
                {
                    if (!isInQuotes || _text[_currentPos - 1] != '\\')
                    {
                        isInQuotes = !isInQuotes;
                        if (isInQuotes)
                            _lastOpenQuotePos = _currentPos;
                        else
                            _lastCloseQuotePos = _currentPos;
                    }
                }
                else if (_text[_currentPos] == ']')
                {
                    if (!isInQuotes)
                    {
                        _lastRightBracketPos = _currentPos;
                        return true;
                    }
                }
            } 
        }

        private bool findTagOpen()
        {
            bool isBetweenCurlies = false;
            bool isInLineComment = false;
            while (true)
            {
                _currentPos++;
                if (_currentPos >= _text.Length)
                    return false;

                // find closing curlies otherwise skip
                if (isBetweenCurlies)
                {
                    if (_text[_currentPos] == '}')
                        isBetweenCurlies = false;
                }
                else if (isInLineComment)
                {
                    if (_text[_currentPos] == '\n' || _text[_currentPos] == '\r')
                        isInLineComment = false;
                }
                else
                {
                    if (_text[_currentPos] == '[')
                    {
                        _lastLeftBracketPos = _currentPos;
                        return true;
                    }
                    if (!isWhitespace(_text[_currentPos]))
                    {
                        _isOtherThanWhitespaceFound = true;
                        if (_text[_currentPos] == '{')
                        {
                            isBetweenCurlies = true;
                        }
                        else if (_text[_currentPos] == '%')
                        {
                            isInLineComment = true;
                        }
                    }
                }
            }
        }

        private bool isWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }
    }

    public class TagEventArgs : EventArgs 
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class BodyEventArgs : EventArgs
    {
        public string Value { get; set; }
    }
    
}
