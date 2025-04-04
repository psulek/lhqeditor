"use strict";
var LhqGenerators = (() => {
  var __create = Object.create;
  var __defProp = Object.defineProperty;
  var __defProps = Object.defineProperties;
  var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
  var __getOwnPropDescs = Object.getOwnPropertyDescriptors;
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __getOwnPropSymbols = Object.getOwnPropertySymbols;
  var __getProtoOf = Object.getPrototypeOf;
  var __hasOwnProp = Object.prototype.hasOwnProperty;
  var __propIsEnum = Object.prototype.propertyIsEnumerable;
  var __defNormalProp = (obj, key, value) => key in obj ? __defProp(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
  var __spreadValues = (a, b) => {
    for (var prop in b || (b = {}))
      if (__hasOwnProp.call(b, prop))
        __defNormalProp(a, prop, b[prop]);
    if (__getOwnPropSymbols)
      for (var prop of __getOwnPropSymbols(b)) {
        if (__propIsEnum.call(b, prop))
          __defNormalProp(a, prop, b[prop]);
      }
    return a;
  };
  var __spreadProps = (a, b) => __defProps(a, __getOwnPropDescs(b));
  var __objRest = (source, exclude) => {
    var target = {};
    for (var prop in source)
      if (__hasOwnProp.call(source, prop) && exclude.indexOf(prop) < 0)
        target[prop] = source[prop];
    if (source != null && __getOwnPropSymbols)
      for (var prop of __getOwnPropSymbols(source)) {
        if (exclude.indexOf(prop) < 0 && __propIsEnum.call(source, prop))
          target[prop] = source[prop];
      }
    return target;
  };
  var __commonJS = (cb, mod) => function __require() {
    return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
  };
  var __export = (target, all) => {
    for (var name2 in all)
      __defProp(target, name2, { get: all[name2], enumerable: true });
  };
  var __copyProps = (to, from, except, desc) => {
    if (from && typeof from === "object" || typeof from === "function") {
      for (let key of __getOwnPropNames(from))
        if (!__hasOwnProp.call(to, key) && key !== except)
          __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
    }
    return to;
  };
  var __toESM = (mod, isNodeMode, target) => (target = mod != null ? __create(__getProtoOf(mod)) : {}, __copyProps(
    // If the importer is in node compatibility mode or this is not an ESM
    // file that has been converted to a CommonJS file using a Babel-
    // compatible transform (i.e. "__esModule" has not been set), then set
    // "default" to the CommonJS "module.exports" for node compatibility.
    isNodeMode || !mod || !mod.__esModule ? __defProp(target, "default", { value: mod, enumerable: true }) : target,
    mod
  ));
  var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);
  var __publicField = (obj, key, value) => __defNormalProp(obj, typeof key !== "symbol" ? key + "" : key, value);
  var __async = (__this, __arguments, generator) => {
    return new Promise((resolve, reject) => {
      var fulfilled = (value) => {
        try {
          step(generator.next(value));
        } catch (e) {
          reject(e);
        }
      };
      var rejected = (value) => {
        try {
          step(generator.throw(value));
        } catch (e) {
          reject(e);
        }
      };
      var step = (x) => x.done ? resolve(x.value) : Promise.resolve(x.value).then(fulfilled, rejected);
      step((generator = generator.apply(__this, __arguments)).next());
    });
  };

  // node_modules/.pnpm/jmespath@0.16.0/node_modules/jmespath/jmespath.js
  var require_jmespath = __commonJS({
    "node_modules/.pnpm/jmespath@0.16.0/node_modules/jmespath/jmespath.js"(exports) {
      "use strict";
      (function(exports2) {
        "use strict";
        function isArray(obj) {
          if (obj !== null) {
            return Object.prototype.toString.call(obj) === "[object Array]";
          } else {
            return false;
          }
        }
        function isObject(obj) {
          if (obj !== null) {
            return Object.prototype.toString.call(obj) === "[object Object]";
          } else {
            return false;
          }
        }
        function strictDeepEqual(first, second) {
          if (first === second) {
            return true;
          }
          var firstType = Object.prototype.toString.call(first);
          if (firstType !== Object.prototype.toString.call(second)) {
            return false;
          }
          if (isArray(first) === true) {
            if (first.length !== second.length) {
              return false;
            }
            for (var i = 0; i < first.length; i++) {
              if (strictDeepEqual(first[i], second[i]) === false) {
                return false;
              }
            }
            return true;
          }
          if (isObject(first) === true) {
            var keysSeen = {};
            for (var key in first) {
              if (hasOwnProperty.call(first, key)) {
                if (strictDeepEqual(first[key], second[key]) === false) {
                  return false;
                }
                keysSeen[key] = true;
              }
            }
            for (var key2 in second) {
              if (hasOwnProperty.call(second, key2)) {
                if (keysSeen[key2] !== true) {
                  return false;
                }
              }
            }
            return true;
          }
          return false;
        }
        function isFalse(obj) {
          if (obj === "" || obj === false || obj === null) {
            return true;
          } else if (isArray(obj) && obj.length === 0) {
            return true;
          } else if (isObject(obj)) {
            for (var key in obj) {
              if (obj.hasOwnProperty(key)) {
                return false;
              }
            }
            return true;
          } else {
            return false;
          }
        }
        function objValues(obj) {
          var keys = Object.keys(obj);
          var values = [];
          for (var i = 0; i < keys.length; i++) {
            values.push(obj[keys[i]]);
          }
          return values;
        }
        function merge(a, b) {
          var merged = {};
          for (var key in a) {
            merged[key] = a[key];
          }
          for (var key2 in b) {
            merged[key2] = b[key2];
          }
          return merged;
        }
        var trimLeft;
        if (typeof String.prototype.trimLeft === "function") {
          trimLeft = function(str) {
            return str.trimLeft();
          };
        } else {
          trimLeft = function(str) {
            return str.match(/^\s*(.*)/)[1];
          };
        }
        var TYPE_NUMBER = 0;
        var TYPE_ANY = 1;
        var TYPE_STRING = 2;
        var TYPE_ARRAY = 3;
        var TYPE_OBJECT = 4;
        var TYPE_BOOLEAN = 5;
        var TYPE_EXPREF = 6;
        var TYPE_NULL = 7;
        var TYPE_ARRAY_NUMBER = 8;
        var TYPE_ARRAY_STRING = 9;
        var TYPE_NAME_TABLE = {
          0: "number",
          1: "any",
          2: "string",
          3: "array",
          4: "object",
          5: "boolean",
          6: "expression",
          7: "null",
          8: "Array<number>",
          9: "Array<string>"
        };
        var TOK_EOF = "EOF";
        var TOK_UNQUOTEDIDENTIFIER = "UnquotedIdentifier";
        var TOK_QUOTEDIDENTIFIER = "QuotedIdentifier";
        var TOK_RBRACKET = "Rbracket";
        var TOK_RPAREN = "Rparen";
        var TOK_COMMA = "Comma";
        var TOK_COLON = "Colon";
        var TOK_RBRACE = "Rbrace";
        var TOK_NUMBER = "Number";
        var TOK_CURRENT = "Current";
        var TOK_EXPREF = "Expref";
        var TOK_PIPE = "Pipe";
        var TOK_OR = "Or";
        var TOK_AND = "And";
        var TOK_EQ = "EQ";
        var TOK_GT = "GT";
        var TOK_LT = "LT";
        var TOK_GTE = "GTE";
        var TOK_LTE = "LTE";
        var TOK_NE = "NE";
        var TOK_FLATTEN = "Flatten";
        var TOK_STAR = "Star";
        var TOK_FILTER = "Filter";
        var TOK_DOT = "Dot";
        var TOK_NOT = "Not";
        var TOK_LBRACE = "Lbrace";
        var TOK_LBRACKET = "Lbracket";
        var TOK_LPAREN = "Lparen";
        var TOK_LITERAL = "Literal";
        var basicTokens = {
          ".": TOK_DOT,
          "*": TOK_STAR,
          ",": TOK_COMMA,
          ":": TOK_COLON,
          "{": TOK_LBRACE,
          "}": TOK_RBRACE,
          "]": TOK_RBRACKET,
          "(": TOK_LPAREN,
          ")": TOK_RPAREN,
          "@": TOK_CURRENT
        };
        var operatorStartToken = {
          "<": true,
          ">": true,
          "=": true,
          "!": true
        };
        var skipChars = {
          " ": true,
          "	": true,
          "\n": true
        };
        function isAlpha(ch) {
          return ch >= "a" && ch <= "z" || ch >= "A" && ch <= "Z" || ch === "_";
        }
        function isNum(ch) {
          return ch >= "0" && ch <= "9" || ch === "-";
        }
        function isAlphaNum(ch) {
          return ch >= "a" && ch <= "z" || ch >= "A" && ch <= "Z" || ch >= "0" && ch <= "9" || ch === "_";
        }
        function Lexer() {
        }
        Lexer.prototype = {
          tokenize: function(stream) {
            var tokens = [];
            this._current = 0;
            var start;
            var identifier;
            var token;
            while (this._current < stream.length) {
              if (isAlpha(stream[this._current])) {
                start = this._current;
                identifier = this._consumeUnquotedIdentifier(stream);
                tokens.push({
                  type: TOK_UNQUOTEDIDENTIFIER,
                  value: identifier,
                  start
                });
              } else if (basicTokens[stream[this._current]] !== void 0) {
                tokens.push({
                  type: basicTokens[stream[this._current]],
                  value: stream[this._current],
                  start: this._current
                });
                this._current++;
              } else if (isNum(stream[this._current])) {
                token = this._consumeNumber(stream);
                tokens.push(token);
              } else if (stream[this._current] === "[") {
                token = this._consumeLBracket(stream);
                tokens.push(token);
              } else if (stream[this._current] === '"') {
                start = this._current;
                identifier = this._consumeQuotedIdentifier(stream);
                tokens.push({
                  type: TOK_QUOTEDIDENTIFIER,
                  value: identifier,
                  start
                });
              } else if (stream[this._current] === "'") {
                start = this._current;
                identifier = this._consumeRawStringLiteral(stream);
                tokens.push({
                  type: TOK_LITERAL,
                  value: identifier,
                  start
                });
              } else if (stream[this._current] === "`") {
                start = this._current;
                var literal = this._consumeLiteral(stream);
                tokens.push({
                  type: TOK_LITERAL,
                  value: literal,
                  start
                });
              } else if (operatorStartToken[stream[this._current]] !== void 0) {
                tokens.push(this._consumeOperator(stream));
              } else if (skipChars[stream[this._current]] !== void 0) {
                this._current++;
              } else if (stream[this._current] === "&") {
                start = this._current;
                this._current++;
                if (stream[this._current] === "&") {
                  this._current++;
                  tokens.push({ type: TOK_AND, value: "&&", start });
                } else {
                  tokens.push({ type: TOK_EXPREF, value: "&", start });
                }
              } else if (stream[this._current] === "|") {
                start = this._current;
                this._current++;
                if (stream[this._current] === "|") {
                  this._current++;
                  tokens.push({ type: TOK_OR, value: "||", start });
                } else {
                  tokens.push({ type: TOK_PIPE, value: "|", start });
                }
              } else {
                var error = new Error("Unknown character:" + stream[this._current]);
                error.name = "LexerError";
                throw error;
              }
            }
            return tokens;
          },
          _consumeUnquotedIdentifier: function(stream) {
            var start = this._current;
            this._current++;
            while (this._current < stream.length && isAlphaNum(stream[this._current])) {
              this._current++;
            }
            return stream.slice(start, this._current);
          },
          _consumeQuotedIdentifier: function(stream) {
            var start = this._current;
            this._current++;
            var maxLength = stream.length;
            while (stream[this._current] !== '"' && this._current < maxLength) {
              var current = this._current;
              if (stream[current] === "\\" && (stream[current + 1] === "\\" || stream[current + 1] === '"')) {
                current += 2;
              } else {
                current++;
              }
              this._current = current;
            }
            this._current++;
            return JSON.parse(stream.slice(start, this._current));
          },
          _consumeRawStringLiteral: function(stream) {
            var start = this._current;
            this._current++;
            var maxLength = stream.length;
            while (stream[this._current] !== "'" && this._current < maxLength) {
              var current = this._current;
              if (stream[current] === "\\" && (stream[current + 1] === "\\" || stream[current + 1] === "'")) {
                current += 2;
              } else {
                current++;
              }
              this._current = current;
            }
            this._current++;
            var literal = stream.slice(start + 1, this._current - 1);
            return literal.replace("\\'", "'");
          },
          _consumeNumber: function(stream) {
            var start = this._current;
            this._current++;
            var maxLength = stream.length;
            while (isNum(stream[this._current]) && this._current < maxLength) {
              this._current++;
            }
            var value = parseInt(stream.slice(start, this._current));
            return { type: TOK_NUMBER, value, start };
          },
          _consumeLBracket: function(stream) {
            var start = this._current;
            this._current++;
            if (stream[this._current] === "?") {
              this._current++;
              return { type: TOK_FILTER, value: "[?", start };
            } else if (stream[this._current] === "]") {
              this._current++;
              return { type: TOK_FLATTEN, value: "[]", start };
            } else {
              return { type: TOK_LBRACKET, value: "[", start };
            }
          },
          _consumeOperator: function(stream) {
            var start = this._current;
            var startingChar = stream[start];
            this._current++;
            if (startingChar === "!") {
              if (stream[this._current] === "=") {
                this._current++;
                return { type: TOK_NE, value: "!=", start };
              } else {
                return { type: TOK_NOT, value: "!", start };
              }
            } else if (startingChar === "<") {
              if (stream[this._current] === "=") {
                this._current++;
                return { type: TOK_LTE, value: "<=", start };
              } else {
                return { type: TOK_LT, value: "<", start };
              }
            } else if (startingChar === ">") {
              if (stream[this._current] === "=") {
                this._current++;
                return { type: TOK_GTE, value: ">=", start };
              } else {
                return { type: TOK_GT, value: ">", start };
              }
            } else if (startingChar === "=") {
              if (stream[this._current] === "=") {
                this._current++;
                return { type: TOK_EQ, value: "==", start };
              }
            }
          },
          _consumeLiteral: function(stream) {
            this._current++;
            var start = this._current;
            var maxLength = stream.length;
            var literal;
            while (stream[this._current] !== "`" && this._current < maxLength) {
              var current = this._current;
              if (stream[current] === "\\" && (stream[current + 1] === "\\" || stream[current + 1] === "`")) {
                current += 2;
              } else {
                current++;
              }
              this._current = current;
            }
            var literalString = trimLeft(stream.slice(start, this._current));
            literalString = literalString.replace("\\`", "`");
            if (this._looksLikeJSON(literalString)) {
              literal = JSON.parse(literalString);
            } else {
              literal = JSON.parse('"' + literalString + '"');
            }
            this._current++;
            return literal;
          },
          _looksLikeJSON: function(literalString) {
            var startingChars = '[{"';
            var jsonLiterals = ["true", "false", "null"];
            var numberLooking = "-0123456789";
            if (literalString === "") {
              return false;
            } else if (startingChars.indexOf(literalString[0]) >= 0) {
              return true;
            } else if (jsonLiterals.indexOf(literalString) >= 0) {
              return true;
            } else if (numberLooking.indexOf(literalString[0]) >= 0) {
              try {
                JSON.parse(literalString);
                return true;
              } catch (ex) {
                return false;
              }
            } else {
              return false;
            }
          }
        };
        var bindingPower = {};
        bindingPower[TOK_EOF] = 0;
        bindingPower[TOK_UNQUOTEDIDENTIFIER] = 0;
        bindingPower[TOK_QUOTEDIDENTIFIER] = 0;
        bindingPower[TOK_RBRACKET] = 0;
        bindingPower[TOK_RPAREN] = 0;
        bindingPower[TOK_COMMA] = 0;
        bindingPower[TOK_RBRACE] = 0;
        bindingPower[TOK_NUMBER] = 0;
        bindingPower[TOK_CURRENT] = 0;
        bindingPower[TOK_EXPREF] = 0;
        bindingPower[TOK_PIPE] = 1;
        bindingPower[TOK_OR] = 2;
        bindingPower[TOK_AND] = 3;
        bindingPower[TOK_EQ] = 5;
        bindingPower[TOK_GT] = 5;
        bindingPower[TOK_LT] = 5;
        bindingPower[TOK_GTE] = 5;
        bindingPower[TOK_LTE] = 5;
        bindingPower[TOK_NE] = 5;
        bindingPower[TOK_FLATTEN] = 9;
        bindingPower[TOK_STAR] = 20;
        bindingPower[TOK_FILTER] = 21;
        bindingPower[TOK_DOT] = 40;
        bindingPower[TOK_NOT] = 45;
        bindingPower[TOK_LBRACE] = 50;
        bindingPower[TOK_LBRACKET] = 55;
        bindingPower[TOK_LPAREN] = 60;
        function Parser() {
        }
        Parser.prototype = {
          parse: function(expression) {
            this._loadTokens(expression);
            this.index = 0;
            var ast = this.expression(0);
            if (this._lookahead(0) !== TOK_EOF) {
              var t = this._lookaheadToken(0);
              var error = new Error(
                "Unexpected token type: " + t.type + ", value: " + t.value
              );
              error.name = "ParserError";
              throw error;
            }
            return ast;
          },
          _loadTokens: function(expression) {
            var lexer = new Lexer();
            var tokens = lexer.tokenize(expression);
            tokens.push({ type: TOK_EOF, value: "", start: expression.length });
            this.tokens = tokens;
          },
          expression: function(rbp) {
            var leftToken = this._lookaheadToken(0);
            this._advance();
            var left = this.nud(leftToken);
            var currentToken = this._lookahead(0);
            while (rbp < bindingPower[currentToken]) {
              this._advance();
              left = this.led(currentToken, left);
              currentToken = this._lookahead(0);
            }
            return left;
          },
          _lookahead: function(number) {
            return this.tokens[this.index + number].type;
          },
          _lookaheadToken: function(number) {
            return this.tokens[this.index + number];
          },
          _advance: function() {
            this.index++;
          },
          nud: function(token) {
            var left;
            var right;
            var expression;
            switch (token.type) {
              case TOK_LITERAL:
                return { type: "Literal", value: token.value };
              case TOK_UNQUOTEDIDENTIFIER:
                return { type: "Field", name: token.value };
              case TOK_QUOTEDIDENTIFIER:
                var node = { type: "Field", name: token.value };
                if (this._lookahead(0) === TOK_LPAREN) {
                  throw new Error("Quoted identifier not allowed for function names.");
                }
                return node;
              case TOK_NOT:
                right = this.expression(bindingPower.Not);
                return { type: "NotExpression", children: [right] };
              case TOK_STAR:
                left = { type: "Identity" };
                right = null;
                if (this._lookahead(0) === TOK_RBRACKET) {
                  right = { type: "Identity" };
                } else {
                  right = this._parseProjectionRHS(bindingPower.Star);
                }
                return { type: "ValueProjection", children: [left, right] };
              case TOK_FILTER:
                return this.led(token.type, { type: "Identity" });
              case TOK_LBRACE:
                return this._parseMultiselectHash();
              case TOK_FLATTEN:
                left = { type: TOK_FLATTEN, children: [{ type: "Identity" }] };
                right = this._parseProjectionRHS(bindingPower.Flatten);
                return { type: "Projection", children: [left, right] };
              case TOK_LBRACKET:
                if (this._lookahead(0) === TOK_NUMBER || this._lookahead(0) === TOK_COLON) {
                  right = this._parseIndexExpression();
                  return this._projectIfSlice({ type: "Identity" }, right);
                } else if (this._lookahead(0) === TOK_STAR && this._lookahead(1) === TOK_RBRACKET) {
                  this._advance();
                  this._advance();
                  right = this._parseProjectionRHS(bindingPower.Star);
                  return {
                    type: "Projection",
                    children: [{ type: "Identity" }, right]
                  };
                }
                return this._parseMultiselectList();
              case TOK_CURRENT:
                return { type: TOK_CURRENT };
              case TOK_EXPREF:
                expression = this.expression(bindingPower.Expref);
                return { type: "ExpressionReference", children: [expression] };
              case TOK_LPAREN:
                var args = [];
                while (this._lookahead(0) !== TOK_RPAREN) {
                  if (this._lookahead(0) === TOK_CURRENT) {
                    expression = { type: TOK_CURRENT };
                    this._advance();
                  } else {
                    expression = this.expression(0);
                  }
                  args.push(expression);
                }
                this._match(TOK_RPAREN);
                return args[0];
              default:
                this._errorToken(token);
            }
          },
          led: function(tokenName, left) {
            var right;
            switch (tokenName) {
              case TOK_DOT:
                var rbp = bindingPower.Dot;
                if (this._lookahead(0) !== TOK_STAR) {
                  right = this._parseDotRHS(rbp);
                  return { type: "Subexpression", children: [left, right] };
                }
                this._advance();
                right = this._parseProjectionRHS(rbp);
                return { type: "ValueProjection", children: [left, right] };
              case TOK_PIPE:
                right = this.expression(bindingPower.Pipe);
                return { type: TOK_PIPE, children: [left, right] };
              case TOK_OR:
                right = this.expression(bindingPower.Or);
                return { type: "OrExpression", children: [left, right] };
              case TOK_AND:
                right = this.expression(bindingPower.And);
                return { type: "AndExpression", children: [left, right] };
              case TOK_LPAREN:
                var name2 = left.name;
                var args = [];
                var expression, node;
                while (this._lookahead(0) !== TOK_RPAREN) {
                  if (this._lookahead(0) === TOK_CURRENT) {
                    expression = { type: TOK_CURRENT };
                    this._advance();
                  } else {
                    expression = this.expression(0);
                  }
                  if (this._lookahead(0) === TOK_COMMA) {
                    this._match(TOK_COMMA);
                  }
                  args.push(expression);
                }
                this._match(TOK_RPAREN);
                node = { type: "Function", name: name2, children: args };
                return node;
              case TOK_FILTER:
                var condition = this.expression(0);
                this._match(TOK_RBRACKET);
                if (this._lookahead(0) === TOK_FLATTEN) {
                  right = { type: "Identity" };
                } else {
                  right = this._parseProjectionRHS(bindingPower.Filter);
                }
                return { type: "FilterProjection", children: [left, right, condition] };
              case TOK_FLATTEN:
                var leftNode = { type: TOK_FLATTEN, children: [left] };
                var rightNode = this._parseProjectionRHS(bindingPower.Flatten);
                return { type: "Projection", children: [leftNode, rightNode] };
              case TOK_EQ:
              case TOK_NE:
              case TOK_GT:
              case TOK_GTE:
              case TOK_LT:
              case TOK_LTE:
                return this._parseComparator(left, tokenName);
              case TOK_LBRACKET:
                var token = this._lookaheadToken(0);
                if (token.type === TOK_NUMBER || token.type === TOK_COLON) {
                  right = this._parseIndexExpression();
                  return this._projectIfSlice(left, right);
                }
                this._match(TOK_STAR);
                this._match(TOK_RBRACKET);
                right = this._parseProjectionRHS(bindingPower.Star);
                return { type: "Projection", children: [left, right] };
              default:
                this._errorToken(this._lookaheadToken(0));
            }
          },
          _match: function(tokenType) {
            if (this._lookahead(0) === tokenType) {
              this._advance();
            } else {
              var t = this._lookaheadToken(0);
              var error = new Error("Expected " + tokenType + ", got: " + t.type);
              error.name = "ParserError";
              throw error;
            }
          },
          _errorToken: function(token) {
            var error = new Error("Invalid token (" + token.type + '): "' + token.value + '"');
            error.name = "ParserError";
            throw error;
          },
          _parseIndexExpression: function() {
            if (this._lookahead(0) === TOK_COLON || this._lookahead(1) === TOK_COLON) {
              return this._parseSliceExpression();
            } else {
              var node = {
                type: "Index",
                value: this._lookaheadToken(0).value
              };
              this._advance();
              this._match(TOK_RBRACKET);
              return node;
            }
          },
          _projectIfSlice: function(left, right) {
            var indexExpr = { type: "IndexExpression", children: [left, right] };
            if (right.type === "Slice") {
              return {
                type: "Projection",
                children: [indexExpr, this._parseProjectionRHS(bindingPower.Star)]
              };
            } else {
              return indexExpr;
            }
          },
          _parseSliceExpression: function() {
            var parts = [null, null, null];
            var index = 0;
            var currentToken = this._lookahead(0);
            while (currentToken !== TOK_RBRACKET && index < 3) {
              if (currentToken === TOK_COLON) {
                index++;
                this._advance();
              } else if (currentToken === TOK_NUMBER) {
                parts[index] = this._lookaheadToken(0).value;
                this._advance();
              } else {
                var t = this._lookahead(0);
                var error = new Error("Syntax error, unexpected token: " + t.value + "(" + t.type + ")");
                error.name = "Parsererror";
                throw error;
              }
              currentToken = this._lookahead(0);
            }
            this._match(TOK_RBRACKET);
            return {
              type: "Slice",
              children: parts
            };
          },
          _parseComparator: function(left, comparator) {
            var right = this.expression(bindingPower[comparator]);
            return { type: "Comparator", name: comparator, children: [left, right] };
          },
          _parseDotRHS: function(rbp) {
            var lookahead = this._lookahead(0);
            var exprTokens = [TOK_UNQUOTEDIDENTIFIER, TOK_QUOTEDIDENTIFIER, TOK_STAR];
            if (exprTokens.indexOf(lookahead) >= 0) {
              return this.expression(rbp);
            } else if (lookahead === TOK_LBRACKET) {
              this._match(TOK_LBRACKET);
              return this._parseMultiselectList();
            } else if (lookahead === TOK_LBRACE) {
              this._match(TOK_LBRACE);
              return this._parseMultiselectHash();
            }
          },
          _parseProjectionRHS: function(rbp) {
            var right;
            if (bindingPower[this._lookahead(0)] < 10) {
              right = { type: "Identity" };
            } else if (this._lookahead(0) === TOK_LBRACKET) {
              right = this.expression(rbp);
            } else if (this._lookahead(0) === TOK_FILTER) {
              right = this.expression(rbp);
            } else if (this._lookahead(0) === TOK_DOT) {
              this._match(TOK_DOT);
              right = this._parseDotRHS(rbp);
            } else {
              var t = this._lookaheadToken(0);
              var error = new Error("Sytanx error, unexpected token: " + t.value + "(" + t.type + ")");
              error.name = "ParserError";
              throw error;
            }
            return right;
          },
          _parseMultiselectList: function() {
            var expressions = [];
            while (this._lookahead(0) !== TOK_RBRACKET) {
              var expression = this.expression(0);
              expressions.push(expression);
              if (this._lookahead(0) === TOK_COMMA) {
                this._match(TOK_COMMA);
                if (this._lookahead(0) === TOK_RBRACKET) {
                  throw new Error("Unexpected token Rbracket");
                }
              }
            }
            this._match(TOK_RBRACKET);
            return { type: "MultiSelectList", children: expressions };
          },
          _parseMultiselectHash: function() {
            var pairs = [];
            var identifierTypes = [TOK_UNQUOTEDIDENTIFIER, TOK_QUOTEDIDENTIFIER];
            var keyToken, keyName, value, node;
            for (; ; ) {
              keyToken = this._lookaheadToken(0);
              if (identifierTypes.indexOf(keyToken.type) < 0) {
                throw new Error("Expecting an identifier token, got: " + keyToken.type);
              }
              keyName = keyToken.value;
              this._advance();
              this._match(TOK_COLON);
              value = this.expression(0);
              node = { type: "KeyValuePair", name: keyName, value };
              pairs.push(node);
              if (this._lookahead(0) === TOK_COMMA) {
                this._match(TOK_COMMA);
              } else if (this._lookahead(0) === TOK_RBRACE) {
                this._match(TOK_RBRACE);
                break;
              }
            }
            return { type: "MultiSelectHash", children: pairs };
          }
        };
        function TreeInterpreter(runtime) {
          this.runtime = runtime;
        }
        TreeInterpreter.prototype = {
          search: function(node, value) {
            return this.visit(node, value);
          },
          visit: function(node, value) {
            var matched, current, result, first, second, field, left, right, collected, i;
            switch (node.type) {
              case "Field":
                if (value !== null && isObject(value)) {
                  field = value[node.name];
                  if (field === void 0) {
                    return null;
                  } else {
                    return field;
                  }
                }
                return null;
              case "Subexpression":
                result = this.visit(node.children[0], value);
                for (i = 1; i < node.children.length; i++) {
                  result = this.visit(node.children[1], result);
                  if (result === null) {
                    return null;
                  }
                }
                return result;
              case "IndexExpression":
                left = this.visit(node.children[0], value);
                right = this.visit(node.children[1], left);
                return right;
              case "Index":
                if (!isArray(value)) {
                  return null;
                }
                var index = node.value;
                if (index < 0) {
                  index = value.length + index;
                }
                result = value[index];
                if (result === void 0) {
                  result = null;
                }
                return result;
              case "Slice":
                if (!isArray(value)) {
                  return null;
                }
                var sliceParams = node.children.slice(0);
                var computed = this.computeSliceParams(value.length, sliceParams);
                var start = computed[0];
                var stop = computed[1];
                var step = computed[2];
                result = [];
                if (step > 0) {
                  for (i = start; i < stop; i += step) {
                    result.push(value[i]);
                  }
                } else {
                  for (i = start; i > stop; i += step) {
                    result.push(value[i]);
                  }
                }
                return result;
              case "Projection":
                var base = this.visit(node.children[0], value);
                if (!isArray(base)) {
                  return null;
                }
                collected = [];
                for (i = 0; i < base.length; i++) {
                  current = this.visit(node.children[1], base[i]);
                  if (current !== null) {
                    collected.push(current);
                  }
                }
                return collected;
              case "ValueProjection":
                base = this.visit(node.children[0], value);
                if (!isObject(base)) {
                  return null;
                }
                collected = [];
                var values = objValues(base);
                for (i = 0; i < values.length; i++) {
                  current = this.visit(node.children[1], values[i]);
                  if (current !== null) {
                    collected.push(current);
                  }
                }
                return collected;
              case "FilterProjection":
                base = this.visit(node.children[0], value);
                if (!isArray(base)) {
                  return null;
                }
                var filtered = [];
                var finalResults = [];
                for (i = 0; i < base.length; i++) {
                  matched = this.visit(node.children[2], base[i]);
                  if (!isFalse(matched)) {
                    filtered.push(base[i]);
                  }
                }
                for (var j = 0; j < filtered.length; j++) {
                  current = this.visit(node.children[1], filtered[j]);
                  if (current !== null) {
                    finalResults.push(current);
                  }
                }
                return finalResults;
              case "Comparator":
                first = this.visit(node.children[0], value);
                second = this.visit(node.children[1], value);
                switch (node.name) {
                  case TOK_EQ:
                    result = strictDeepEqual(first, second);
                    break;
                  case TOK_NE:
                    result = !strictDeepEqual(first, second);
                    break;
                  case TOK_GT:
                    result = first > second;
                    break;
                  case TOK_GTE:
                    result = first >= second;
                    break;
                  case TOK_LT:
                    result = first < second;
                    break;
                  case TOK_LTE:
                    result = first <= second;
                    break;
                  default:
                    throw new Error("Unknown comparator: " + node.name);
                }
                return result;
              case TOK_FLATTEN:
                var original = this.visit(node.children[0], value);
                if (!isArray(original)) {
                  return null;
                }
                var merged = [];
                for (i = 0; i < original.length; i++) {
                  current = original[i];
                  if (isArray(current)) {
                    merged.push.apply(merged, current);
                  } else {
                    merged.push(current);
                  }
                }
                return merged;
              case "Identity":
                return value;
              case "MultiSelectList":
                if (value === null) {
                  return null;
                }
                collected = [];
                for (i = 0; i < node.children.length; i++) {
                  collected.push(this.visit(node.children[i], value));
                }
                return collected;
              case "MultiSelectHash":
                if (value === null) {
                  return null;
                }
                collected = {};
                var child;
                for (i = 0; i < node.children.length; i++) {
                  child = node.children[i];
                  collected[child.name] = this.visit(child.value, value);
                }
                return collected;
              case "OrExpression":
                matched = this.visit(node.children[0], value);
                if (isFalse(matched)) {
                  matched = this.visit(node.children[1], value);
                }
                return matched;
              case "AndExpression":
                first = this.visit(node.children[0], value);
                if (isFalse(first) === true) {
                  return first;
                }
                return this.visit(node.children[1], value);
              case "NotExpression":
                first = this.visit(node.children[0], value);
                return isFalse(first);
              case "Literal":
                return node.value;
              case TOK_PIPE:
                left = this.visit(node.children[0], value);
                return this.visit(node.children[1], left);
              case TOK_CURRENT:
                return value;
              case "Function":
                var resolvedArgs = [];
                for (i = 0; i < node.children.length; i++) {
                  resolvedArgs.push(this.visit(node.children[i], value));
                }
                return this.runtime.callFunction(node.name, resolvedArgs);
              case "ExpressionReference":
                var refNode = node.children[0];
                refNode.jmespathType = TOK_EXPREF;
                return refNode;
              default:
                throw new Error("Unknown node type: " + node.type);
            }
          },
          computeSliceParams: function(arrayLength, sliceParams) {
            var start = sliceParams[0];
            var stop = sliceParams[1];
            var step = sliceParams[2];
            var computed = [null, null, null];
            if (step === null) {
              step = 1;
            } else if (step === 0) {
              var error = new Error("Invalid slice, step cannot be 0");
              error.name = "RuntimeError";
              throw error;
            }
            var stepValueNegative = step < 0 ? true : false;
            if (start === null) {
              start = stepValueNegative ? arrayLength - 1 : 0;
            } else {
              start = this.capSliceRange(arrayLength, start, step);
            }
            if (stop === null) {
              stop = stepValueNegative ? -1 : arrayLength;
            } else {
              stop = this.capSliceRange(arrayLength, stop, step);
            }
            computed[0] = start;
            computed[1] = stop;
            computed[2] = step;
            return computed;
          },
          capSliceRange: function(arrayLength, actualValue, step) {
            if (actualValue < 0) {
              actualValue += arrayLength;
              if (actualValue < 0) {
                actualValue = step < 0 ? -1 : 0;
              }
            } else if (actualValue >= arrayLength) {
              actualValue = step < 0 ? arrayLength - 1 : arrayLength;
            }
            return actualValue;
          }
        };
        function Runtime(interpreter) {
          this._interpreter = interpreter;
          this.functionTable = {
            // name: [function, <signature>]
            // The <signature> can be:
            //
            // {
            //   args: [[type1, type2], [type1, type2]],
            //   variadic: true|false
            // }
            //
            // Each arg in the arg list is a list of valid types
            // (if the function is overloaded and supports multiple
            // types.  If the type is "any" then no type checking
            // occurs on the argument.  Variadic is optional
            // and if not provided is assumed to be false.
            abs: { _func: this._functionAbs, _signature: [{ types: [TYPE_NUMBER] }] },
            avg: { _func: this._functionAvg, _signature: [{ types: [TYPE_ARRAY_NUMBER] }] },
            ceil: { _func: this._functionCeil, _signature: [{ types: [TYPE_NUMBER] }] },
            contains: {
              _func: this._functionContains,
              _signature: [
                { types: [TYPE_STRING, TYPE_ARRAY] },
                { types: [TYPE_ANY] }
              ]
            },
            "ends_with": {
              _func: this._functionEndsWith,
              _signature: [{ types: [TYPE_STRING] }, { types: [TYPE_STRING] }]
            },
            floor: { _func: this._functionFloor, _signature: [{ types: [TYPE_NUMBER] }] },
            length: {
              _func: this._functionLength,
              _signature: [{ types: [TYPE_STRING, TYPE_ARRAY, TYPE_OBJECT] }]
            },
            map: {
              _func: this._functionMap,
              _signature: [{ types: [TYPE_EXPREF] }, { types: [TYPE_ARRAY] }]
            },
            max: {
              _func: this._functionMax,
              _signature: [{ types: [TYPE_ARRAY_NUMBER, TYPE_ARRAY_STRING] }]
            },
            "merge": {
              _func: this._functionMerge,
              _signature: [{ types: [TYPE_OBJECT], variadic: true }]
            },
            "max_by": {
              _func: this._functionMaxBy,
              _signature: [{ types: [TYPE_ARRAY] }, { types: [TYPE_EXPREF] }]
            },
            sum: { _func: this._functionSum, _signature: [{ types: [TYPE_ARRAY_NUMBER] }] },
            "starts_with": {
              _func: this._functionStartsWith,
              _signature: [{ types: [TYPE_STRING] }, { types: [TYPE_STRING] }]
            },
            min: {
              _func: this._functionMin,
              _signature: [{ types: [TYPE_ARRAY_NUMBER, TYPE_ARRAY_STRING] }]
            },
            "min_by": {
              _func: this._functionMinBy,
              _signature: [{ types: [TYPE_ARRAY] }, { types: [TYPE_EXPREF] }]
            },
            type: { _func: this._functionType, _signature: [{ types: [TYPE_ANY] }] },
            keys: { _func: this._functionKeys, _signature: [{ types: [TYPE_OBJECT] }] },
            values: { _func: this._functionValues, _signature: [{ types: [TYPE_OBJECT] }] },
            sort: { _func: this._functionSort, _signature: [{ types: [TYPE_ARRAY_STRING, TYPE_ARRAY_NUMBER] }] },
            "sort_by": {
              _func: this._functionSortBy,
              _signature: [{ types: [TYPE_ARRAY] }, { types: [TYPE_EXPREF] }]
            },
            join: {
              _func: this._functionJoin,
              _signature: [
                { types: [TYPE_STRING] },
                { types: [TYPE_ARRAY_STRING] }
              ]
            },
            reverse: {
              _func: this._functionReverse,
              _signature: [{ types: [TYPE_STRING, TYPE_ARRAY] }]
            },
            "to_array": { _func: this._functionToArray, _signature: [{ types: [TYPE_ANY] }] },
            "to_string": { _func: this._functionToString, _signature: [{ types: [TYPE_ANY] }] },
            "to_number": { _func: this._functionToNumber, _signature: [{ types: [TYPE_ANY] }] },
            "not_null": {
              _func: this._functionNotNull,
              _signature: [{ types: [TYPE_ANY], variadic: true }]
            }
          };
        }
        Runtime.prototype = {
          callFunction: function(name2, resolvedArgs) {
            var functionEntry = this.functionTable[name2];
            if (functionEntry === void 0) {
              throw new Error("Unknown function: " + name2 + "()");
            }
            this._validateArgs(name2, resolvedArgs, functionEntry._signature);
            return functionEntry._func.call(this, resolvedArgs);
          },
          _validateArgs: function(name2, args, signature) {
            var pluralized;
            if (signature[signature.length - 1].variadic) {
              if (args.length < signature.length) {
                pluralized = signature.length === 1 ? " argument" : " arguments";
                throw new Error("ArgumentError: " + name2 + "() takes at least" + signature.length + pluralized + " but received " + args.length);
              }
            } else if (args.length !== signature.length) {
              pluralized = signature.length === 1 ? " argument" : " arguments";
              throw new Error("ArgumentError: " + name2 + "() takes " + signature.length + pluralized + " but received " + args.length);
            }
            var currentSpec;
            var actualType;
            var typeMatched;
            for (var i = 0; i < signature.length; i++) {
              typeMatched = false;
              currentSpec = signature[i].types;
              actualType = this._getTypeName(args[i]);
              for (var j = 0; j < currentSpec.length; j++) {
                if (this._typeMatches(actualType, currentSpec[j], args[i])) {
                  typeMatched = true;
                  break;
                }
              }
              if (!typeMatched) {
                var expected = currentSpec.map(function(typeIdentifier) {
                  return TYPE_NAME_TABLE[typeIdentifier];
                }).join(",");
                throw new Error("TypeError: " + name2 + "() expected argument " + (i + 1) + " to be type " + expected + " but received type " + TYPE_NAME_TABLE[actualType] + " instead.");
              }
            }
          },
          _typeMatches: function(actual, expected, argValue) {
            if (expected === TYPE_ANY) {
              return true;
            }
            if (expected === TYPE_ARRAY_STRING || expected === TYPE_ARRAY_NUMBER || expected === TYPE_ARRAY) {
              if (expected === TYPE_ARRAY) {
                return actual === TYPE_ARRAY;
              } else if (actual === TYPE_ARRAY) {
                var subtype;
                if (expected === TYPE_ARRAY_NUMBER) {
                  subtype = TYPE_NUMBER;
                } else if (expected === TYPE_ARRAY_STRING) {
                  subtype = TYPE_STRING;
                }
                for (var i = 0; i < argValue.length; i++) {
                  if (!this._typeMatches(
                    this._getTypeName(argValue[i]),
                    subtype,
                    argValue[i]
                  )) {
                    return false;
                  }
                }
                return true;
              }
            } else {
              return actual === expected;
            }
          },
          _getTypeName: function(obj) {
            switch (Object.prototype.toString.call(obj)) {
              case "[object String]":
                return TYPE_STRING;
              case "[object Number]":
                return TYPE_NUMBER;
              case "[object Array]":
                return TYPE_ARRAY;
              case "[object Boolean]":
                return TYPE_BOOLEAN;
              case "[object Null]":
                return TYPE_NULL;
              case "[object Object]":
                if (obj.jmespathType === TOK_EXPREF) {
                  return TYPE_EXPREF;
                } else {
                  return TYPE_OBJECT;
                }
            }
          },
          _functionStartsWith: function(resolvedArgs) {
            return resolvedArgs[0].lastIndexOf(resolvedArgs[1]) === 0;
          },
          _functionEndsWith: function(resolvedArgs) {
            var searchStr = resolvedArgs[0];
            var suffix = resolvedArgs[1];
            return searchStr.indexOf(suffix, searchStr.length - suffix.length) !== -1;
          },
          _functionReverse: function(resolvedArgs) {
            var typeName = this._getTypeName(resolvedArgs[0]);
            if (typeName === TYPE_STRING) {
              var originalStr = resolvedArgs[0];
              var reversedStr = "";
              for (var i = originalStr.length - 1; i >= 0; i--) {
                reversedStr += originalStr[i];
              }
              return reversedStr;
            } else {
              var reversedArray = resolvedArgs[0].slice(0);
              reversedArray.reverse();
              return reversedArray;
            }
          },
          _functionAbs: function(resolvedArgs) {
            return Math.abs(resolvedArgs[0]);
          },
          _functionCeil: function(resolvedArgs) {
            return Math.ceil(resolvedArgs[0]);
          },
          _functionAvg: function(resolvedArgs) {
            var sum = 0;
            var inputArray = resolvedArgs[0];
            for (var i = 0; i < inputArray.length; i++) {
              sum += inputArray[i];
            }
            return sum / inputArray.length;
          },
          _functionContains: function(resolvedArgs) {
            return resolvedArgs[0].indexOf(resolvedArgs[1]) >= 0;
          },
          _functionFloor: function(resolvedArgs) {
            return Math.floor(resolvedArgs[0]);
          },
          _functionLength: function(resolvedArgs) {
            if (!isObject(resolvedArgs[0])) {
              return resolvedArgs[0].length;
            } else {
              return Object.keys(resolvedArgs[0]).length;
            }
          },
          _functionMap: function(resolvedArgs) {
            var mapped = [];
            var interpreter = this._interpreter;
            var exprefNode = resolvedArgs[0];
            var elements = resolvedArgs[1];
            for (var i = 0; i < elements.length; i++) {
              mapped.push(interpreter.visit(exprefNode, elements[i]));
            }
            return mapped;
          },
          _functionMerge: function(resolvedArgs) {
            var merged = {};
            for (var i = 0; i < resolvedArgs.length; i++) {
              var current = resolvedArgs[i];
              for (var key in current) {
                merged[key] = current[key];
              }
            }
            return merged;
          },
          _functionMax: function(resolvedArgs) {
            if (resolvedArgs[0].length > 0) {
              var typeName = this._getTypeName(resolvedArgs[0][0]);
              if (typeName === TYPE_NUMBER) {
                return Math.max.apply(Math, resolvedArgs[0]);
              } else {
                var elements = resolvedArgs[0];
                var maxElement = elements[0];
                for (var i = 1; i < elements.length; i++) {
                  if (maxElement.localeCompare(elements[i]) < 0) {
                    maxElement = elements[i];
                  }
                }
                return maxElement;
              }
            } else {
              return null;
            }
          },
          _functionMin: function(resolvedArgs) {
            if (resolvedArgs[0].length > 0) {
              var typeName = this._getTypeName(resolvedArgs[0][0]);
              if (typeName === TYPE_NUMBER) {
                return Math.min.apply(Math, resolvedArgs[0]);
              } else {
                var elements = resolvedArgs[0];
                var minElement = elements[0];
                for (var i = 1; i < elements.length; i++) {
                  if (elements[i].localeCompare(minElement) < 0) {
                    minElement = elements[i];
                  }
                }
                return minElement;
              }
            } else {
              return null;
            }
          },
          _functionSum: function(resolvedArgs) {
            var sum = 0;
            var listToSum = resolvedArgs[0];
            for (var i = 0; i < listToSum.length; i++) {
              sum += listToSum[i];
            }
            return sum;
          },
          _functionType: function(resolvedArgs) {
            switch (this._getTypeName(resolvedArgs[0])) {
              case TYPE_NUMBER:
                return "number";
              case TYPE_STRING:
                return "string";
              case TYPE_ARRAY:
                return "array";
              case TYPE_OBJECT:
                return "object";
              case TYPE_BOOLEAN:
                return "boolean";
              case TYPE_EXPREF:
                return "expref";
              case TYPE_NULL:
                return "null";
            }
          },
          _functionKeys: function(resolvedArgs) {
            return Object.keys(resolvedArgs[0]);
          },
          _functionValues: function(resolvedArgs) {
            var obj = resolvedArgs[0];
            var keys = Object.keys(obj);
            var values = [];
            for (var i = 0; i < keys.length; i++) {
              values.push(obj[keys[i]]);
            }
            return values;
          },
          _functionJoin: function(resolvedArgs) {
            var joinChar = resolvedArgs[0];
            var listJoin = resolvedArgs[1];
            return listJoin.join(joinChar);
          },
          _functionToArray: function(resolvedArgs) {
            if (this._getTypeName(resolvedArgs[0]) === TYPE_ARRAY) {
              return resolvedArgs[0];
            } else {
              return [resolvedArgs[0]];
            }
          },
          _functionToString: function(resolvedArgs) {
            if (this._getTypeName(resolvedArgs[0]) === TYPE_STRING) {
              return resolvedArgs[0];
            } else {
              return JSON.stringify(resolvedArgs[0]);
            }
          },
          _functionToNumber: function(resolvedArgs) {
            var typeName = this._getTypeName(resolvedArgs[0]);
            var convertedValue;
            if (typeName === TYPE_NUMBER) {
              return resolvedArgs[0];
            } else if (typeName === TYPE_STRING) {
              convertedValue = +resolvedArgs[0];
              if (!isNaN(convertedValue)) {
                return convertedValue;
              }
            }
            return null;
          },
          _functionNotNull: function(resolvedArgs) {
            for (var i = 0; i < resolvedArgs.length; i++) {
              if (this._getTypeName(resolvedArgs[i]) !== TYPE_NULL) {
                return resolvedArgs[i];
              }
            }
            return null;
          },
          _functionSort: function(resolvedArgs) {
            var sortedArray = resolvedArgs[0].slice(0);
            sortedArray.sort();
            return sortedArray;
          },
          _functionSortBy: function(resolvedArgs) {
            var sortedArray = resolvedArgs[0].slice(0);
            if (sortedArray.length === 0) {
              return sortedArray;
            }
            var interpreter = this._interpreter;
            var exprefNode = resolvedArgs[1];
            var requiredType = this._getTypeName(
              interpreter.visit(exprefNode, sortedArray[0])
            );
            if ([TYPE_NUMBER, TYPE_STRING].indexOf(requiredType) < 0) {
              throw new Error("TypeError");
            }
            var that = this;
            var decorated = [];
            for (var i = 0; i < sortedArray.length; i++) {
              decorated.push([i, sortedArray[i]]);
            }
            decorated.sort(function(a, b) {
              var exprA = interpreter.visit(exprefNode, a[1]);
              var exprB = interpreter.visit(exprefNode, b[1]);
              if (that._getTypeName(exprA) !== requiredType) {
                throw new Error(
                  "TypeError: expected " + requiredType + ", received " + that._getTypeName(exprA)
                );
              } else if (that._getTypeName(exprB) !== requiredType) {
                throw new Error(
                  "TypeError: expected " + requiredType + ", received " + that._getTypeName(exprB)
                );
              }
              if (exprA > exprB) {
                return 1;
              } else if (exprA < exprB) {
                return -1;
              } else {
                return a[0] - b[0];
              }
            });
            for (var j = 0; j < decorated.length; j++) {
              sortedArray[j] = decorated[j][1];
            }
            return sortedArray;
          },
          _functionMaxBy: function(resolvedArgs) {
            var exprefNode = resolvedArgs[1];
            var resolvedArray = resolvedArgs[0];
            var keyFunction = this.createKeyFunction(exprefNode, [TYPE_NUMBER, TYPE_STRING]);
            var maxNumber = -Infinity;
            var maxRecord;
            var current;
            for (var i = 0; i < resolvedArray.length; i++) {
              current = keyFunction(resolvedArray[i]);
              if (current > maxNumber) {
                maxNumber = current;
                maxRecord = resolvedArray[i];
              }
            }
            return maxRecord;
          },
          _functionMinBy: function(resolvedArgs) {
            var exprefNode = resolvedArgs[1];
            var resolvedArray = resolvedArgs[0];
            var keyFunction = this.createKeyFunction(exprefNode, [TYPE_NUMBER, TYPE_STRING]);
            var minNumber = Infinity;
            var minRecord;
            var current;
            for (var i = 0; i < resolvedArray.length; i++) {
              current = keyFunction(resolvedArray[i]);
              if (current < minNumber) {
                minNumber = current;
                minRecord = resolvedArray[i];
              }
            }
            return minRecord;
          },
          createKeyFunction: function(exprefNode, allowedTypes) {
            var that = this;
            var interpreter = this._interpreter;
            var keyFunc = function(x) {
              var current = interpreter.visit(exprefNode, x);
              if (allowedTypes.indexOf(that._getTypeName(current)) < 0) {
                var msg = "TypeError: expected one of " + allowedTypes + ", received " + that._getTypeName(current);
                throw new Error(msg);
              }
              return current;
            };
            return keyFunc;
          }
        };
        function compile(stream) {
          var parser = new Parser();
          var ast = parser.parse(stream);
          return ast;
        }
        function tokenize(stream) {
          var lexer = new Lexer();
          return lexer.tokenize(stream);
        }
        function search(data, expression) {
          var parser = new Parser();
          var runtime = new Runtime();
          var interpreter = new TreeInterpreter(runtime);
          runtime._interpreter = interpreter;
          var node = parser.parse(expression);
          return interpreter.search(node, data);
        }
        exports2.tokenize = tokenize;
        exports2.compile = compile;
        exports2.search = search;
        exports2.strictDeepEqual = strictDeepEqual;
      })(typeof exports === "undefined" ? exports.jmespath = {} : exports);
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/utils.js
  var require_utils = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/utils.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.extend = extend;
      exports.indexOf = indexOf;
      exports.escapeExpression = escapeExpression;
      exports.isEmpty = isEmpty;
      exports.createFrame = createFrame;
      exports.blockParams = blockParams;
      exports.appendContextPath = appendContextPath;
      var escape = {
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': "&quot;",
        "'": "&#x27;",
        "`": "&#x60;",
        "=": "&#x3D;"
      };
      var badChars = /[&<>"'`=]/g;
      var possible = /[&<>"'`=]/;
      function escapeChar(chr) {
        return escape[chr];
      }
      function extend(obj) {
        for (var i = 1; i < arguments.length; i++) {
          for (var key in arguments[i]) {
            if (Object.prototype.hasOwnProperty.call(arguments[i], key)) {
              obj[key] = arguments[i][key];
            }
          }
        }
        return obj;
      }
      var toString = Object.prototype.toString;
      exports.toString = toString;
      var isFunction = function isFunction2(value) {
        return typeof value === "function";
      };
      if (isFunction(/x/)) {
        exports.isFunction = isFunction = function(value) {
          return typeof value === "function" && toString.call(value) === "[object Function]";
        };
      }
      exports.isFunction = isFunction;
      var isArray = Array.isArray || function(value) {
        return value && typeof value === "object" ? toString.call(value) === "[object Array]" : false;
      };
      exports.isArray = isArray;
      function indexOf(array, value) {
        for (var i = 0, len = array.length; i < len; i++) {
          if (array[i] === value) {
            return i;
          }
        }
        return -1;
      }
      function escapeExpression(string) {
        if (typeof string !== "string") {
          if (string && string.toHTML) {
            return string.toHTML();
          } else if (string == null) {
            return "";
          } else if (!string) {
            return string + "";
          }
          string = "" + string;
        }
        if (!possible.test(string)) {
          return string;
        }
        return string.replace(badChars, escapeChar);
      }
      function isEmpty(value) {
        if (!value && value !== 0) {
          return true;
        } else if (isArray(value) && value.length === 0) {
          return true;
        } else {
          return false;
        }
      }
      function createFrame(object) {
        var frame = extend({}, object);
        frame._parent = object;
        return frame;
      }
      function blockParams(params, ids) {
        params.path = ids;
        return params;
      }
      function appendContextPath(contextPath, id) {
        return (contextPath ? contextPath + "." : "") + id;
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/exception.js
  var require_exception = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/exception.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var errorProps = ["description", "fileName", "lineNumber", "endLineNumber", "message", "name", "number", "stack"];
      function Exception(message, node) {
        var loc = node && node.loc, line = void 0, endLineNumber = void 0, column = void 0, endColumn = void 0;
        if (loc) {
          line = loc.start.line;
          endLineNumber = loc.end.line;
          column = loc.start.column;
          endColumn = loc.end.column;
          message += " - " + line + ":" + column;
        }
        var tmp = Error.prototype.constructor.call(this, message);
        for (var idx = 0; idx < errorProps.length; idx++) {
          this[errorProps[idx]] = tmp[errorProps[idx]];
        }
        if (Error.captureStackTrace) {
          Error.captureStackTrace(this, Exception);
        }
        try {
          if (loc) {
            this.lineNumber = line;
            this.endLineNumber = endLineNumber;
            if (Object.defineProperty) {
              Object.defineProperty(this, "column", {
                value: column,
                enumerable: true
              });
              Object.defineProperty(this, "endColumn", {
                value: endColumn,
                enumerable: true
              });
            } else {
              this.column = column;
              this.endColumn = endColumn;
            }
          }
        } catch (nop) {
        }
      }
      Exception.prototype = new Error();
      exports["default"] = Exception;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/block-helper-missing.js
  var require_block_helper_missing = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/block-helper-missing.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var _utils = require_utils();
      exports["default"] = function(instance) {
        instance.registerHelper("blockHelperMissing", function(context, options) {
          var inverse = options.inverse, fn2 = options.fn;
          if (context === true) {
            return fn2(this);
          } else if (context === false || context == null) {
            return inverse(this);
          } else if (_utils.isArray(context)) {
            if (context.length > 0) {
              if (options.ids) {
                options.ids = [options.name];
              }
              return instance.helpers.each(context, options);
            } else {
              return inverse(this);
            }
          } else {
            if (options.data && options.ids) {
              var data = _utils.createFrame(options.data);
              data.contextPath = _utils.appendContextPath(options.data.contextPath, options.name);
              options = { data };
            }
            return fn2(context, options);
          }
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/each.js
  var require_each = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/each.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _utils = require_utils();
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      exports["default"] = function(instance) {
        instance.registerHelper("each", function(context, options) {
          if (!options) {
            throw new _exception2["default"]("Must pass iterator to #each");
          }
          var fn2 = options.fn, inverse = options.inverse, i = 0, ret = "", data = void 0, contextPath = void 0;
          if (options.data && options.ids) {
            contextPath = _utils.appendContextPath(options.data.contextPath, options.ids[0]) + ".";
          }
          if (_utils.isFunction(context)) {
            context = context.call(this);
          }
          if (options.data) {
            data = _utils.createFrame(options.data);
          }
          function execIteration(field, index, last) {
            if (data) {
              data.key = field;
              data.index = index;
              data.first = index === 0;
              data.last = !!last;
              if (contextPath) {
                data.contextPath = contextPath + field;
              }
            }
            ret = ret + fn2(context[field], {
              data,
              blockParams: _utils.blockParams([context[field], field], [contextPath + field, null])
            });
          }
          if (context && typeof context === "object") {
            if (_utils.isArray(context)) {
              for (var j = context.length; i < j; i++) {
                if (i in context) {
                  execIteration(i, i, i === context.length - 1);
                }
              }
            } else if (typeof Symbol === "function" && context[Symbol.iterator]) {
              var newContext = [];
              var iterator = context[Symbol.iterator]();
              for (var it = iterator.next(); !it.done; it = iterator.next()) {
                newContext.push(it.value);
              }
              context = newContext;
              for (var j = context.length; i < j; i++) {
                execIteration(i, i, i === context.length - 1);
              }
            } else {
              (function() {
                var priorKey = void 0;
                Object.keys(context).forEach(function(key) {
                  if (priorKey !== void 0) {
                    execIteration(priorKey, i - 1);
                  }
                  priorKey = key;
                  i++;
                });
                if (priorKey !== void 0) {
                  execIteration(priorKey, i - 1, true);
                }
              })();
            }
          }
          if (i === 0) {
            ret = inverse(this);
          }
          return ret;
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/helper-missing.js
  var require_helper_missing = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/helper-missing.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      exports["default"] = function(instance) {
        instance.registerHelper("helperMissing", function() {
          if (arguments.length === 1) {
            return void 0;
          } else {
            throw new _exception2["default"]('Missing helper: "' + arguments[arguments.length - 1].name + '"');
          }
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/if.js
  var require_if = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/if.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _utils = require_utils();
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      exports["default"] = function(instance) {
        instance.registerHelper("if", function(conditional, options) {
          if (arguments.length != 2) {
            throw new _exception2["default"]("#if requires exactly one argument");
          }
          if (_utils.isFunction(conditional)) {
            conditional = conditional.call(this);
          }
          if (!options.hash.includeZero && !conditional || _utils.isEmpty(conditional)) {
            return options.inverse(this);
          } else {
            return options.fn(this);
          }
        });
        instance.registerHelper("unless", function(conditional, options) {
          if (arguments.length != 2) {
            throw new _exception2["default"]("#unless requires exactly one argument");
          }
          return instance.helpers["if"].call(this, conditional, {
            fn: options.inverse,
            inverse: options.fn,
            hash: options.hash
          });
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/log.js
  var require_log = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/log.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      exports["default"] = function(instance) {
        instance.registerHelper("log", function() {
          var args = [void 0], options = arguments[arguments.length - 1];
          for (var i = 0; i < arguments.length - 1; i++) {
            args.push(arguments[i]);
          }
          var level = 1;
          if (options.hash.level != null) {
            level = options.hash.level;
          } else if (options.data && options.data.level != null) {
            level = options.data.level;
          }
          args[0] = level;
          instance.log.apply(instance, args);
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/lookup.js
  var require_lookup = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/lookup.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      exports["default"] = function(instance) {
        instance.registerHelper("lookup", function(obj, field, options) {
          if (!obj) {
            return obj;
          }
          return options.lookupProperty(obj, field);
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/with.js
  var require_with = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers/with.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _utils = require_utils();
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      exports["default"] = function(instance) {
        instance.registerHelper("with", function(context, options) {
          if (arguments.length != 2) {
            throw new _exception2["default"]("#with requires exactly one argument");
          }
          if (_utils.isFunction(context)) {
            context = context.call(this);
          }
          var fn2 = options.fn;
          if (!_utils.isEmpty(context)) {
            var data = options.data;
            if (options.data && options.ids) {
              data = _utils.createFrame(options.data);
              data.contextPath = _utils.appendContextPath(options.data.contextPath, options.ids[0]);
            }
            return fn2(context, {
              data,
              blockParams: _utils.blockParams([context], [data && data.contextPath])
            });
          } else {
            return options.inverse(this);
          }
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers.js
  var require_helpers = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/helpers.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.registerDefaultHelpers = registerDefaultHelpers;
      exports.moveHelperToHooks = moveHelperToHooks;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _helpersBlockHelperMissing = require_block_helper_missing();
      var _helpersBlockHelperMissing2 = _interopRequireDefault(_helpersBlockHelperMissing);
      var _helpersEach = require_each();
      var _helpersEach2 = _interopRequireDefault(_helpersEach);
      var _helpersHelperMissing = require_helper_missing();
      var _helpersHelperMissing2 = _interopRequireDefault(_helpersHelperMissing);
      var _helpersIf = require_if();
      var _helpersIf2 = _interopRequireDefault(_helpersIf);
      var _helpersLog = require_log();
      var _helpersLog2 = _interopRequireDefault(_helpersLog);
      var _helpersLookup = require_lookup();
      var _helpersLookup2 = _interopRequireDefault(_helpersLookup);
      var _helpersWith = require_with();
      var _helpersWith2 = _interopRequireDefault(_helpersWith);
      function registerDefaultHelpers(instance) {
        _helpersBlockHelperMissing2["default"](instance);
        _helpersEach2["default"](instance);
        _helpersHelperMissing2["default"](instance);
        _helpersIf2["default"](instance);
        _helpersLog2["default"](instance);
        _helpersLookup2["default"](instance);
        _helpersWith2["default"](instance);
      }
      function moveHelperToHooks(instance, helperName, keepHelper) {
        if (instance.helpers[helperName]) {
          instance.hooks[helperName] = instance.helpers[helperName];
          if (!keepHelper) {
            delete instance.helpers[helperName];
          }
        }
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/decorators/inline.js
  var require_inline = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/decorators/inline.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var _utils = require_utils();
      exports["default"] = function(instance) {
        instance.registerDecorator("inline", function(fn2, props, container, options) {
          var ret = fn2;
          if (!props.partials) {
            props.partials = {};
            ret = function(context, options2) {
              var original = container.partials;
              container.partials = _utils.extend({}, original, props.partials);
              var ret2 = fn2(context, options2);
              container.partials = original;
              return ret2;
            };
          }
          props.partials[options.args[0]] = options.fn;
          return ret;
        });
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/decorators.js
  var require_decorators = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/decorators.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.registerDefaultDecorators = registerDefaultDecorators;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _decoratorsInline = require_inline();
      var _decoratorsInline2 = _interopRequireDefault(_decoratorsInline);
      function registerDefaultDecorators(instance) {
        _decoratorsInline2["default"](instance);
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/logger.js
  var require_logger = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/logger.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var _utils = require_utils();
      var logger = {
        methodMap: ["debug", "info", "warn", "error"],
        level: "info",
        // Maps a given level value to the `methodMap` indexes above.
        lookupLevel: function lookupLevel(level) {
          if (typeof level === "string") {
            var levelMap = _utils.indexOf(logger.methodMap, level.toLowerCase());
            if (levelMap >= 0) {
              level = levelMap;
            } else {
              level = parseInt(level, 10);
            }
          }
          return level;
        },
        // Can be overridden in the host environment
        log: function log(level) {
          level = logger.lookupLevel(level);
          if (typeof console !== "undefined" && logger.lookupLevel(logger.level) <= level) {
            var method = logger.methodMap[level];
            if (!console[method]) {
              method = "log";
            }
            for (var _len = arguments.length, message = Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
              message[_key - 1] = arguments[_key];
            }
            console[method].apply(console, message);
          }
        }
      };
      exports["default"] = logger;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/internal/create-new-lookup-object.js
  var require_create_new_lookup_object = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/internal/create-new-lookup-object.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.createNewLookupObject = createNewLookupObject;
      var _utils = require_utils();
      function createNewLookupObject() {
        for (var _len = arguments.length, sources = Array(_len), _key = 0; _key < _len; _key++) {
          sources[_key] = arguments[_key];
        }
        return _utils.extend.apply(void 0, [/* @__PURE__ */ Object.create(null)].concat(sources));
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/internal/proto-access.js
  var require_proto_access = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/internal/proto-access.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.createProtoAccessControl = createProtoAccessControl;
      exports.resultIsAllowed = resultIsAllowed;
      exports.resetLoggedProperties = resetLoggedProperties;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _createNewLookupObject = require_create_new_lookup_object();
      var _logger = require_logger();
      var _logger2 = _interopRequireDefault(_logger);
      var loggedProperties = /* @__PURE__ */ Object.create(null);
      function createProtoAccessControl(runtimeOptions) {
        var defaultMethodWhiteList = /* @__PURE__ */ Object.create(null);
        defaultMethodWhiteList["constructor"] = false;
        defaultMethodWhiteList["__defineGetter__"] = false;
        defaultMethodWhiteList["__defineSetter__"] = false;
        defaultMethodWhiteList["__lookupGetter__"] = false;
        var defaultPropertyWhiteList = /* @__PURE__ */ Object.create(null);
        defaultPropertyWhiteList["__proto__"] = false;
        return {
          properties: {
            whitelist: _createNewLookupObject.createNewLookupObject(defaultPropertyWhiteList, runtimeOptions.allowedProtoProperties),
            defaultValue: runtimeOptions.allowProtoPropertiesByDefault
          },
          methods: {
            whitelist: _createNewLookupObject.createNewLookupObject(defaultMethodWhiteList, runtimeOptions.allowedProtoMethods),
            defaultValue: runtimeOptions.allowProtoMethodsByDefault
          }
        };
      }
      function resultIsAllowed(result, protoAccessControl, propertyName) {
        if (typeof result === "function") {
          return checkWhiteList(protoAccessControl.methods, propertyName);
        } else {
          return checkWhiteList(protoAccessControl.properties, propertyName);
        }
      }
      function checkWhiteList(protoAccessControlForType, propertyName) {
        if (protoAccessControlForType.whitelist[propertyName] !== void 0) {
          return protoAccessControlForType.whitelist[propertyName] === true;
        }
        if (protoAccessControlForType.defaultValue !== void 0) {
          return protoAccessControlForType.defaultValue;
        }
        logUnexpecedPropertyAccessOnce(propertyName);
        return false;
      }
      function logUnexpecedPropertyAccessOnce(propertyName) {
        if (loggedProperties[propertyName] !== true) {
          loggedProperties[propertyName] = true;
          _logger2["default"].log("error", 'Handlebars: Access has been denied to resolve the property "' + propertyName + '" because it is not an "own property" of its parent.\nYou can add a runtime option to disable the check or this warning:\nSee https://handlebarsjs.com/api-reference/runtime-options.html#options-to-control-prototype-access for details');
        }
      }
      function resetLoggedProperties() {
        Object.keys(loggedProperties).forEach(function(propertyName) {
          delete loggedProperties[propertyName];
        });
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/base.js
  var require_base = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/base.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.HandlebarsEnvironment = HandlebarsEnvironment;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _utils = require_utils();
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      var _helpers = require_helpers();
      var _decorators = require_decorators();
      var _logger = require_logger();
      var _logger2 = _interopRequireDefault(_logger);
      var _internalProtoAccess = require_proto_access();
      var VERSION = "4.7.8";
      exports.VERSION = VERSION;
      var COMPILER_REVISION = 8;
      exports.COMPILER_REVISION = COMPILER_REVISION;
      var LAST_COMPATIBLE_COMPILER_REVISION = 7;
      exports.LAST_COMPATIBLE_COMPILER_REVISION = LAST_COMPATIBLE_COMPILER_REVISION;
      var REVISION_CHANGES = {
        1: "<= 1.0.rc.2",
        // 1.0.rc.2 is actually rev2 but doesn't report it
        2: "== 1.0.0-rc.3",
        3: "== 1.0.0-rc.4",
        4: "== 1.x.x",
        5: "== 2.0.0-alpha.x",
        6: ">= 2.0.0-beta.1",
        7: ">= 4.0.0 <4.3.0",
        8: ">= 4.3.0"
      };
      exports.REVISION_CHANGES = REVISION_CHANGES;
      var objectType2 = "[object Object]";
      function HandlebarsEnvironment(helpers, partials, decorators) {
        this.helpers = helpers || {};
        this.partials = partials || {};
        this.decorators = decorators || {};
        _helpers.registerDefaultHelpers(this);
        _decorators.registerDefaultDecorators(this);
      }
      HandlebarsEnvironment.prototype = {
        constructor: HandlebarsEnvironment,
        logger: _logger2["default"],
        log: _logger2["default"].log,
        registerHelper: function registerHelper(name2, fn2) {
          if (_utils.toString.call(name2) === objectType2) {
            if (fn2) {
              throw new _exception2["default"]("Arg not supported with multiple helpers");
            }
            _utils.extend(this.helpers, name2);
          } else {
            this.helpers[name2] = fn2;
          }
        },
        unregisterHelper: function unregisterHelper(name2) {
          delete this.helpers[name2];
        },
        registerPartial: function registerPartial(name2, partial) {
          if (_utils.toString.call(name2) === objectType2) {
            _utils.extend(this.partials, name2);
          } else {
            if (typeof partial === "undefined") {
              throw new _exception2["default"]('Attempting to register a partial called "' + name2 + '" as undefined');
            }
            this.partials[name2] = partial;
          }
        },
        unregisterPartial: function unregisterPartial(name2) {
          delete this.partials[name2];
        },
        registerDecorator: function registerDecorator(name2, fn2) {
          if (_utils.toString.call(name2) === objectType2) {
            if (fn2) {
              throw new _exception2["default"]("Arg not supported with multiple decorators");
            }
            _utils.extend(this.decorators, name2);
          } else {
            this.decorators[name2] = fn2;
          }
        },
        unregisterDecorator: function unregisterDecorator(name2) {
          delete this.decorators[name2];
        },
        /**
         * Reset the memory of illegal property accesses that have already been logged.
         * @deprecated should only be used in handlebars test-cases
         */
        resetLoggedPropertyAccesses: function resetLoggedPropertyAccesses() {
          _internalProtoAccess.resetLoggedProperties();
        }
      };
      var log = _logger2["default"].log;
      exports.log = log;
      exports.createFrame = _utils.createFrame;
      exports.logger = _logger2["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/safe-string.js
  var require_safe_string = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/safe-string.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function SafeString(string) {
        this.string = string;
      }
      SafeString.prototype.toString = SafeString.prototype.toHTML = function() {
        return "" + this.string;
      };
      exports["default"] = SafeString;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/internal/wrapHelper.js
  var require_wrapHelper = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/internal/wrapHelper.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.wrapHelper = wrapHelper;
      function wrapHelper(helper, transformOptionsFn) {
        if (typeof helper !== "function") {
          return helper;
        }
        var wrapper = function wrapper2() {
          var options = arguments[arguments.length - 1];
          arguments[arguments.length - 1] = transformOptionsFn(options);
          return helper.apply(this, arguments);
        };
        return wrapper;
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/runtime.js
  var require_runtime = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/runtime.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.checkRevision = checkRevision;
      exports.template = template;
      exports.wrapProgram = wrapProgram;
      exports.resolvePartial = resolvePartial;
      exports.invokePartial = invokePartial;
      exports.noop = noop;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      function _interopRequireWildcard(obj) {
        if (obj && obj.__esModule) {
          return obj;
        } else {
          var newObj = {};
          if (obj != null) {
            for (var key in obj) {
              if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key];
            }
          }
          newObj["default"] = obj;
          return newObj;
        }
      }
      var _utils = require_utils();
      var Utils = _interopRequireWildcard(_utils);
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      var _base = require_base();
      var _helpers = require_helpers();
      var _internalWrapHelper = require_wrapHelper();
      var _internalProtoAccess = require_proto_access();
      function checkRevision(compilerInfo) {
        var compilerRevision = compilerInfo && compilerInfo[0] || 1, currentRevision = _base.COMPILER_REVISION;
        if (compilerRevision >= _base.LAST_COMPATIBLE_COMPILER_REVISION && compilerRevision <= _base.COMPILER_REVISION) {
          return;
        }
        if (compilerRevision < _base.LAST_COMPATIBLE_COMPILER_REVISION) {
          var runtimeVersions = _base.REVISION_CHANGES[currentRevision], compilerVersions = _base.REVISION_CHANGES[compilerRevision];
          throw new _exception2["default"]("Template was precompiled with an older version of Handlebars than the current runtime. Please update your precompiler to a newer version (" + runtimeVersions + ") or downgrade your runtime to an older version (" + compilerVersions + ").");
        } else {
          throw new _exception2["default"]("Template was precompiled with a newer version of Handlebars than the current runtime. Please update your runtime to a newer version (" + compilerInfo[1] + ").");
        }
      }
      function template(templateSpec, env) {
        if (!env) {
          throw new _exception2["default"]("No environment passed to template");
        }
        if (!templateSpec || !templateSpec.main) {
          throw new _exception2["default"]("Unknown template object: " + typeof templateSpec);
        }
        templateSpec.main.decorator = templateSpec.main_d;
        env.VM.checkRevision(templateSpec.compiler);
        var templateWasPrecompiledWithCompilerV7 = templateSpec.compiler && templateSpec.compiler[0] === 7;
        function invokePartialWrapper(partial, context, options) {
          if (options.hash) {
            context = Utils.extend({}, context, options.hash);
            if (options.ids) {
              options.ids[0] = true;
            }
          }
          partial = env.VM.resolvePartial.call(this, partial, context, options);
          var extendedOptions = Utils.extend({}, options, {
            hooks: this.hooks,
            protoAccessControl: this.protoAccessControl
          });
          var result = env.VM.invokePartial.call(this, partial, context, extendedOptions);
          if (result == null && env.compile) {
            options.partials[options.name] = env.compile(partial, templateSpec.compilerOptions, env);
            result = options.partials[options.name](context, extendedOptions);
          }
          if (result != null) {
            if (options.indent) {
              var lines = result.split("\n");
              for (var i = 0, l = lines.length; i < l; i++) {
                if (!lines[i] && i + 1 === l) {
                  break;
                }
                lines[i] = options.indent + lines[i];
              }
              result = lines.join("\n");
            }
            return result;
          } else {
            throw new _exception2["default"]("The partial " + options.name + " could not be compiled when running in runtime-only mode");
          }
        }
        var container = {
          strict: function strict(obj, name2, loc) {
            if (!obj || !(name2 in obj)) {
              throw new _exception2["default"]('"' + name2 + '" not defined in ' + obj, {
                loc
              });
            }
            return container.lookupProperty(obj, name2);
          },
          lookupProperty: function lookupProperty(parent, propertyName) {
            var result = parent[propertyName];
            if (result == null) {
              return result;
            }
            if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
              return result;
            }
            if (_internalProtoAccess.resultIsAllowed(result, container.protoAccessControl, propertyName)) {
              return result;
            }
            return void 0;
          },
          lookup: function lookup(depths, name2) {
            var len = depths.length;
            for (var i = 0; i < len; i++) {
              var result = depths[i] && container.lookupProperty(depths[i], name2);
              if (result != null) {
                return depths[i][name2];
              }
            }
          },
          lambda: function lambda(current, context) {
            return typeof current === "function" ? current.call(context) : current;
          },
          escapeExpression: Utils.escapeExpression,
          invokePartial: invokePartialWrapper,
          fn: function fn2(i) {
            var ret2 = templateSpec[i];
            ret2.decorator = templateSpec[i + "_d"];
            return ret2;
          },
          programs: [],
          program: function program(i, data, declaredBlockParams, blockParams, depths) {
            var programWrapper = this.programs[i], fn2 = this.fn(i);
            if (data || depths || blockParams || declaredBlockParams) {
              programWrapper = wrapProgram(this, i, fn2, data, declaredBlockParams, blockParams, depths);
            } else if (!programWrapper) {
              programWrapper = this.programs[i] = wrapProgram(this, i, fn2);
            }
            return programWrapper;
          },
          data: function data(value, depth) {
            while (value && depth--) {
              value = value._parent;
            }
            return value;
          },
          mergeIfNeeded: function mergeIfNeeded(param, common) {
            var obj = param || common;
            if (param && common && param !== common) {
              obj = Utils.extend({}, common, param);
            }
            return obj;
          },
          // An empty object to use as replacement for null-contexts
          nullContext: Object.seal({}),
          noop: env.VM.noop,
          compilerInfo: templateSpec.compiler
        };
        function ret(context) {
          var options = arguments.length <= 1 || arguments[1] === void 0 ? {} : arguments[1];
          var data = options.data;
          ret._setup(options);
          if (!options.partial && templateSpec.useData) {
            data = initData(context, data);
          }
          var depths = void 0, blockParams = templateSpec.useBlockParams ? [] : void 0;
          if (templateSpec.useDepths) {
            if (options.depths) {
              depths = context != options.depths[0] ? [context].concat(options.depths) : options.depths;
            } else {
              depths = [context];
            }
          }
          function main(context2) {
            return "" + templateSpec.main(container, context2, container.helpers, container.partials, data, blockParams, depths);
          }
          main = executeDecorators(templateSpec.main, main, container, options.depths || [], data, blockParams);
          return main(context, options);
        }
        ret.isTop = true;
        ret._setup = function(options) {
          if (!options.partial) {
            var mergedHelpers = Utils.extend({}, env.helpers, options.helpers);
            wrapHelpersToPassLookupProperty(mergedHelpers, container);
            container.helpers = mergedHelpers;
            if (templateSpec.usePartial) {
              container.partials = container.mergeIfNeeded(options.partials, env.partials);
            }
            if (templateSpec.usePartial || templateSpec.useDecorators) {
              container.decorators = Utils.extend({}, env.decorators, options.decorators);
            }
            container.hooks = {};
            container.protoAccessControl = _internalProtoAccess.createProtoAccessControl(options);
            var keepHelperInHelpers = options.allowCallsToHelperMissing || templateWasPrecompiledWithCompilerV7;
            _helpers.moveHelperToHooks(container, "helperMissing", keepHelperInHelpers);
            _helpers.moveHelperToHooks(container, "blockHelperMissing", keepHelperInHelpers);
          } else {
            container.protoAccessControl = options.protoAccessControl;
            container.helpers = options.helpers;
            container.partials = options.partials;
            container.decorators = options.decorators;
            container.hooks = options.hooks;
          }
        };
        ret._child = function(i, data, blockParams, depths) {
          if (templateSpec.useBlockParams && !blockParams) {
            throw new _exception2["default"]("must pass block params");
          }
          if (templateSpec.useDepths && !depths) {
            throw new _exception2["default"]("must pass parent depths");
          }
          return wrapProgram(container, i, templateSpec[i], data, 0, blockParams, depths);
        };
        return ret;
      }
      function wrapProgram(container, i, fn2, data, declaredBlockParams, blockParams, depths) {
        function prog(context) {
          var options = arguments.length <= 1 || arguments[1] === void 0 ? {} : arguments[1];
          var currentDepths = depths;
          if (depths && context != depths[0] && !(context === container.nullContext && depths[0] === null)) {
            currentDepths = [context].concat(depths);
          }
          return fn2(container, context, container.helpers, container.partials, options.data || data, blockParams && [options.blockParams].concat(blockParams), currentDepths);
        }
        prog = executeDecorators(fn2, prog, container, depths, data, blockParams);
        prog.program = i;
        prog.depth = depths ? depths.length : 0;
        prog.blockParams = declaredBlockParams || 0;
        return prog;
      }
      function resolvePartial(partial, context, options) {
        if (!partial) {
          if (options.name === "@partial-block") {
            partial = options.data["partial-block"];
          } else {
            partial = options.partials[options.name];
          }
        } else if (!partial.call && !options.name) {
          options.name = partial;
          partial = options.partials[partial];
        }
        return partial;
      }
      function invokePartial(partial, context, options) {
        var currentPartialBlock = options.data && options.data["partial-block"];
        options.partial = true;
        if (options.ids) {
          options.data.contextPath = options.ids[0] || options.data.contextPath;
        }
        var partialBlock = void 0;
        if (options.fn && options.fn !== noop) {
          (function() {
            options.data = _base.createFrame(options.data);
            var fn2 = options.fn;
            partialBlock = options.data["partial-block"] = function partialBlockWrapper(context2) {
              var options2 = arguments.length <= 1 || arguments[1] === void 0 ? {} : arguments[1];
              options2.data = _base.createFrame(options2.data);
              options2.data["partial-block"] = currentPartialBlock;
              return fn2(context2, options2);
            };
            if (fn2.partials) {
              options.partials = Utils.extend({}, options.partials, fn2.partials);
            }
          })();
        }
        if (partial === void 0 && partialBlock) {
          partial = partialBlock;
        }
        if (partial === void 0) {
          throw new _exception2["default"]("The partial " + options.name + " could not be found");
        } else if (partial instanceof Function) {
          return partial(context, options);
        }
      }
      function noop() {
        return "";
      }
      function initData(context, data) {
        if (!data || !("root" in data)) {
          data = data ? _base.createFrame(data) : {};
          data.root = context;
        }
        return data;
      }
      function executeDecorators(fn2, prog, container, depths, data, blockParams) {
        if (fn2.decorator) {
          var props = {};
          prog = fn2.decorator(prog, props, container, depths && depths[0], data, blockParams, depths);
          Utils.extend(prog, props);
        }
        return prog;
      }
      function wrapHelpersToPassLookupProperty(mergedHelpers, container) {
        Object.keys(mergedHelpers).forEach(function(helperName) {
          var helper = mergedHelpers[helperName];
          mergedHelpers[helperName] = passLookupPropertyOption(helper, container);
        });
      }
      function passLookupPropertyOption(helper, container) {
        var lookupProperty = container.lookupProperty;
        return _internalWrapHelper.wrapHelper(helper, function(options) {
          return Utils.extend({ lookupProperty }, options);
        });
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/no-conflict.js
  var require_no_conflict = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/no-conflict.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      exports["default"] = function(Handlebars3) {
        (function() {
          if (typeof globalThis === "object") return;
          Object.prototype.__defineGetter__("__magic__", function() {
            return this;
          });
          __magic__.globalThis = __magic__;
          delete Object.prototype.__magic__;
        })();
        var $Handlebars = globalThis.Handlebars;
        Handlebars3.noConflict = function() {
          if (globalThis.Handlebars === Handlebars3) {
            globalThis.Handlebars = $Handlebars;
          }
          return Handlebars3;
        };
      };
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars.runtime.js
  var require_handlebars_runtime = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars.runtime.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      function _interopRequireWildcard(obj) {
        if (obj && obj.__esModule) {
          return obj;
        } else {
          var newObj = {};
          if (obj != null) {
            for (var key in obj) {
              if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key];
            }
          }
          newObj["default"] = obj;
          return newObj;
        }
      }
      var _handlebarsBase = require_base();
      var base = _interopRequireWildcard(_handlebarsBase);
      var _handlebarsSafeString = require_safe_string();
      var _handlebarsSafeString2 = _interopRequireDefault(_handlebarsSafeString);
      var _handlebarsException = require_exception();
      var _handlebarsException2 = _interopRequireDefault(_handlebarsException);
      var _handlebarsUtils = require_utils();
      var Utils = _interopRequireWildcard(_handlebarsUtils);
      var _handlebarsRuntime = require_runtime();
      var runtime = _interopRequireWildcard(_handlebarsRuntime);
      var _handlebarsNoConflict = require_no_conflict();
      var _handlebarsNoConflict2 = _interopRequireDefault(_handlebarsNoConflict);
      function create() {
        var hb = new base.HandlebarsEnvironment();
        Utils.extend(hb, base);
        hb.SafeString = _handlebarsSafeString2["default"];
        hb.Exception = _handlebarsException2["default"];
        hb.Utils = Utils;
        hb.escapeExpression = Utils.escapeExpression;
        hb.VM = runtime;
        hb.template = function(spec) {
          return runtime.template(spec, hb);
        };
        return hb;
      }
      var inst = create();
      inst.create = create;
      _handlebarsNoConflict2["default"](inst);
      inst["default"] = inst;
      exports["default"] = inst;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/ast.js
  var require_ast = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/ast.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var AST = {
        // Public API used to evaluate derived attributes regarding AST nodes
        helpers: {
          // a mustache is definitely a helper if:
          // * it is an eligible helper, and
          // * it has at least one parameter or hash segment
          helperExpression: function helperExpression(node) {
            return node.type === "SubExpression" || (node.type === "MustacheStatement" || node.type === "BlockStatement") && !!(node.params && node.params.length || node.hash);
          },
          scopedId: function scopedId(path) {
            return /^\.|this\b/.test(path.original);
          },
          // an ID is simple if it only has one part, and that part is not
          // `..` or `this`.
          simpleId: function simpleId(path) {
            return path.parts.length === 1 && !AST.helpers.scopedId(path) && !path.depth;
          }
        }
      };
      exports["default"] = AST;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/parser.js
  var require_parser = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/parser.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var handlebars = function() {
        var parser = {
          trace: function trace() {
          },
          yy: {},
          symbols_: { "error": 2, "root": 3, "program": 4, "EOF": 5, "program_repetition0": 6, "statement": 7, "mustache": 8, "block": 9, "rawBlock": 10, "partial": 11, "partialBlock": 12, "content": 13, "COMMENT": 14, "CONTENT": 15, "openRawBlock": 16, "rawBlock_repetition0": 17, "END_RAW_BLOCK": 18, "OPEN_RAW_BLOCK": 19, "helperName": 20, "openRawBlock_repetition0": 21, "openRawBlock_option0": 22, "CLOSE_RAW_BLOCK": 23, "openBlock": 24, "block_option0": 25, "closeBlock": 26, "openInverse": 27, "block_option1": 28, "OPEN_BLOCK": 29, "openBlock_repetition0": 30, "openBlock_option0": 31, "openBlock_option1": 32, "CLOSE": 33, "OPEN_INVERSE": 34, "openInverse_repetition0": 35, "openInverse_option0": 36, "openInverse_option1": 37, "openInverseChain": 38, "OPEN_INVERSE_CHAIN": 39, "openInverseChain_repetition0": 40, "openInverseChain_option0": 41, "openInverseChain_option1": 42, "inverseAndProgram": 43, "INVERSE": 44, "inverseChain": 45, "inverseChain_option0": 46, "OPEN_ENDBLOCK": 47, "OPEN": 48, "mustache_repetition0": 49, "mustache_option0": 50, "OPEN_UNESCAPED": 51, "mustache_repetition1": 52, "mustache_option1": 53, "CLOSE_UNESCAPED": 54, "OPEN_PARTIAL": 55, "partialName": 56, "partial_repetition0": 57, "partial_option0": 58, "openPartialBlock": 59, "OPEN_PARTIAL_BLOCK": 60, "openPartialBlock_repetition0": 61, "openPartialBlock_option0": 62, "param": 63, "sexpr": 64, "OPEN_SEXPR": 65, "sexpr_repetition0": 66, "sexpr_option0": 67, "CLOSE_SEXPR": 68, "hash": 69, "hash_repetition_plus0": 70, "hashSegment": 71, "ID": 72, "EQUALS": 73, "blockParams": 74, "OPEN_BLOCK_PARAMS": 75, "blockParams_repetition_plus0": 76, "CLOSE_BLOCK_PARAMS": 77, "path": 78, "dataName": 79, "STRING": 80, "NUMBER": 81, "BOOLEAN": 82, "UNDEFINED": 83, "NULL": 84, "DATA": 85, "pathSegments": 86, "SEP": 87, "$accept": 0, "$end": 1 },
          terminals_: { 2: "error", 5: "EOF", 14: "COMMENT", 15: "CONTENT", 18: "END_RAW_BLOCK", 19: "OPEN_RAW_BLOCK", 23: "CLOSE_RAW_BLOCK", 29: "OPEN_BLOCK", 33: "CLOSE", 34: "OPEN_INVERSE", 39: "OPEN_INVERSE_CHAIN", 44: "INVERSE", 47: "OPEN_ENDBLOCK", 48: "OPEN", 51: "OPEN_UNESCAPED", 54: "CLOSE_UNESCAPED", 55: "OPEN_PARTIAL", 60: "OPEN_PARTIAL_BLOCK", 65: "OPEN_SEXPR", 68: "CLOSE_SEXPR", 72: "ID", 73: "EQUALS", 75: "OPEN_BLOCK_PARAMS", 77: "CLOSE_BLOCK_PARAMS", 80: "STRING", 81: "NUMBER", 82: "BOOLEAN", 83: "UNDEFINED", 84: "NULL", 85: "DATA", 87: "SEP" },
          productions_: [0, [3, 2], [4, 1], [7, 1], [7, 1], [7, 1], [7, 1], [7, 1], [7, 1], [7, 1], [13, 1], [10, 3], [16, 5], [9, 4], [9, 4], [24, 6], [27, 6], [38, 6], [43, 2], [45, 3], [45, 1], [26, 3], [8, 5], [8, 5], [11, 5], [12, 3], [59, 5], [63, 1], [63, 1], [64, 5], [69, 1], [71, 3], [74, 3], [20, 1], [20, 1], [20, 1], [20, 1], [20, 1], [20, 1], [20, 1], [56, 1], [56, 1], [79, 2], [78, 1], [86, 3], [86, 1], [6, 0], [6, 2], [17, 0], [17, 2], [21, 0], [21, 2], [22, 0], [22, 1], [25, 0], [25, 1], [28, 0], [28, 1], [30, 0], [30, 2], [31, 0], [31, 1], [32, 0], [32, 1], [35, 0], [35, 2], [36, 0], [36, 1], [37, 0], [37, 1], [40, 0], [40, 2], [41, 0], [41, 1], [42, 0], [42, 1], [46, 0], [46, 1], [49, 0], [49, 2], [50, 0], [50, 1], [52, 0], [52, 2], [53, 0], [53, 1], [57, 0], [57, 2], [58, 0], [58, 1], [61, 0], [61, 2], [62, 0], [62, 1], [66, 0], [66, 2], [67, 0], [67, 1], [70, 1], [70, 2], [76, 1], [76, 2]],
          performAction: function anonymous(yytext, yyleng, yylineno, yy, yystate, $$, _$) {
            var $0 = $$.length - 1;
            switch (yystate) {
              case 1:
                return $$[$0 - 1];
                break;
              case 2:
                this.$ = yy.prepareProgram($$[$0]);
                break;
              case 3:
                this.$ = $$[$0];
                break;
              case 4:
                this.$ = $$[$0];
                break;
              case 5:
                this.$ = $$[$0];
                break;
              case 6:
                this.$ = $$[$0];
                break;
              case 7:
                this.$ = $$[$0];
                break;
              case 8:
                this.$ = $$[$0];
                break;
              case 9:
                this.$ = {
                  type: "CommentStatement",
                  value: yy.stripComment($$[$0]),
                  strip: yy.stripFlags($$[$0], $$[$0]),
                  loc: yy.locInfo(this._$)
                };
                break;
              case 10:
                this.$ = {
                  type: "ContentStatement",
                  original: $$[$0],
                  value: $$[$0],
                  loc: yy.locInfo(this._$)
                };
                break;
              case 11:
                this.$ = yy.prepareRawBlock($$[$0 - 2], $$[$0 - 1], $$[$0], this._$);
                break;
              case 12:
                this.$ = { path: $$[$0 - 3], params: $$[$0 - 2], hash: $$[$0 - 1] };
                break;
              case 13:
                this.$ = yy.prepareBlock($$[$0 - 3], $$[$0 - 2], $$[$0 - 1], $$[$0], false, this._$);
                break;
              case 14:
                this.$ = yy.prepareBlock($$[$0 - 3], $$[$0 - 2], $$[$0 - 1], $$[$0], true, this._$);
                break;
              case 15:
                this.$ = { open: $$[$0 - 5], path: $$[$0 - 4], params: $$[$0 - 3], hash: $$[$0 - 2], blockParams: $$[$0 - 1], strip: yy.stripFlags($$[$0 - 5], $$[$0]) };
                break;
              case 16:
                this.$ = { path: $$[$0 - 4], params: $$[$0 - 3], hash: $$[$0 - 2], blockParams: $$[$0 - 1], strip: yy.stripFlags($$[$0 - 5], $$[$0]) };
                break;
              case 17:
                this.$ = { path: $$[$0 - 4], params: $$[$0 - 3], hash: $$[$0 - 2], blockParams: $$[$0 - 1], strip: yy.stripFlags($$[$0 - 5], $$[$0]) };
                break;
              case 18:
                this.$ = { strip: yy.stripFlags($$[$0 - 1], $$[$0 - 1]), program: $$[$0] };
                break;
              case 19:
                var inverse = yy.prepareBlock($$[$0 - 2], $$[$0 - 1], $$[$0], $$[$0], false, this._$), program = yy.prepareProgram([inverse], $$[$0 - 1].loc);
                program.chained = true;
                this.$ = { strip: $$[$0 - 2].strip, program, chain: true };
                break;
              case 20:
                this.$ = $$[$0];
                break;
              case 21:
                this.$ = { path: $$[$0 - 1], strip: yy.stripFlags($$[$0 - 2], $$[$0]) };
                break;
              case 22:
                this.$ = yy.prepareMustache($$[$0 - 3], $$[$0 - 2], $$[$0 - 1], $$[$0 - 4], yy.stripFlags($$[$0 - 4], $$[$0]), this._$);
                break;
              case 23:
                this.$ = yy.prepareMustache($$[$0 - 3], $$[$0 - 2], $$[$0 - 1], $$[$0 - 4], yy.stripFlags($$[$0 - 4], $$[$0]), this._$);
                break;
              case 24:
                this.$ = {
                  type: "PartialStatement",
                  name: $$[$0 - 3],
                  params: $$[$0 - 2],
                  hash: $$[$0 - 1],
                  indent: "",
                  strip: yy.stripFlags($$[$0 - 4], $$[$0]),
                  loc: yy.locInfo(this._$)
                };
                break;
              case 25:
                this.$ = yy.preparePartialBlock($$[$0 - 2], $$[$0 - 1], $$[$0], this._$);
                break;
              case 26:
                this.$ = { path: $$[$0 - 3], params: $$[$0 - 2], hash: $$[$0 - 1], strip: yy.stripFlags($$[$0 - 4], $$[$0]) };
                break;
              case 27:
                this.$ = $$[$0];
                break;
              case 28:
                this.$ = $$[$0];
                break;
              case 29:
                this.$ = {
                  type: "SubExpression",
                  path: $$[$0 - 3],
                  params: $$[$0 - 2],
                  hash: $$[$0 - 1],
                  loc: yy.locInfo(this._$)
                };
                break;
              case 30:
                this.$ = { type: "Hash", pairs: $$[$0], loc: yy.locInfo(this._$) };
                break;
              case 31:
                this.$ = { type: "HashPair", key: yy.id($$[$0 - 2]), value: $$[$0], loc: yy.locInfo(this._$) };
                break;
              case 32:
                this.$ = yy.id($$[$0 - 1]);
                break;
              case 33:
                this.$ = $$[$0];
                break;
              case 34:
                this.$ = $$[$0];
                break;
              case 35:
                this.$ = { type: "StringLiteral", value: $$[$0], original: $$[$0], loc: yy.locInfo(this._$) };
                break;
              case 36:
                this.$ = { type: "NumberLiteral", value: Number($$[$0]), original: Number($$[$0]), loc: yy.locInfo(this._$) };
                break;
              case 37:
                this.$ = { type: "BooleanLiteral", value: $$[$0] === "true", original: $$[$0] === "true", loc: yy.locInfo(this._$) };
                break;
              case 38:
                this.$ = { type: "UndefinedLiteral", original: void 0, value: void 0, loc: yy.locInfo(this._$) };
                break;
              case 39:
                this.$ = { type: "NullLiteral", original: null, value: null, loc: yy.locInfo(this._$) };
                break;
              case 40:
                this.$ = $$[$0];
                break;
              case 41:
                this.$ = $$[$0];
                break;
              case 42:
                this.$ = yy.preparePath(true, $$[$0], this._$);
                break;
              case 43:
                this.$ = yy.preparePath(false, $$[$0], this._$);
                break;
              case 44:
                $$[$0 - 2].push({ part: yy.id($$[$0]), original: $$[$0], separator: $$[$0 - 1] });
                this.$ = $$[$0 - 2];
                break;
              case 45:
                this.$ = [{ part: yy.id($$[$0]), original: $$[$0] }];
                break;
              case 46:
                this.$ = [];
                break;
              case 47:
                $$[$0 - 1].push($$[$0]);
                break;
              case 48:
                this.$ = [];
                break;
              case 49:
                $$[$0 - 1].push($$[$0]);
                break;
              case 50:
                this.$ = [];
                break;
              case 51:
                $$[$0 - 1].push($$[$0]);
                break;
              case 58:
                this.$ = [];
                break;
              case 59:
                $$[$0 - 1].push($$[$0]);
                break;
              case 64:
                this.$ = [];
                break;
              case 65:
                $$[$0 - 1].push($$[$0]);
                break;
              case 70:
                this.$ = [];
                break;
              case 71:
                $$[$0 - 1].push($$[$0]);
                break;
              case 78:
                this.$ = [];
                break;
              case 79:
                $$[$0 - 1].push($$[$0]);
                break;
              case 82:
                this.$ = [];
                break;
              case 83:
                $$[$0 - 1].push($$[$0]);
                break;
              case 86:
                this.$ = [];
                break;
              case 87:
                $$[$0 - 1].push($$[$0]);
                break;
              case 90:
                this.$ = [];
                break;
              case 91:
                $$[$0 - 1].push($$[$0]);
                break;
              case 94:
                this.$ = [];
                break;
              case 95:
                $$[$0 - 1].push($$[$0]);
                break;
              case 98:
                this.$ = [$$[$0]];
                break;
              case 99:
                $$[$0 - 1].push($$[$0]);
                break;
              case 100:
                this.$ = [$$[$0]];
                break;
              case 101:
                $$[$0 - 1].push($$[$0]);
                break;
            }
          },
          table: [{ 3: 1, 4: 2, 5: [2, 46], 6: 3, 14: [2, 46], 15: [2, 46], 19: [2, 46], 29: [2, 46], 34: [2, 46], 48: [2, 46], 51: [2, 46], 55: [2, 46], 60: [2, 46] }, { 1: [3] }, { 5: [1, 4] }, { 5: [2, 2], 7: 5, 8: 6, 9: 7, 10: 8, 11: 9, 12: 10, 13: 11, 14: [1, 12], 15: [1, 20], 16: 17, 19: [1, 23], 24: 15, 27: 16, 29: [1, 21], 34: [1, 22], 39: [2, 2], 44: [2, 2], 47: [2, 2], 48: [1, 13], 51: [1, 14], 55: [1, 18], 59: 19, 60: [1, 24] }, { 1: [2, 1] }, { 5: [2, 47], 14: [2, 47], 15: [2, 47], 19: [2, 47], 29: [2, 47], 34: [2, 47], 39: [2, 47], 44: [2, 47], 47: [2, 47], 48: [2, 47], 51: [2, 47], 55: [2, 47], 60: [2, 47] }, { 5: [2, 3], 14: [2, 3], 15: [2, 3], 19: [2, 3], 29: [2, 3], 34: [2, 3], 39: [2, 3], 44: [2, 3], 47: [2, 3], 48: [2, 3], 51: [2, 3], 55: [2, 3], 60: [2, 3] }, { 5: [2, 4], 14: [2, 4], 15: [2, 4], 19: [2, 4], 29: [2, 4], 34: [2, 4], 39: [2, 4], 44: [2, 4], 47: [2, 4], 48: [2, 4], 51: [2, 4], 55: [2, 4], 60: [2, 4] }, { 5: [2, 5], 14: [2, 5], 15: [2, 5], 19: [2, 5], 29: [2, 5], 34: [2, 5], 39: [2, 5], 44: [2, 5], 47: [2, 5], 48: [2, 5], 51: [2, 5], 55: [2, 5], 60: [2, 5] }, { 5: [2, 6], 14: [2, 6], 15: [2, 6], 19: [2, 6], 29: [2, 6], 34: [2, 6], 39: [2, 6], 44: [2, 6], 47: [2, 6], 48: [2, 6], 51: [2, 6], 55: [2, 6], 60: [2, 6] }, { 5: [2, 7], 14: [2, 7], 15: [2, 7], 19: [2, 7], 29: [2, 7], 34: [2, 7], 39: [2, 7], 44: [2, 7], 47: [2, 7], 48: [2, 7], 51: [2, 7], 55: [2, 7], 60: [2, 7] }, { 5: [2, 8], 14: [2, 8], 15: [2, 8], 19: [2, 8], 29: [2, 8], 34: [2, 8], 39: [2, 8], 44: [2, 8], 47: [2, 8], 48: [2, 8], 51: [2, 8], 55: [2, 8], 60: [2, 8] }, { 5: [2, 9], 14: [2, 9], 15: [2, 9], 19: [2, 9], 29: [2, 9], 34: [2, 9], 39: [2, 9], 44: [2, 9], 47: [2, 9], 48: [2, 9], 51: [2, 9], 55: [2, 9], 60: [2, 9] }, { 20: 25, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 36, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 4: 37, 6: 3, 14: [2, 46], 15: [2, 46], 19: [2, 46], 29: [2, 46], 34: [2, 46], 39: [2, 46], 44: [2, 46], 47: [2, 46], 48: [2, 46], 51: [2, 46], 55: [2, 46], 60: [2, 46] }, { 4: 38, 6: 3, 14: [2, 46], 15: [2, 46], 19: [2, 46], 29: [2, 46], 34: [2, 46], 44: [2, 46], 47: [2, 46], 48: [2, 46], 51: [2, 46], 55: [2, 46], 60: [2, 46] }, { 15: [2, 48], 17: 39, 18: [2, 48] }, { 20: 41, 56: 40, 64: 42, 65: [1, 43], 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 4: 44, 6: 3, 14: [2, 46], 15: [2, 46], 19: [2, 46], 29: [2, 46], 34: [2, 46], 47: [2, 46], 48: [2, 46], 51: [2, 46], 55: [2, 46], 60: [2, 46] }, { 5: [2, 10], 14: [2, 10], 15: [2, 10], 18: [2, 10], 19: [2, 10], 29: [2, 10], 34: [2, 10], 39: [2, 10], 44: [2, 10], 47: [2, 10], 48: [2, 10], 51: [2, 10], 55: [2, 10], 60: [2, 10] }, { 20: 45, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 46, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 47, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 41, 56: 48, 64: 42, 65: [1, 43], 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 33: [2, 78], 49: 49, 65: [2, 78], 72: [2, 78], 80: [2, 78], 81: [2, 78], 82: [2, 78], 83: [2, 78], 84: [2, 78], 85: [2, 78] }, { 23: [2, 33], 33: [2, 33], 54: [2, 33], 65: [2, 33], 68: [2, 33], 72: [2, 33], 75: [2, 33], 80: [2, 33], 81: [2, 33], 82: [2, 33], 83: [2, 33], 84: [2, 33], 85: [2, 33] }, { 23: [2, 34], 33: [2, 34], 54: [2, 34], 65: [2, 34], 68: [2, 34], 72: [2, 34], 75: [2, 34], 80: [2, 34], 81: [2, 34], 82: [2, 34], 83: [2, 34], 84: [2, 34], 85: [2, 34] }, { 23: [2, 35], 33: [2, 35], 54: [2, 35], 65: [2, 35], 68: [2, 35], 72: [2, 35], 75: [2, 35], 80: [2, 35], 81: [2, 35], 82: [2, 35], 83: [2, 35], 84: [2, 35], 85: [2, 35] }, { 23: [2, 36], 33: [2, 36], 54: [2, 36], 65: [2, 36], 68: [2, 36], 72: [2, 36], 75: [2, 36], 80: [2, 36], 81: [2, 36], 82: [2, 36], 83: [2, 36], 84: [2, 36], 85: [2, 36] }, { 23: [2, 37], 33: [2, 37], 54: [2, 37], 65: [2, 37], 68: [2, 37], 72: [2, 37], 75: [2, 37], 80: [2, 37], 81: [2, 37], 82: [2, 37], 83: [2, 37], 84: [2, 37], 85: [2, 37] }, { 23: [2, 38], 33: [2, 38], 54: [2, 38], 65: [2, 38], 68: [2, 38], 72: [2, 38], 75: [2, 38], 80: [2, 38], 81: [2, 38], 82: [2, 38], 83: [2, 38], 84: [2, 38], 85: [2, 38] }, { 23: [2, 39], 33: [2, 39], 54: [2, 39], 65: [2, 39], 68: [2, 39], 72: [2, 39], 75: [2, 39], 80: [2, 39], 81: [2, 39], 82: [2, 39], 83: [2, 39], 84: [2, 39], 85: [2, 39] }, { 23: [2, 43], 33: [2, 43], 54: [2, 43], 65: [2, 43], 68: [2, 43], 72: [2, 43], 75: [2, 43], 80: [2, 43], 81: [2, 43], 82: [2, 43], 83: [2, 43], 84: [2, 43], 85: [2, 43], 87: [1, 50] }, { 72: [1, 35], 86: 51 }, { 23: [2, 45], 33: [2, 45], 54: [2, 45], 65: [2, 45], 68: [2, 45], 72: [2, 45], 75: [2, 45], 80: [2, 45], 81: [2, 45], 82: [2, 45], 83: [2, 45], 84: [2, 45], 85: [2, 45], 87: [2, 45] }, { 52: 52, 54: [2, 82], 65: [2, 82], 72: [2, 82], 80: [2, 82], 81: [2, 82], 82: [2, 82], 83: [2, 82], 84: [2, 82], 85: [2, 82] }, { 25: 53, 38: 55, 39: [1, 57], 43: 56, 44: [1, 58], 45: 54, 47: [2, 54] }, { 28: 59, 43: 60, 44: [1, 58], 47: [2, 56] }, { 13: 62, 15: [1, 20], 18: [1, 61] }, { 33: [2, 86], 57: 63, 65: [2, 86], 72: [2, 86], 80: [2, 86], 81: [2, 86], 82: [2, 86], 83: [2, 86], 84: [2, 86], 85: [2, 86] }, { 33: [2, 40], 65: [2, 40], 72: [2, 40], 80: [2, 40], 81: [2, 40], 82: [2, 40], 83: [2, 40], 84: [2, 40], 85: [2, 40] }, { 33: [2, 41], 65: [2, 41], 72: [2, 41], 80: [2, 41], 81: [2, 41], 82: [2, 41], 83: [2, 41], 84: [2, 41], 85: [2, 41] }, { 20: 64, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 26: 65, 47: [1, 66] }, { 30: 67, 33: [2, 58], 65: [2, 58], 72: [2, 58], 75: [2, 58], 80: [2, 58], 81: [2, 58], 82: [2, 58], 83: [2, 58], 84: [2, 58], 85: [2, 58] }, { 33: [2, 64], 35: 68, 65: [2, 64], 72: [2, 64], 75: [2, 64], 80: [2, 64], 81: [2, 64], 82: [2, 64], 83: [2, 64], 84: [2, 64], 85: [2, 64] }, { 21: 69, 23: [2, 50], 65: [2, 50], 72: [2, 50], 80: [2, 50], 81: [2, 50], 82: [2, 50], 83: [2, 50], 84: [2, 50], 85: [2, 50] }, { 33: [2, 90], 61: 70, 65: [2, 90], 72: [2, 90], 80: [2, 90], 81: [2, 90], 82: [2, 90], 83: [2, 90], 84: [2, 90], 85: [2, 90] }, { 20: 74, 33: [2, 80], 50: 71, 63: 72, 64: 75, 65: [1, 43], 69: 73, 70: 76, 71: 77, 72: [1, 78], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 72: [1, 79] }, { 23: [2, 42], 33: [2, 42], 54: [2, 42], 65: [2, 42], 68: [2, 42], 72: [2, 42], 75: [2, 42], 80: [2, 42], 81: [2, 42], 82: [2, 42], 83: [2, 42], 84: [2, 42], 85: [2, 42], 87: [1, 50] }, { 20: 74, 53: 80, 54: [2, 84], 63: 81, 64: 75, 65: [1, 43], 69: 82, 70: 76, 71: 77, 72: [1, 78], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 26: 83, 47: [1, 66] }, { 47: [2, 55] }, { 4: 84, 6: 3, 14: [2, 46], 15: [2, 46], 19: [2, 46], 29: [2, 46], 34: [2, 46], 39: [2, 46], 44: [2, 46], 47: [2, 46], 48: [2, 46], 51: [2, 46], 55: [2, 46], 60: [2, 46] }, { 47: [2, 20] }, { 20: 85, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 4: 86, 6: 3, 14: [2, 46], 15: [2, 46], 19: [2, 46], 29: [2, 46], 34: [2, 46], 47: [2, 46], 48: [2, 46], 51: [2, 46], 55: [2, 46], 60: [2, 46] }, { 26: 87, 47: [1, 66] }, { 47: [2, 57] }, { 5: [2, 11], 14: [2, 11], 15: [2, 11], 19: [2, 11], 29: [2, 11], 34: [2, 11], 39: [2, 11], 44: [2, 11], 47: [2, 11], 48: [2, 11], 51: [2, 11], 55: [2, 11], 60: [2, 11] }, { 15: [2, 49], 18: [2, 49] }, { 20: 74, 33: [2, 88], 58: 88, 63: 89, 64: 75, 65: [1, 43], 69: 90, 70: 76, 71: 77, 72: [1, 78], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 65: [2, 94], 66: 91, 68: [2, 94], 72: [2, 94], 80: [2, 94], 81: [2, 94], 82: [2, 94], 83: [2, 94], 84: [2, 94], 85: [2, 94] }, { 5: [2, 25], 14: [2, 25], 15: [2, 25], 19: [2, 25], 29: [2, 25], 34: [2, 25], 39: [2, 25], 44: [2, 25], 47: [2, 25], 48: [2, 25], 51: [2, 25], 55: [2, 25], 60: [2, 25] }, { 20: 92, 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 74, 31: 93, 33: [2, 60], 63: 94, 64: 75, 65: [1, 43], 69: 95, 70: 76, 71: 77, 72: [1, 78], 75: [2, 60], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 74, 33: [2, 66], 36: 96, 63: 97, 64: 75, 65: [1, 43], 69: 98, 70: 76, 71: 77, 72: [1, 78], 75: [2, 66], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 74, 22: 99, 23: [2, 52], 63: 100, 64: 75, 65: [1, 43], 69: 101, 70: 76, 71: 77, 72: [1, 78], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 20: 74, 33: [2, 92], 62: 102, 63: 103, 64: 75, 65: [1, 43], 69: 104, 70: 76, 71: 77, 72: [1, 78], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 33: [1, 105] }, { 33: [2, 79], 65: [2, 79], 72: [2, 79], 80: [2, 79], 81: [2, 79], 82: [2, 79], 83: [2, 79], 84: [2, 79], 85: [2, 79] }, { 33: [2, 81] }, { 23: [2, 27], 33: [2, 27], 54: [2, 27], 65: [2, 27], 68: [2, 27], 72: [2, 27], 75: [2, 27], 80: [2, 27], 81: [2, 27], 82: [2, 27], 83: [2, 27], 84: [2, 27], 85: [2, 27] }, { 23: [2, 28], 33: [2, 28], 54: [2, 28], 65: [2, 28], 68: [2, 28], 72: [2, 28], 75: [2, 28], 80: [2, 28], 81: [2, 28], 82: [2, 28], 83: [2, 28], 84: [2, 28], 85: [2, 28] }, { 23: [2, 30], 33: [2, 30], 54: [2, 30], 68: [2, 30], 71: 106, 72: [1, 107], 75: [2, 30] }, { 23: [2, 98], 33: [2, 98], 54: [2, 98], 68: [2, 98], 72: [2, 98], 75: [2, 98] }, { 23: [2, 45], 33: [2, 45], 54: [2, 45], 65: [2, 45], 68: [2, 45], 72: [2, 45], 73: [1, 108], 75: [2, 45], 80: [2, 45], 81: [2, 45], 82: [2, 45], 83: [2, 45], 84: [2, 45], 85: [2, 45], 87: [2, 45] }, { 23: [2, 44], 33: [2, 44], 54: [2, 44], 65: [2, 44], 68: [2, 44], 72: [2, 44], 75: [2, 44], 80: [2, 44], 81: [2, 44], 82: [2, 44], 83: [2, 44], 84: [2, 44], 85: [2, 44], 87: [2, 44] }, { 54: [1, 109] }, { 54: [2, 83], 65: [2, 83], 72: [2, 83], 80: [2, 83], 81: [2, 83], 82: [2, 83], 83: [2, 83], 84: [2, 83], 85: [2, 83] }, { 54: [2, 85] }, { 5: [2, 13], 14: [2, 13], 15: [2, 13], 19: [2, 13], 29: [2, 13], 34: [2, 13], 39: [2, 13], 44: [2, 13], 47: [2, 13], 48: [2, 13], 51: [2, 13], 55: [2, 13], 60: [2, 13] }, { 38: 55, 39: [1, 57], 43: 56, 44: [1, 58], 45: 111, 46: 110, 47: [2, 76] }, { 33: [2, 70], 40: 112, 65: [2, 70], 72: [2, 70], 75: [2, 70], 80: [2, 70], 81: [2, 70], 82: [2, 70], 83: [2, 70], 84: [2, 70], 85: [2, 70] }, { 47: [2, 18] }, { 5: [2, 14], 14: [2, 14], 15: [2, 14], 19: [2, 14], 29: [2, 14], 34: [2, 14], 39: [2, 14], 44: [2, 14], 47: [2, 14], 48: [2, 14], 51: [2, 14], 55: [2, 14], 60: [2, 14] }, { 33: [1, 113] }, { 33: [2, 87], 65: [2, 87], 72: [2, 87], 80: [2, 87], 81: [2, 87], 82: [2, 87], 83: [2, 87], 84: [2, 87], 85: [2, 87] }, { 33: [2, 89] }, { 20: 74, 63: 115, 64: 75, 65: [1, 43], 67: 114, 68: [2, 96], 69: 116, 70: 76, 71: 77, 72: [1, 78], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 33: [1, 117] }, { 32: 118, 33: [2, 62], 74: 119, 75: [1, 120] }, { 33: [2, 59], 65: [2, 59], 72: [2, 59], 75: [2, 59], 80: [2, 59], 81: [2, 59], 82: [2, 59], 83: [2, 59], 84: [2, 59], 85: [2, 59] }, { 33: [2, 61], 75: [2, 61] }, { 33: [2, 68], 37: 121, 74: 122, 75: [1, 120] }, { 33: [2, 65], 65: [2, 65], 72: [2, 65], 75: [2, 65], 80: [2, 65], 81: [2, 65], 82: [2, 65], 83: [2, 65], 84: [2, 65], 85: [2, 65] }, { 33: [2, 67], 75: [2, 67] }, { 23: [1, 123] }, { 23: [2, 51], 65: [2, 51], 72: [2, 51], 80: [2, 51], 81: [2, 51], 82: [2, 51], 83: [2, 51], 84: [2, 51], 85: [2, 51] }, { 23: [2, 53] }, { 33: [1, 124] }, { 33: [2, 91], 65: [2, 91], 72: [2, 91], 80: [2, 91], 81: [2, 91], 82: [2, 91], 83: [2, 91], 84: [2, 91], 85: [2, 91] }, { 33: [2, 93] }, { 5: [2, 22], 14: [2, 22], 15: [2, 22], 19: [2, 22], 29: [2, 22], 34: [2, 22], 39: [2, 22], 44: [2, 22], 47: [2, 22], 48: [2, 22], 51: [2, 22], 55: [2, 22], 60: [2, 22] }, { 23: [2, 99], 33: [2, 99], 54: [2, 99], 68: [2, 99], 72: [2, 99], 75: [2, 99] }, { 73: [1, 108] }, { 20: 74, 63: 125, 64: 75, 65: [1, 43], 72: [1, 35], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 5: [2, 23], 14: [2, 23], 15: [2, 23], 19: [2, 23], 29: [2, 23], 34: [2, 23], 39: [2, 23], 44: [2, 23], 47: [2, 23], 48: [2, 23], 51: [2, 23], 55: [2, 23], 60: [2, 23] }, { 47: [2, 19] }, { 47: [2, 77] }, { 20: 74, 33: [2, 72], 41: 126, 63: 127, 64: 75, 65: [1, 43], 69: 128, 70: 76, 71: 77, 72: [1, 78], 75: [2, 72], 78: 26, 79: 27, 80: [1, 28], 81: [1, 29], 82: [1, 30], 83: [1, 31], 84: [1, 32], 85: [1, 34], 86: 33 }, { 5: [2, 24], 14: [2, 24], 15: [2, 24], 19: [2, 24], 29: [2, 24], 34: [2, 24], 39: [2, 24], 44: [2, 24], 47: [2, 24], 48: [2, 24], 51: [2, 24], 55: [2, 24], 60: [2, 24] }, { 68: [1, 129] }, { 65: [2, 95], 68: [2, 95], 72: [2, 95], 80: [2, 95], 81: [2, 95], 82: [2, 95], 83: [2, 95], 84: [2, 95], 85: [2, 95] }, { 68: [2, 97] }, { 5: [2, 21], 14: [2, 21], 15: [2, 21], 19: [2, 21], 29: [2, 21], 34: [2, 21], 39: [2, 21], 44: [2, 21], 47: [2, 21], 48: [2, 21], 51: [2, 21], 55: [2, 21], 60: [2, 21] }, { 33: [1, 130] }, { 33: [2, 63] }, { 72: [1, 132], 76: 131 }, { 33: [1, 133] }, { 33: [2, 69] }, { 15: [2, 12], 18: [2, 12] }, { 14: [2, 26], 15: [2, 26], 19: [2, 26], 29: [2, 26], 34: [2, 26], 47: [2, 26], 48: [2, 26], 51: [2, 26], 55: [2, 26], 60: [2, 26] }, { 23: [2, 31], 33: [2, 31], 54: [2, 31], 68: [2, 31], 72: [2, 31], 75: [2, 31] }, { 33: [2, 74], 42: 134, 74: 135, 75: [1, 120] }, { 33: [2, 71], 65: [2, 71], 72: [2, 71], 75: [2, 71], 80: [2, 71], 81: [2, 71], 82: [2, 71], 83: [2, 71], 84: [2, 71], 85: [2, 71] }, { 33: [2, 73], 75: [2, 73] }, { 23: [2, 29], 33: [2, 29], 54: [2, 29], 65: [2, 29], 68: [2, 29], 72: [2, 29], 75: [2, 29], 80: [2, 29], 81: [2, 29], 82: [2, 29], 83: [2, 29], 84: [2, 29], 85: [2, 29] }, { 14: [2, 15], 15: [2, 15], 19: [2, 15], 29: [2, 15], 34: [2, 15], 39: [2, 15], 44: [2, 15], 47: [2, 15], 48: [2, 15], 51: [2, 15], 55: [2, 15], 60: [2, 15] }, { 72: [1, 137], 77: [1, 136] }, { 72: [2, 100], 77: [2, 100] }, { 14: [2, 16], 15: [2, 16], 19: [2, 16], 29: [2, 16], 34: [2, 16], 44: [2, 16], 47: [2, 16], 48: [2, 16], 51: [2, 16], 55: [2, 16], 60: [2, 16] }, { 33: [1, 138] }, { 33: [2, 75] }, { 33: [2, 32] }, { 72: [2, 101], 77: [2, 101] }, { 14: [2, 17], 15: [2, 17], 19: [2, 17], 29: [2, 17], 34: [2, 17], 39: [2, 17], 44: [2, 17], 47: [2, 17], 48: [2, 17], 51: [2, 17], 55: [2, 17], 60: [2, 17] }],
          defaultActions: { 4: [2, 1], 54: [2, 55], 56: [2, 20], 60: [2, 57], 73: [2, 81], 82: [2, 85], 86: [2, 18], 90: [2, 89], 101: [2, 53], 104: [2, 93], 110: [2, 19], 111: [2, 77], 116: [2, 97], 119: [2, 63], 122: [2, 69], 135: [2, 75], 136: [2, 32] },
          parseError: function parseError(str, hash) {
            throw new Error(str);
          },
          parse: function parse(input) {
            var self = this, stack = [0], vstack = [null], lstack = [], table = this.table, yytext = "", yylineno = 0, yyleng = 0, recovering = 0, TERROR = 2, EOF = 1;
            this.lexer.setInput(input);
            this.lexer.yy = this.yy;
            this.yy.lexer = this.lexer;
            this.yy.parser = this;
            if (typeof this.lexer.yylloc == "undefined") this.lexer.yylloc = {};
            var yyloc = this.lexer.yylloc;
            lstack.push(yyloc);
            var ranges = this.lexer.options && this.lexer.options.ranges;
            if (typeof this.yy.parseError === "function") this.parseError = this.yy.parseError;
            function popStack(n) {
              stack.length = stack.length - 2 * n;
              vstack.length = vstack.length - n;
              lstack.length = lstack.length - n;
            }
            function lex() {
              var token;
              token = self.lexer.lex() || 1;
              if (typeof token !== "number") {
                token = self.symbols_[token] || token;
              }
              return token;
            }
            var symbol, preErrorSymbol, state, action, a, r, yyval = {}, p, len, newState, expected;
            while (true) {
              state = stack[stack.length - 1];
              if (this.defaultActions[state]) {
                action = this.defaultActions[state];
              } else {
                if (symbol === null || typeof symbol == "undefined") {
                  symbol = lex();
                }
                action = table[state] && table[state][symbol];
              }
              if (typeof action === "undefined" || !action.length || !action[0]) {
                var errStr = "";
                if (!recovering) {
                  expected = [];
                  for (p in table[state]) if (this.terminals_[p] && p > 2) {
                    expected.push("'" + this.terminals_[p] + "'");
                  }
                  if (this.lexer.showPosition) {
                    errStr = "Parse error on line " + (yylineno + 1) + ":\n" + this.lexer.showPosition() + "\nExpecting " + expected.join(", ") + ", got '" + (this.terminals_[symbol] || symbol) + "'";
                  } else {
                    errStr = "Parse error on line " + (yylineno + 1) + ": Unexpected " + (symbol == 1 ? "end of input" : "'" + (this.terminals_[symbol] || symbol) + "'");
                  }
                  this.parseError(errStr, { text: this.lexer.match, token: this.terminals_[symbol] || symbol, line: this.lexer.yylineno, loc: yyloc, expected });
                }
              }
              if (action[0] instanceof Array && action.length > 1) {
                throw new Error("Parse Error: multiple actions possible at state: " + state + ", token: " + symbol);
              }
              switch (action[0]) {
                case 1:
                  stack.push(symbol);
                  vstack.push(this.lexer.yytext);
                  lstack.push(this.lexer.yylloc);
                  stack.push(action[1]);
                  symbol = null;
                  if (!preErrorSymbol) {
                    yyleng = this.lexer.yyleng;
                    yytext = this.lexer.yytext;
                    yylineno = this.lexer.yylineno;
                    yyloc = this.lexer.yylloc;
                    if (recovering > 0) recovering--;
                  } else {
                    symbol = preErrorSymbol;
                    preErrorSymbol = null;
                  }
                  break;
                case 2:
                  len = this.productions_[action[1]][1];
                  yyval.$ = vstack[vstack.length - len];
                  yyval._$ = { first_line: lstack[lstack.length - (len || 1)].first_line, last_line: lstack[lstack.length - 1].last_line, first_column: lstack[lstack.length - (len || 1)].first_column, last_column: lstack[lstack.length - 1].last_column };
                  if (ranges) {
                    yyval._$.range = [lstack[lstack.length - (len || 1)].range[0], lstack[lstack.length - 1].range[1]];
                  }
                  r = this.performAction.call(yyval, yytext, yyleng, yylineno, this.yy, action[1], vstack, lstack);
                  if (typeof r !== "undefined") {
                    return r;
                  }
                  if (len) {
                    stack = stack.slice(0, -1 * len * 2);
                    vstack = vstack.slice(0, -1 * len);
                    lstack = lstack.slice(0, -1 * len);
                  }
                  stack.push(this.productions_[action[1]][0]);
                  vstack.push(yyval.$);
                  lstack.push(yyval._$);
                  newState = table[stack[stack.length - 2]][stack[stack.length - 1]];
                  stack.push(newState);
                  break;
                case 3:
                  return true;
              }
            }
            return true;
          }
        };
        var lexer = function() {
          var lexer2 = {
            EOF: 1,
            parseError: function parseError(str, hash) {
              if (this.yy.parser) {
                this.yy.parser.parseError(str, hash);
              } else {
                throw new Error(str);
              }
            },
            setInput: function setInput(input) {
              this._input = input;
              this._more = this._less = this.done = false;
              this.yylineno = this.yyleng = 0;
              this.yytext = this.matched = this.match = "";
              this.conditionStack = ["INITIAL"];
              this.yylloc = { first_line: 1, first_column: 0, last_line: 1, last_column: 0 };
              if (this.options.ranges) this.yylloc.range = [0, 0];
              this.offset = 0;
              return this;
            },
            input: function input() {
              var ch = this._input[0];
              this.yytext += ch;
              this.yyleng++;
              this.offset++;
              this.match += ch;
              this.matched += ch;
              var lines = ch.match(/(?:\r\n?|\n).*/g);
              if (lines) {
                this.yylineno++;
                this.yylloc.last_line++;
              } else {
                this.yylloc.last_column++;
              }
              if (this.options.ranges) this.yylloc.range[1]++;
              this._input = this._input.slice(1);
              return ch;
            },
            unput: function unput(ch) {
              var len = ch.length;
              var lines = ch.split(/(?:\r\n?|\n)/g);
              this._input = ch + this._input;
              this.yytext = this.yytext.substr(0, this.yytext.length - len - 1);
              this.offset -= len;
              var oldLines = this.match.split(/(?:\r\n?|\n)/g);
              this.match = this.match.substr(0, this.match.length - 1);
              this.matched = this.matched.substr(0, this.matched.length - 1);
              if (lines.length - 1) this.yylineno -= lines.length - 1;
              var r = this.yylloc.range;
              this.yylloc = {
                first_line: this.yylloc.first_line,
                last_line: this.yylineno + 1,
                first_column: this.yylloc.first_column,
                last_column: lines ? (lines.length === oldLines.length ? this.yylloc.first_column : 0) + oldLines[oldLines.length - lines.length].length - lines[0].length : this.yylloc.first_column - len
              };
              if (this.options.ranges) {
                this.yylloc.range = [r[0], r[0] + this.yyleng - len];
              }
              return this;
            },
            more: function more() {
              this._more = true;
              return this;
            },
            less: function less(n) {
              this.unput(this.match.slice(n));
            },
            pastInput: function pastInput() {
              var past = this.matched.substr(0, this.matched.length - this.match.length);
              return (past.length > 20 ? "..." : "") + past.substr(-20).replace(/\n/g, "");
            },
            upcomingInput: function upcomingInput() {
              var next = this.match;
              if (next.length < 20) {
                next += this._input.substr(0, 20 - next.length);
              }
              return (next.substr(0, 20) + (next.length > 20 ? "..." : "")).replace(/\n/g, "");
            },
            showPosition: function showPosition() {
              var pre = this.pastInput();
              var c = new Array(pre.length + 1).join("-");
              return pre + this.upcomingInput() + "\n" + c + "^";
            },
            next: function next() {
              if (this.done) {
                return this.EOF;
              }
              if (!this._input) this.done = true;
              var token, match, tempMatch, index, col, lines;
              if (!this._more) {
                this.yytext = "";
                this.match = "";
              }
              var rules = this._currentRules();
              for (var i = 0; i < rules.length; i++) {
                tempMatch = this._input.match(this.rules[rules[i]]);
                if (tempMatch && (!match || tempMatch[0].length > match[0].length)) {
                  match = tempMatch;
                  index = i;
                  if (!this.options.flex) break;
                }
              }
              if (match) {
                lines = match[0].match(/(?:\r\n?|\n).*/g);
                if (lines) this.yylineno += lines.length;
                this.yylloc = {
                  first_line: this.yylloc.last_line,
                  last_line: this.yylineno + 1,
                  first_column: this.yylloc.last_column,
                  last_column: lines ? lines[lines.length - 1].length - lines[lines.length - 1].match(/\r?\n?/)[0].length : this.yylloc.last_column + match[0].length
                };
                this.yytext += match[0];
                this.match += match[0];
                this.matches = match;
                this.yyleng = this.yytext.length;
                if (this.options.ranges) {
                  this.yylloc.range = [this.offset, this.offset += this.yyleng];
                }
                this._more = false;
                this._input = this._input.slice(match[0].length);
                this.matched += match[0];
                token = this.performAction.call(this, this.yy, this, rules[index], this.conditionStack[this.conditionStack.length - 1]);
                if (this.done && this._input) this.done = false;
                if (token) return token;
                else return;
              }
              if (this._input === "") {
                return this.EOF;
              } else {
                return this.parseError("Lexical error on line " + (this.yylineno + 1) + ". Unrecognized text.\n" + this.showPosition(), { text: "", token: null, line: this.yylineno });
              }
            },
            lex: function lex() {
              var r = this.next();
              if (typeof r !== "undefined") {
                return r;
              } else {
                return this.lex();
              }
            },
            begin: function begin(condition) {
              this.conditionStack.push(condition);
            },
            popState: function popState() {
              return this.conditionStack.pop();
            },
            _currentRules: function _currentRules() {
              return this.conditions[this.conditionStack[this.conditionStack.length - 1]].rules;
            },
            topState: function topState() {
              return this.conditionStack[this.conditionStack.length - 2];
            },
            pushState: function begin(condition) {
              this.begin(condition);
            }
          };
          lexer2.options = {};
          lexer2.performAction = function anonymous(yy, yy_, $avoiding_name_collisions, YY_START) {
            function strip(start, end) {
              return yy_.yytext = yy_.yytext.substring(start, yy_.yyleng - end + start);
            }
            var YYSTATE = YY_START;
            switch ($avoiding_name_collisions) {
              case 0:
                if (yy_.yytext.slice(-2) === "\\\\") {
                  strip(0, 1);
                  this.begin("mu");
                } else if (yy_.yytext.slice(-1) === "\\") {
                  strip(0, 1);
                  this.begin("emu");
                } else {
                  this.begin("mu");
                }
                if (yy_.yytext) return 15;
                break;
              case 1:
                return 15;
                break;
              case 2:
                this.popState();
                return 15;
                break;
              case 3:
                this.begin("raw");
                return 15;
                break;
              case 4:
                this.popState();
                if (this.conditionStack[this.conditionStack.length - 1] === "raw") {
                  return 15;
                } else {
                  strip(5, 9);
                  return "END_RAW_BLOCK";
                }
                break;
              case 5:
                return 15;
                break;
              case 6:
                this.popState();
                return 14;
                break;
              case 7:
                return 65;
                break;
              case 8:
                return 68;
                break;
              case 9:
                return 19;
                break;
              case 10:
                this.popState();
                this.begin("raw");
                return 23;
                break;
              case 11:
                return 55;
                break;
              case 12:
                return 60;
                break;
              case 13:
                return 29;
                break;
              case 14:
                return 47;
                break;
              case 15:
                this.popState();
                return 44;
                break;
              case 16:
                this.popState();
                return 44;
                break;
              case 17:
                return 34;
                break;
              case 18:
                return 39;
                break;
              case 19:
                return 51;
                break;
              case 20:
                return 48;
                break;
              case 21:
                this.unput(yy_.yytext);
                this.popState();
                this.begin("com");
                break;
              case 22:
                this.popState();
                return 14;
                break;
              case 23:
                return 48;
                break;
              case 24:
                return 73;
                break;
              case 25:
                return 72;
                break;
              case 26:
                return 72;
                break;
              case 27:
                return 87;
                break;
              case 28:
                break;
              case 29:
                this.popState();
                return 54;
                break;
              case 30:
                this.popState();
                return 33;
                break;
              case 31:
                yy_.yytext = strip(1, 2).replace(/\\"/g, '"');
                return 80;
                break;
              case 32:
                yy_.yytext = strip(1, 2).replace(/\\'/g, "'");
                return 80;
                break;
              case 33:
                return 85;
                break;
              case 34:
                return 82;
                break;
              case 35:
                return 82;
                break;
              case 36:
                return 83;
                break;
              case 37:
                return 84;
                break;
              case 38:
                return 81;
                break;
              case 39:
                return 75;
                break;
              case 40:
                return 77;
                break;
              case 41:
                return 72;
                break;
              case 42:
                yy_.yytext = yy_.yytext.replace(/\\([\\\]])/g, "$1");
                return 72;
                break;
              case 43:
                return "INVALID";
                break;
              case 44:
                return 5;
                break;
            }
          };
          lexer2.rules = [/^(?:[^\x00]*?(?=(\{\{)))/, /^(?:[^\x00]+)/, /^(?:[^\x00]{2,}?(?=(\{\{|\\\{\{|\\\\\{\{|$)))/, /^(?:\{\{\{\{(?=[^/]))/, /^(?:\{\{\{\{\/[^\s!"#%-,\.\/;->@\[-\^`\{-~]+(?=[=}\s\/.])\}\}\}\})/, /^(?:[^\x00]+?(?=(\{\{\{\{)))/, /^(?:[\s\S]*?--(~)?\}\})/, /^(?:\()/, /^(?:\))/, /^(?:\{\{\{\{)/, /^(?:\}\}\}\})/, /^(?:\{\{(~)?>)/, /^(?:\{\{(~)?#>)/, /^(?:\{\{(~)?#\*?)/, /^(?:\{\{(~)?\/)/, /^(?:\{\{(~)?\^\s*(~)?\}\})/, /^(?:\{\{(~)?\s*else\s*(~)?\}\})/, /^(?:\{\{(~)?\^)/, /^(?:\{\{(~)?\s*else\b)/, /^(?:\{\{(~)?\{)/, /^(?:\{\{(~)?&)/, /^(?:\{\{(~)?!--)/, /^(?:\{\{(~)?![\s\S]*?\}\})/, /^(?:\{\{(~)?\*?)/, /^(?:=)/, /^(?:\.\.)/, /^(?:\.(?=([=~}\s\/.)|])))/, /^(?:[\/.])/, /^(?:\s+)/, /^(?:\}(~)?\}\})/, /^(?:(~)?\}\})/, /^(?:"(\\["]|[^"])*")/, /^(?:'(\\[']|[^'])*')/, /^(?:@)/, /^(?:true(?=([~}\s)])))/, /^(?:false(?=([~}\s)])))/, /^(?:undefined(?=([~}\s)])))/, /^(?:null(?=([~}\s)])))/, /^(?:-?[0-9]+(?:\.[0-9]+)?(?=([~}\s)])))/, /^(?:as\s+\|)/, /^(?:\|)/, /^(?:([^\s!"#%-,\.\/;->@\[-\^`\{-~]+(?=([=~}\s\/.)|]))))/, /^(?:\[(\\\]|[^\]])*\])/, /^(?:.)/, /^(?:$)/];
          lexer2.conditions = { "mu": { "rules": [7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44], "inclusive": false }, "emu": { "rules": [2], "inclusive": false }, "com": { "rules": [6], "inclusive": false }, "raw": { "rules": [3, 4, 5], "inclusive": false }, "INITIAL": { "rules": [0, 1, 44], "inclusive": true } };
          return lexer2;
        }();
        parser.lexer = lexer;
        function Parser() {
          this.yy = {};
        }
        Parser.prototype = parser;
        parser.Parser = Parser;
        return new Parser();
      }();
      exports["default"] = handlebars;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/visitor.js
  var require_visitor = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/visitor.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      function Visitor() {
        this.parents = [];
      }
      Visitor.prototype = {
        constructor: Visitor,
        mutating: false,
        // Visits a given value. If mutating, will replace the value if necessary.
        acceptKey: function acceptKey(node, name2) {
          var value = this.accept(node[name2]);
          if (this.mutating) {
            if (value && !Visitor.prototype[value.type]) {
              throw new _exception2["default"]('Unexpected node type "' + value.type + '" found when accepting ' + name2 + " on " + node.type);
            }
            node[name2] = value;
          }
        },
        // Performs an accept operation with added sanity check to ensure
        // required keys are not removed.
        acceptRequired: function acceptRequired(node, name2) {
          this.acceptKey(node, name2);
          if (!node[name2]) {
            throw new _exception2["default"](node.type + " requires " + name2);
          }
        },
        // Traverses a given array. If mutating, empty respnses will be removed
        // for child elements.
        acceptArray: function acceptArray(array) {
          for (var i = 0, l = array.length; i < l; i++) {
            this.acceptKey(array, i);
            if (!array[i]) {
              array.splice(i, 1);
              i--;
              l--;
            }
          }
        },
        accept: function accept(object) {
          if (!object) {
            return;
          }
          if (!this[object.type]) {
            throw new _exception2["default"]("Unknown type: " + object.type, object);
          }
          if (this.current) {
            this.parents.unshift(this.current);
          }
          this.current = object;
          var ret = this[object.type](object);
          this.current = this.parents.shift();
          if (!this.mutating || ret) {
            return ret;
          } else if (ret !== false) {
            return object;
          }
        },
        Program: function Program(program) {
          this.acceptArray(program.body);
        },
        MustacheStatement: visitSubExpression,
        Decorator: visitSubExpression,
        BlockStatement: visitBlock,
        DecoratorBlock: visitBlock,
        PartialStatement: visitPartial,
        PartialBlockStatement: function PartialBlockStatement(partial) {
          visitPartial.call(this, partial);
          this.acceptKey(partial, "program");
        },
        ContentStatement: function ContentStatement() {
        },
        CommentStatement: function CommentStatement() {
        },
        SubExpression: visitSubExpression,
        PathExpression: function PathExpression() {
        },
        StringLiteral: function StringLiteral() {
        },
        NumberLiteral: function NumberLiteral() {
        },
        BooleanLiteral: function BooleanLiteral() {
        },
        UndefinedLiteral: function UndefinedLiteral() {
        },
        NullLiteral: function NullLiteral() {
        },
        Hash: function Hash(hash) {
          this.acceptArray(hash.pairs);
        },
        HashPair: function HashPair(pair) {
          this.acceptRequired(pair, "value");
        }
      };
      function visitSubExpression(mustache) {
        this.acceptRequired(mustache, "path");
        this.acceptArray(mustache.params);
        this.acceptKey(mustache, "hash");
      }
      function visitBlock(block) {
        visitSubExpression.call(this, block);
        this.acceptKey(block, "program");
        this.acceptKey(block, "inverse");
      }
      function visitPartial(partial) {
        this.acceptRequired(partial, "name");
        this.acceptArray(partial.params);
        this.acceptKey(partial, "hash");
      }
      exports["default"] = Visitor;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/whitespace-control.js
  var require_whitespace_control = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/whitespace-control.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _visitor = require_visitor();
      var _visitor2 = _interopRequireDefault(_visitor);
      function WhitespaceControl() {
        var options = arguments.length <= 0 || arguments[0] === void 0 ? {} : arguments[0];
        this.options = options;
      }
      WhitespaceControl.prototype = new _visitor2["default"]();
      WhitespaceControl.prototype.Program = function(program) {
        var doStandalone = !this.options.ignoreStandalone;
        var isRoot = !this.isRootSeen;
        this.isRootSeen = true;
        var body = program.body;
        for (var i = 0, l = body.length; i < l; i++) {
          var current = body[i], strip = this.accept(current);
          if (!strip) {
            continue;
          }
          var _isPrevWhitespace = isPrevWhitespace(body, i, isRoot), _isNextWhitespace = isNextWhitespace(body, i, isRoot), openStandalone = strip.openStandalone && _isPrevWhitespace, closeStandalone = strip.closeStandalone && _isNextWhitespace, inlineStandalone = strip.inlineStandalone && _isPrevWhitespace && _isNextWhitespace;
          if (strip.close) {
            omitRight(body, i, true);
          }
          if (strip.open) {
            omitLeft(body, i, true);
          }
          if (doStandalone && inlineStandalone) {
            omitRight(body, i);
            if (omitLeft(body, i)) {
              if (current.type === "PartialStatement") {
                current.indent = /([ \t]+$)/.exec(body[i - 1].original)[1];
              }
            }
          }
          if (doStandalone && openStandalone) {
            omitRight((current.program || current.inverse).body);
            omitLeft(body, i);
          }
          if (doStandalone && closeStandalone) {
            omitRight(body, i);
            omitLeft((current.inverse || current.program).body);
          }
        }
        return program;
      };
      WhitespaceControl.prototype.BlockStatement = WhitespaceControl.prototype.DecoratorBlock = WhitespaceControl.prototype.PartialBlockStatement = function(block) {
        this.accept(block.program);
        this.accept(block.inverse);
        var program = block.program || block.inverse, inverse = block.program && block.inverse, firstInverse = inverse, lastInverse = inverse;
        if (inverse && inverse.chained) {
          firstInverse = inverse.body[0].program;
          while (lastInverse.chained) {
            lastInverse = lastInverse.body[lastInverse.body.length - 1].program;
          }
        }
        var strip = {
          open: block.openStrip.open,
          close: block.closeStrip.close,
          // Determine the standalone candiacy. Basically flag our content as being possibly standalone
          // so our parent can determine if we actually are standalone
          openStandalone: isNextWhitespace(program.body),
          closeStandalone: isPrevWhitespace((firstInverse || program).body)
        };
        if (block.openStrip.close) {
          omitRight(program.body, null, true);
        }
        if (inverse) {
          var inverseStrip = block.inverseStrip;
          if (inverseStrip.open) {
            omitLeft(program.body, null, true);
          }
          if (inverseStrip.close) {
            omitRight(firstInverse.body, null, true);
          }
          if (block.closeStrip.open) {
            omitLeft(lastInverse.body, null, true);
          }
          if (!this.options.ignoreStandalone && isPrevWhitespace(program.body) && isNextWhitespace(firstInverse.body)) {
            omitLeft(program.body);
            omitRight(firstInverse.body);
          }
        } else if (block.closeStrip.open) {
          omitLeft(program.body, null, true);
        }
        return strip;
      };
      WhitespaceControl.prototype.Decorator = WhitespaceControl.prototype.MustacheStatement = function(mustache) {
        return mustache.strip;
      };
      WhitespaceControl.prototype.PartialStatement = WhitespaceControl.prototype.CommentStatement = function(node) {
        var strip = node.strip || {};
        return {
          inlineStandalone: true,
          open: strip.open,
          close: strip.close
        };
      };
      function isPrevWhitespace(body, i, isRoot) {
        if (i === void 0) {
          i = body.length;
        }
        var prev = body[i - 1], sibling = body[i - 2];
        if (!prev) {
          return isRoot;
        }
        if (prev.type === "ContentStatement") {
          return (sibling || !isRoot ? /\r?\n\s*?$/ : /(^|\r?\n)\s*?$/).test(prev.original);
        }
      }
      function isNextWhitespace(body, i, isRoot) {
        if (i === void 0) {
          i = -1;
        }
        var next = body[i + 1], sibling = body[i + 2];
        if (!next) {
          return isRoot;
        }
        if (next.type === "ContentStatement") {
          return (sibling || !isRoot ? /^\s*?\r?\n/ : /^\s*?(\r?\n|$)/).test(next.original);
        }
      }
      function omitRight(body, i, multiple) {
        var current = body[i == null ? 0 : i + 1];
        if (!current || current.type !== "ContentStatement" || !multiple && current.rightStripped) {
          return;
        }
        var original = current.value;
        current.value = current.value.replace(multiple ? /^\s+/ : /^[ \t]*\r?\n?/, "");
        current.rightStripped = current.value !== original;
      }
      function omitLeft(body, i, multiple) {
        var current = body[i == null ? body.length - 1 : i - 1];
        if (!current || current.type !== "ContentStatement" || !multiple && current.leftStripped) {
          return;
        }
        var original = current.value;
        current.value = current.value.replace(multiple ? /\s+$/ : /[ \t]+$/, "");
        current.leftStripped = current.value !== original;
        return current.leftStripped;
      }
      exports["default"] = WhitespaceControl;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/helpers.js
  var require_helpers2 = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/helpers.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.SourceLocation = SourceLocation;
      exports.id = id;
      exports.stripFlags = stripFlags;
      exports.stripComment = stripComment;
      exports.preparePath = preparePath;
      exports.prepareMustache = prepareMustache;
      exports.prepareRawBlock = prepareRawBlock;
      exports.prepareBlock = prepareBlock;
      exports.prepareProgram = prepareProgram;
      exports.preparePartialBlock = preparePartialBlock;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      function validateClose(open, close) {
        close = close.path ? close.path.original : close;
        if (open.path.original !== close) {
          var errorNode = { loc: open.path.loc };
          throw new _exception2["default"](open.path.original + " doesn't match " + close, errorNode);
        }
      }
      function SourceLocation(source, locInfo) {
        this.source = source;
        this.start = {
          line: locInfo.first_line,
          column: locInfo.first_column
        };
        this.end = {
          line: locInfo.last_line,
          column: locInfo.last_column
        };
      }
      function id(token) {
        if (/^\[.*\]$/.test(token)) {
          return token.substring(1, token.length - 1);
        } else {
          return token;
        }
      }
      function stripFlags(open, close) {
        return {
          open: open.charAt(2) === "~",
          close: close.charAt(close.length - 3) === "~"
        };
      }
      function stripComment(comment) {
        return comment.replace(/^\{\{~?!-?-?/, "").replace(/-?-?~?\}\}$/, "");
      }
      function preparePath(data, parts, loc) {
        loc = this.locInfo(loc);
        var original = data ? "@" : "", dig = [], depth = 0;
        for (var i = 0, l = parts.length; i < l; i++) {
          var part = parts[i].part, isLiteral = parts[i].original !== part;
          original += (parts[i].separator || "") + part;
          if (!isLiteral && (part === ".." || part === "." || part === "this")) {
            if (dig.length > 0) {
              throw new _exception2["default"]("Invalid path: " + original, { loc });
            } else if (part === "..") {
              depth++;
            }
          } else {
            dig.push(part);
          }
        }
        return {
          type: "PathExpression",
          data,
          depth,
          parts: dig,
          original,
          loc
        };
      }
      function prepareMustache(path, params, hash, open, strip, locInfo) {
        var escapeFlag = open.charAt(3) || open.charAt(2), escaped = escapeFlag !== "{" && escapeFlag !== "&";
        var decorator = /\*/.test(open);
        return {
          type: decorator ? "Decorator" : "MustacheStatement",
          path,
          params,
          hash,
          escaped,
          strip,
          loc: this.locInfo(locInfo)
        };
      }
      function prepareRawBlock(openRawBlock, contents, close, locInfo) {
        validateClose(openRawBlock, close);
        locInfo = this.locInfo(locInfo);
        var program = {
          type: "Program",
          body: contents,
          strip: {},
          loc: locInfo
        };
        return {
          type: "BlockStatement",
          path: openRawBlock.path,
          params: openRawBlock.params,
          hash: openRawBlock.hash,
          program,
          openStrip: {},
          inverseStrip: {},
          closeStrip: {},
          loc: locInfo
        };
      }
      function prepareBlock(openBlock, program, inverseAndProgram, close, inverted, locInfo) {
        if (close && close.path) {
          validateClose(openBlock, close);
        }
        var decorator = /\*/.test(openBlock.open);
        program.blockParams = openBlock.blockParams;
        var inverse = void 0, inverseStrip = void 0;
        if (inverseAndProgram) {
          if (decorator) {
            throw new _exception2["default"]("Unexpected inverse block on decorator", inverseAndProgram);
          }
          if (inverseAndProgram.chain) {
            inverseAndProgram.program.body[0].closeStrip = close.strip;
          }
          inverseStrip = inverseAndProgram.strip;
          inverse = inverseAndProgram.program;
        }
        if (inverted) {
          inverted = inverse;
          inverse = program;
          program = inverted;
        }
        return {
          type: decorator ? "DecoratorBlock" : "BlockStatement",
          path: openBlock.path,
          params: openBlock.params,
          hash: openBlock.hash,
          program,
          inverse,
          openStrip: openBlock.strip,
          inverseStrip,
          closeStrip: close && close.strip,
          loc: this.locInfo(locInfo)
        };
      }
      function prepareProgram(statements, loc) {
        if (!loc && statements.length) {
          var firstLoc = statements[0].loc, lastLoc = statements[statements.length - 1].loc;
          if (firstLoc && lastLoc) {
            loc = {
              source: firstLoc.source,
              start: {
                line: firstLoc.start.line,
                column: firstLoc.start.column
              },
              end: {
                line: lastLoc.end.line,
                column: lastLoc.end.column
              }
            };
          }
        }
        return {
          type: "Program",
          body: statements,
          strip: {},
          loc
        };
      }
      function preparePartialBlock(open, program, close, locInfo) {
        validateClose(open, close);
        return {
          type: "PartialBlockStatement",
          name: open.path,
          params: open.params,
          hash: open.hash,
          program,
          openStrip: open.strip,
          closeStrip: close && close.strip,
          loc: this.locInfo(locInfo)
        };
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/base.js
  var require_base2 = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/base.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.parseWithoutProcessing = parseWithoutProcessing;
      exports.parse = parse;
      function _interopRequireWildcard(obj) {
        if (obj && obj.__esModule) {
          return obj;
        } else {
          var newObj = {};
          if (obj != null) {
            for (var key in obj) {
              if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key];
            }
          }
          newObj["default"] = obj;
          return newObj;
        }
      }
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _parser = require_parser();
      var _parser2 = _interopRequireDefault(_parser);
      var _whitespaceControl = require_whitespace_control();
      var _whitespaceControl2 = _interopRequireDefault(_whitespaceControl);
      var _helpers = require_helpers2();
      var Helpers = _interopRequireWildcard(_helpers);
      var _utils = require_utils();
      exports.parser = _parser2["default"];
      var yy = {};
      _utils.extend(yy, Helpers);
      function parseWithoutProcessing(input, options) {
        if (input.type === "Program") {
          return input;
        }
        _parser2["default"].yy = yy;
        yy.locInfo = function(locInfo) {
          return new yy.SourceLocation(options && options.srcName, locInfo);
        };
        var ast = _parser2["default"].parse(input);
        return ast;
      }
      function parse(input, options) {
        var ast = parseWithoutProcessing(input, options);
        var strip = new _whitespaceControl2["default"](options);
        return strip.accept(ast);
      }
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/compiler.js
  var require_compiler = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/compiler.js"(exports) {
      "use strict";
      exports.__esModule = true;
      exports.Compiler = Compiler;
      exports.precompile = precompile;
      exports.compile = compile;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      var _utils = require_utils();
      var _ast = require_ast();
      var _ast2 = _interopRequireDefault(_ast);
      var slice = [].slice;
      function Compiler() {
      }
      Compiler.prototype = {
        compiler: Compiler,
        equals: function equals(other) {
          var len = this.opcodes.length;
          if (other.opcodes.length !== len) {
            return false;
          }
          for (var i = 0; i < len; i++) {
            var opcode = this.opcodes[i], otherOpcode = other.opcodes[i];
            if (opcode.opcode !== otherOpcode.opcode || !argEquals(opcode.args, otherOpcode.args)) {
              return false;
            }
          }
          len = this.children.length;
          for (var i = 0; i < len; i++) {
            if (!this.children[i].equals(other.children[i])) {
              return false;
            }
          }
          return true;
        },
        guid: 0,
        compile: function compile2(program, options) {
          this.sourceNode = [];
          this.opcodes = [];
          this.children = [];
          this.options = options;
          this.stringParams = options.stringParams;
          this.trackIds = options.trackIds;
          options.blockParams = options.blockParams || [];
          options.knownHelpers = _utils.extend(/* @__PURE__ */ Object.create(null), {
            helperMissing: true,
            blockHelperMissing: true,
            each: true,
            "if": true,
            unless: true,
            "with": true,
            log: true,
            lookup: true
          }, options.knownHelpers);
          return this.accept(program);
        },
        compileProgram: function compileProgram(program) {
          var childCompiler = new this.compiler(), result = childCompiler.compile(program, this.options), guid = this.guid++;
          this.usePartial = this.usePartial || result.usePartial;
          this.children[guid] = result;
          this.useDepths = this.useDepths || result.useDepths;
          return guid;
        },
        accept: function accept(node) {
          if (!this[node.type]) {
            throw new _exception2["default"]("Unknown type: " + node.type, node);
          }
          this.sourceNode.unshift(node);
          var ret = this[node.type](node);
          this.sourceNode.shift();
          return ret;
        },
        Program: function Program(program) {
          this.options.blockParams.unshift(program.blockParams);
          var body = program.body, bodyLength = body.length;
          for (var i = 0; i < bodyLength; i++) {
            this.accept(body[i]);
          }
          this.options.blockParams.shift();
          this.isSimple = bodyLength === 1;
          this.blockParams = program.blockParams ? program.blockParams.length : 0;
          return this;
        },
        BlockStatement: function BlockStatement(block) {
          transformLiteralToPath(block);
          var program = block.program, inverse = block.inverse;
          program = program && this.compileProgram(program);
          inverse = inverse && this.compileProgram(inverse);
          var type = this.classifySexpr(block);
          if (type === "helper") {
            this.helperSexpr(block, program, inverse);
          } else if (type === "simple") {
            this.simpleSexpr(block);
            this.opcode("pushProgram", program);
            this.opcode("pushProgram", inverse);
            this.opcode("emptyHash");
            this.opcode("blockValue", block.path.original);
          } else {
            this.ambiguousSexpr(block, program, inverse);
            this.opcode("pushProgram", program);
            this.opcode("pushProgram", inverse);
            this.opcode("emptyHash");
            this.opcode("ambiguousBlockValue");
          }
          this.opcode("append");
        },
        DecoratorBlock: function DecoratorBlock(decorator) {
          var program = decorator.program && this.compileProgram(decorator.program);
          var params = this.setupFullMustacheParams(decorator, program, void 0), path = decorator.path;
          this.useDecorators = true;
          this.opcode("registerDecorator", params.length, path.original);
        },
        PartialStatement: function PartialStatement(partial) {
          this.usePartial = true;
          var program = partial.program;
          if (program) {
            program = this.compileProgram(partial.program);
          }
          var params = partial.params;
          if (params.length > 1) {
            throw new _exception2["default"]("Unsupported number of partial arguments: " + params.length, partial);
          } else if (!params.length) {
            if (this.options.explicitPartialContext) {
              this.opcode("pushLiteral", "undefined");
            } else {
              params.push({ type: "PathExpression", parts: [], depth: 0 });
            }
          }
          var partialName = partial.name.original, isDynamic = partial.name.type === "SubExpression";
          if (isDynamic) {
            this.accept(partial.name);
          }
          this.setupFullMustacheParams(partial, program, void 0, true);
          var indent = partial.indent || "";
          if (this.options.preventIndent && indent) {
            this.opcode("appendContent", indent);
            indent = "";
          }
          this.opcode("invokePartial", isDynamic, partialName, indent);
          this.opcode("append");
        },
        PartialBlockStatement: function PartialBlockStatement(partialBlock) {
          this.PartialStatement(partialBlock);
        },
        MustacheStatement: function MustacheStatement(mustache) {
          this.SubExpression(mustache);
          if (mustache.escaped && !this.options.noEscape) {
            this.opcode("appendEscaped");
          } else {
            this.opcode("append");
          }
        },
        Decorator: function Decorator(decorator) {
          this.DecoratorBlock(decorator);
        },
        ContentStatement: function ContentStatement(content) {
          if (content.value) {
            this.opcode("appendContent", content.value);
          }
        },
        CommentStatement: function CommentStatement() {
        },
        SubExpression: function SubExpression(sexpr) {
          transformLiteralToPath(sexpr);
          var type = this.classifySexpr(sexpr);
          if (type === "simple") {
            this.simpleSexpr(sexpr);
          } else if (type === "helper") {
            this.helperSexpr(sexpr);
          } else {
            this.ambiguousSexpr(sexpr);
          }
        },
        ambiguousSexpr: function ambiguousSexpr(sexpr, program, inverse) {
          var path = sexpr.path, name2 = path.parts[0], isBlock = program != null || inverse != null;
          this.opcode("getContext", path.depth);
          this.opcode("pushProgram", program);
          this.opcode("pushProgram", inverse);
          path.strict = true;
          this.accept(path);
          this.opcode("invokeAmbiguous", name2, isBlock);
        },
        simpleSexpr: function simpleSexpr(sexpr) {
          var path = sexpr.path;
          path.strict = true;
          this.accept(path);
          this.opcode("resolvePossibleLambda");
        },
        helperSexpr: function helperSexpr(sexpr, program, inverse) {
          var params = this.setupFullMustacheParams(sexpr, program, inverse), path = sexpr.path, name2 = path.parts[0];
          if (this.options.knownHelpers[name2]) {
            this.opcode("invokeKnownHelper", params.length, name2);
          } else if (this.options.knownHelpersOnly) {
            throw new _exception2["default"]("You specified knownHelpersOnly, but used the unknown helper " + name2, sexpr);
          } else {
            path.strict = true;
            path.falsy = true;
            this.accept(path);
            this.opcode("invokeHelper", params.length, path.original, _ast2["default"].helpers.simpleId(path));
          }
        },
        PathExpression: function PathExpression(path) {
          this.addDepth(path.depth);
          this.opcode("getContext", path.depth);
          var name2 = path.parts[0], scoped = _ast2["default"].helpers.scopedId(path), blockParamId = !path.depth && !scoped && this.blockParamIndex(name2);
          if (blockParamId) {
            this.opcode("lookupBlockParam", blockParamId, path.parts);
          } else if (!name2) {
            this.opcode("pushContext");
          } else if (path.data) {
            this.options.data = true;
            this.opcode("lookupData", path.depth, path.parts, path.strict);
          } else {
            this.opcode("lookupOnContext", path.parts, path.falsy, path.strict, scoped);
          }
        },
        StringLiteral: function StringLiteral(string) {
          this.opcode("pushString", string.value);
        },
        NumberLiteral: function NumberLiteral(number) {
          this.opcode("pushLiteral", number.value);
        },
        BooleanLiteral: function BooleanLiteral(bool) {
          this.opcode("pushLiteral", bool.value);
        },
        UndefinedLiteral: function UndefinedLiteral() {
          this.opcode("pushLiteral", "undefined");
        },
        NullLiteral: function NullLiteral() {
          this.opcode("pushLiteral", "null");
        },
        Hash: function Hash(hash) {
          var pairs = hash.pairs, i = 0, l = pairs.length;
          this.opcode("pushHash");
          for (; i < l; i++) {
            this.pushParam(pairs[i].value);
          }
          while (i--) {
            this.opcode("assignToHash", pairs[i].key);
          }
          this.opcode("popHash");
        },
        // HELPERS
        opcode: function opcode(name2) {
          this.opcodes.push({
            opcode: name2,
            args: slice.call(arguments, 1),
            loc: this.sourceNode[0].loc
          });
        },
        addDepth: function addDepth(depth) {
          if (!depth) {
            return;
          }
          this.useDepths = true;
        },
        classifySexpr: function classifySexpr(sexpr) {
          var isSimple = _ast2["default"].helpers.simpleId(sexpr.path);
          var isBlockParam = isSimple && !!this.blockParamIndex(sexpr.path.parts[0]);
          var isHelper = !isBlockParam && _ast2["default"].helpers.helperExpression(sexpr);
          var isEligible = !isBlockParam && (isHelper || isSimple);
          if (isEligible && !isHelper) {
            var _name = sexpr.path.parts[0], options = this.options;
            if (options.knownHelpers[_name]) {
              isHelper = true;
            } else if (options.knownHelpersOnly) {
              isEligible = false;
            }
          }
          if (isHelper) {
            return "helper";
          } else if (isEligible) {
            return "ambiguous";
          } else {
            return "simple";
          }
        },
        pushParams: function pushParams(params) {
          for (var i = 0, l = params.length; i < l; i++) {
            this.pushParam(params[i]);
          }
        },
        pushParam: function pushParam(val) {
          var value = val.value != null ? val.value : val.original || "";
          if (this.stringParams) {
            if (value.replace) {
              value = value.replace(/^(\.?\.\/)*/g, "").replace(/\//g, ".");
            }
            if (val.depth) {
              this.addDepth(val.depth);
            }
            this.opcode("getContext", val.depth || 0);
            this.opcode("pushStringParam", value, val.type);
            if (val.type === "SubExpression") {
              this.accept(val);
            }
          } else {
            if (this.trackIds) {
              var blockParamIndex = void 0;
              if (val.parts && !_ast2["default"].helpers.scopedId(val) && !val.depth) {
                blockParamIndex = this.blockParamIndex(val.parts[0]);
              }
              if (blockParamIndex) {
                var blockParamChild = val.parts.slice(1).join(".");
                this.opcode("pushId", "BlockParam", blockParamIndex, blockParamChild);
              } else {
                value = val.original || value;
                if (value.replace) {
                  value = value.replace(/^this(?:\.|$)/, "").replace(/^\.\//, "").replace(/^\.$/, "");
                }
                this.opcode("pushId", val.type, value);
              }
            }
            this.accept(val);
          }
        },
        setupFullMustacheParams: function setupFullMustacheParams(sexpr, program, inverse, omitEmpty) {
          var params = sexpr.params;
          this.pushParams(params);
          this.opcode("pushProgram", program);
          this.opcode("pushProgram", inverse);
          if (sexpr.hash) {
            this.accept(sexpr.hash);
          } else {
            this.opcode("emptyHash", omitEmpty);
          }
          return params;
        },
        blockParamIndex: function blockParamIndex(name2) {
          for (var depth = 0, len = this.options.blockParams.length; depth < len; depth++) {
            var blockParams = this.options.blockParams[depth], param = blockParams && _utils.indexOf(blockParams, name2);
            if (blockParams && param >= 0) {
              return [depth, param];
            }
          }
        }
      };
      function precompile(input, options, env) {
        if (input == null || typeof input !== "string" && input.type !== "Program") {
          throw new _exception2["default"]("You must pass a string or Handlebars AST to Handlebars.precompile. You passed " + input);
        }
        options = options || {};
        if (!("data" in options)) {
          options.data = true;
        }
        if (options.compat) {
          options.useDepths = true;
        }
        var ast = env.parse(input, options), environment = new env.Compiler().compile(ast, options);
        return new env.JavaScriptCompiler().compile(environment, options);
      }
      function compile(input, options, env) {
        if (options === void 0) options = {};
        if (input == null || typeof input !== "string" && input.type !== "Program") {
          throw new _exception2["default"]("You must pass a string or Handlebars AST to Handlebars.compile. You passed " + input);
        }
        options = _utils.extend({}, options);
        if (!("data" in options)) {
          options.data = true;
        }
        if (options.compat) {
          options.useDepths = true;
        }
        var compiled = void 0;
        function compileInput() {
          var ast = env.parse(input, options), environment = new env.Compiler().compile(ast, options), templateSpec = new env.JavaScriptCompiler().compile(environment, options, void 0, true);
          return env.template(templateSpec);
        }
        function ret(context, execOptions) {
          if (!compiled) {
            compiled = compileInput();
          }
          return compiled.call(this, context, execOptions);
        }
        ret._setup = function(setupOptions) {
          if (!compiled) {
            compiled = compileInput();
          }
          return compiled._setup(setupOptions);
        };
        ret._child = function(i, data, blockParams, depths) {
          if (!compiled) {
            compiled = compileInput();
          }
          return compiled._child(i, data, blockParams, depths);
        };
        return ret;
      }
      function argEquals(a, b) {
        if (a === b) {
          return true;
        }
        if (_utils.isArray(a) && _utils.isArray(b) && a.length === b.length) {
          for (var i = 0; i < a.length; i++) {
            if (!argEquals(a[i], b[i])) {
              return false;
            }
          }
          return true;
        }
      }
      function transformLiteralToPath(sexpr) {
        if (!sexpr.path.parts) {
          var literal = sexpr.path;
          sexpr.path = {
            type: "PathExpression",
            data: false,
            depth: 0,
            parts: [literal.original + ""],
            original: literal.original + "",
            loc: literal.loc
          };
        }
      }
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/base64.js
  var require_base64 = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/base64.js"(exports) {
      "use strict";
      var intToCharMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".split("");
      exports.encode = function(number) {
        if (0 <= number && number < intToCharMap.length) {
          return intToCharMap[number];
        }
        throw new TypeError("Must be between 0 and 63: " + number);
      };
      exports.decode = function(charCode) {
        var bigA = 65;
        var bigZ = 90;
        var littleA = 97;
        var littleZ = 122;
        var zero = 48;
        var nine = 57;
        var plus = 43;
        var slash = 47;
        var littleOffset = 26;
        var numberOffset = 52;
        if (bigA <= charCode && charCode <= bigZ) {
          return charCode - bigA;
        }
        if (littleA <= charCode && charCode <= littleZ) {
          return charCode - littleA + littleOffset;
        }
        if (zero <= charCode && charCode <= nine) {
          return charCode - zero + numberOffset;
        }
        if (charCode == plus) {
          return 62;
        }
        if (charCode == slash) {
          return 63;
        }
        return -1;
      };
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/base64-vlq.js
  var require_base64_vlq = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/base64-vlq.js"(exports) {
      "use strict";
      var base64 = require_base64();
      var VLQ_BASE_SHIFT = 5;
      var VLQ_BASE = 1 << VLQ_BASE_SHIFT;
      var VLQ_BASE_MASK = VLQ_BASE - 1;
      var VLQ_CONTINUATION_BIT = VLQ_BASE;
      function toVLQSigned(aValue) {
        return aValue < 0 ? (-aValue << 1) + 1 : (aValue << 1) + 0;
      }
      function fromVLQSigned(aValue) {
        var isNegative = (aValue & 1) === 1;
        var shifted = aValue >> 1;
        return isNegative ? -shifted : shifted;
      }
      exports.encode = function base64VLQ_encode(aValue) {
        var encoded = "";
        var digit;
        var vlq = toVLQSigned(aValue);
        do {
          digit = vlq & VLQ_BASE_MASK;
          vlq >>>= VLQ_BASE_SHIFT;
          if (vlq > 0) {
            digit |= VLQ_CONTINUATION_BIT;
          }
          encoded += base64.encode(digit);
        } while (vlq > 0);
        return encoded;
      };
      exports.decode = function base64VLQ_decode(aStr, aIndex, aOutParam) {
        var strLen = aStr.length;
        var result = 0;
        var shift = 0;
        var continuation, digit;
        do {
          if (aIndex >= strLen) {
            throw new Error("Expected more digits in base 64 VLQ value.");
          }
          digit = base64.decode(aStr.charCodeAt(aIndex++));
          if (digit === -1) {
            throw new Error("Invalid base64 digit: " + aStr.charAt(aIndex - 1));
          }
          continuation = !!(digit & VLQ_CONTINUATION_BIT);
          digit &= VLQ_BASE_MASK;
          result = result + (digit << shift);
          shift += VLQ_BASE_SHIFT;
        } while (continuation);
        aOutParam.value = fromVLQSigned(result);
        aOutParam.rest = aIndex;
      };
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/util.js
  var require_util = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/util.js"(exports) {
      "use strict";
      function getArg(aArgs, aName, aDefaultValue) {
        if (aName in aArgs) {
          return aArgs[aName];
        } else if (arguments.length === 3) {
          return aDefaultValue;
        } else {
          throw new Error('"' + aName + '" is a required argument.');
        }
      }
      exports.getArg = getArg;
      var urlRegexp = /^(?:([\w+\-.]+):)?\/\/(?:(\w+:\w+)@)?([\w.-]*)(?::(\d+))?(.*)$/;
      var dataUrlRegexp = /^data:.+\,.+$/;
      function urlParse(aUrl) {
        var match = aUrl.match(urlRegexp);
        if (!match) {
          return null;
        }
        return {
          scheme: match[1],
          auth: match[2],
          host: match[3],
          port: match[4],
          path: match[5]
        };
      }
      exports.urlParse = urlParse;
      function urlGenerate(aParsedUrl) {
        var url = "";
        if (aParsedUrl.scheme) {
          url += aParsedUrl.scheme + ":";
        }
        url += "//";
        if (aParsedUrl.auth) {
          url += aParsedUrl.auth + "@";
        }
        if (aParsedUrl.host) {
          url += aParsedUrl.host;
        }
        if (aParsedUrl.port) {
          url += ":" + aParsedUrl.port;
        }
        if (aParsedUrl.path) {
          url += aParsedUrl.path;
        }
        return url;
      }
      exports.urlGenerate = urlGenerate;
      function normalize(aPath) {
        var path = aPath;
        var url = urlParse(aPath);
        if (url) {
          if (!url.path) {
            return aPath;
          }
          path = url.path;
        }
        var isAbsolute = exports.isAbsolute(path);
        var parts = path.split(/\/+/);
        for (var part, up = 0, i = parts.length - 1; i >= 0; i--) {
          part = parts[i];
          if (part === ".") {
            parts.splice(i, 1);
          } else if (part === "..") {
            up++;
          } else if (up > 0) {
            if (part === "") {
              parts.splice(i + 1, up);
              up = 0;
            } else {
              parts.splice(i, 2);
              up--;
            }
          }
        }
        path = parts.join("/");
        if (path === "") {
          path = isAbsolute ? "/" : ".";
        }
        if (url) {
          url.path = path;
          return urlGenerate(url);
        }
        return path;
      }
      exports.normalize = normalize;
      function join(aRoot, aPath) {
        if (aRoot === "") {
          aRoot = ".";
        }
        if (aPath === "") {
          aPath = ".";
        }
        var aPathUrl = urlParse(aPath);
        var aRootUrl = urlParse(aRoot);
        if (aRootUrl) {
          aRoot = aRootUrl.path || "/";
        }
        if (aPathUrl && !aPathUrl.scheme) {
          if (aRootUrl) {
            aPathUrl.scheme = aRootUrl.scheme;
          }
          return urlGenerate(aPathUrl);
        }
        if (aPathUrl || aPath.match(dataUrlRegexp)) {
          return aPath;
        }
        if (aRootUrl && !aRootUrl.host && !aRootUrl.path) {
          aRootUrl.host = aPath;
          return urlGenerate(aRootUrl);
        }
        var joined = aPath.charAt(0) === "/" ? aPath : normalize(aRoot.replace(/\/+$/, "") + "/" + aPath);
        if (aRootUrl) {
          aRootUrl.path = joined;
          return urlGenerate(aRootUrl);
        }
        return joined;
      }
      exports.join = join;
      exports.isAbsolute = function(aPath) {
        return aPath.charAt(0) === "/" || urlRegexp.test(aPath);
      };
      function relative(aRoot, aPath) {
        if (aRoot === "") {
          aRoot = ".";
        }
        aRoot = aRoot.replace(/\/$/, "");
        var level = 0;
        while (aPath.indexOf(aRoot + "/") !== 0) {
          var index = aRoot.lastIndexOf("/");
          if (index < 0) {
            return aPath;
          }
          aRoot = aRoot.slice(0, index);
          if (aRoot.match(/^([^\/]+:\/)?\/*$/)) {
            return aPath;
          }
          ++level;
        }
        return Array(level + 1).join("../") + aPath.substr(aRoot.length + 1);
      }
      exports.relative = relative;
      var supportsNullProto = function() {
        var obj = /* @__PURE__ */ Object.create(null);
        return !("__proto__" in obj);
      }();
      function identity(s) {
        return s;
      }
      function toSetString(aStr) {
        if (isProtoString(aStr)) {
          return "$" + aStr;
        }
        return aStr;
      }
      exports.toSetString = supportsNullProto ? identity : toSetString;
      function fromSetString(aStr) {
        if (isProtoString(aStr)) {
          return aStr.slice(1);
        }
        return aStr;
      }
      exports.fromSetString = supportsNullProto ? identity : fromSetString;
      function isProtoString(s) {
        if (!s) {
          return false;
        }
        var length = s.length;
        if (length < 9) {
          return false;
        }
        if (s.charCodeAt(length - 1) !== 95 || s.charCodeAt(length - 2) !== 95 || s.charCodeAt(length - 3) !== 111 || s.charCodeAt(length - 4) !== 116 || s.charCodeAt(length - 5) !== 111 || s.charCodeAt(length - 6) !== 114 || s.charCodeAt(length - 7) !== 112 || s.charCodeAt(length - 8) !== 95 || s.charCodeAt(length - 9) !== 95) {
          return false;
        }
        for (var i = length - 10; i >= 0; i--) {
          if (s.charCodeAt(i) !== 36) {
            return false;
          }
        }
        return true;
      }
      function compareByOriginalPositions(mappingA, mappingB, onlyCompareOriginal) {
        var cmp = strcmp(mappingA.source, mappingB.source);
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.originalLine - mappingB.originalLine;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.originalColumn - mappingB.originalColumn;
        if (cmp !== 0 || onlyCompareOriginal) {
          return cmp;
        }
        cmp = mappingA.generatedColumn - mappingB.generatedColumn;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.generatedLine - mappingB.generatedLine;
        if (cmp !== 0) {
          return cmp;
        }
        return strcmp(mappingA.name, mappingB.name);
      }
      exports.compareByOriginalPositions = compareByOriginalPositions;
      function compareByGeneratedPositionsDeflated(mappingA, mappingB, onlyCompareGenerated) {
        var cmp = mappingA.generatedLine - mappingB.generatedLine;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.generatedColumn - mappingB.generatedColumn;
        if (cmp !== 0 || onlyCompareGenerated) {
          return cmp;
        }
        cmp = strcmp(mappingA.source, mappingB.source);
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.originalLine - mappingB.originalLine;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.originalColumn - mappingB.originalColumn;
        if (cmp !== 0) {
          return cmp;
        }
        return strcmp(mappingA.name, mappingB.name);
      }
      exports.compareByGeneratedPositionsDeflated = compareByGeneratedPositionsDeflated;
      function strcmp(aStr1, aStr2) {
        if (aStr1 === aStr2) {
          return 0;
        }
        if (aStr1 === null) {
          return 1;
        }
        if (aStr2 === null) {
          return -1;
        }
        if (aStr1 > aStr2) {
          return 1;
        }
        return -1;
      }
      function compareByGeneratedPositionsInflated(mappingA, mappingB) {
        var cmp = mappingA.generatedLine - mappingB.generatedLine;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.generatedColumn - mappingB.generatedColumn;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = strcmp(mappingA.source, mappingB.source);
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.originalLine - mappingB.originalLine;
        if (cmp !== 0) {
          return cmp;
        }
        cmp = mappingA.originalColumn - mappingB.originalColumn;
        if (cmp !== 0) {
          return cmp;
        }
        return strcmp(mappingA.name, mappingB.name);
      }
      exports.compareByGeneratedPositionsInflated = compareByGeneratedPositionsInflated;
      function parseSourceMapInput(str) {
        return JSON.parse(str.replace(/^\)]}'[^\n]*\n/, ""));
      }
      exports.parseSourceMapInput = parseSourceMapInput;
      function computeSourceURL(sourceRoot, sourceURL, sourceMapURL) {
        sourceURL = sourceURL || "";
        if (sourceRoot) {
          if (sourceRoot[sourceRoot.length - 1] !== "/" && sourceURL[0] !== "/") {
            sourceRoot += "/";
          }
          sourceURL = sourceRoot + sourceURL;
        }
        if (sourceMapURL) {
          var parsed = urlParse(sourceMapURL);
          if (!parsed) {
            throw new Error("sourceMapURL could not be parsed");
          }
          if (parsed.path) {
            var index = parsed.path.lastIndexOf("/");
            if (index >= 0) {
              parsed.path = parsed.path.substring(0, index + 1);
            }
          }
          sourceURL = join(urlGenerate(parsed), sourceURL);
        }
        return normalize(sourceURL);
      }
      exports.computeSourceURL = computeSourceURL;
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/array-set.js
  var require_array_set = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/array-set.js"(exports) {
      "use strict";
      var util2 = require_util();
      var has = Object.prototype.hasOwnProperty;
      var hasNativeMap = typeof Map !== "undefined";
      function ArraySet() {
        this._array = [];
        this._set = hasNativeMap ? /* @__PURE__ */ new Map() : /* @__PURE__ */ Object.create(null);
      }
      ArraySet.fromArray = function ArraySet_fromArray(aArray, aAllowDuplicates) {
        var set = new ArraySet();
        for (var i = 0, len = aArray.length; i < len; i++) {
          set.add(aArray[i], aAllowDuplicates);
        }
        return set;
      };
      ArraySet.prototype.size = function ArraySet_size() {
        return hasNativeMap ? this._set.size : Object.getOwnPropertyNames(this._set).length;
      };
      ArraySet.prototype.add = function ArraySet_add(aStr, aAllowDuplicates) {
        var sStr = hasNativeMap ? aStr : util2.toSetString(aStr);
        var isDuplicate = hasNativeMap ? this.has(aStr) : has.call(this._set, sStr);
        var idx = this._array.length;
        if (!isDuplicate || aAllowDuplicates) {
          this._array.push(aStr);
        }
        if (!isDuplicate) {
          if (hasNativeMap) {
            this._set.set(aStr, idx);
          } else {
            this._set[sStr] = idx;
          }
        }
      };
      ArraySet.prototype.has = function ArraySet_has(aStr) {
        if (hasNativeMap) {
          return this._set.has(aStr);
        } else {
          var sStr = util2.toSetString(aStr);
          return has.call(this._set, sStr);
        }
      };
      ArraySet.prototype.indexOf = function ArraySet_indexOf(aStr) {
        if (hasNativeMap) {
          var idx = this._set.get(aStr);
          if (idx >= 0) {
            return idx;
          }
        } else {
          var sStr = util2.toSetString(aStr);
          if (has.call(this._set, sStr)) {
            return this._set[sStr];
          }
        }
        throw new Error('"' + aStr + '" is not in the set.');
      };
      ArraySet.prototype.at = function ArraySet_at(aIdx) {
        if (aIdx >= 0 && aIdx < this._array.length) {
          return this._array[aIdx];
        }
        throw new Error("No element indexed by " + aIdx);
      };
      ArraySet.prototype.toArray = function ArraySet_toArray() {
        return this._array.slice();
      };
      exports.ArraySet = ArraySet;
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/mapping-list.js
  var require_mapping_list = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/mapping-list.js"(exports) {
      "use strict";
      var util2 = require_util();
      function generatedPositionAfter(mappingA, mappingB) {
        var lineA = mappingA.generatedLine;
        var lineB = mappingB.generatedLine;
        var columnA = mappingA.generatedColumn;
        var columnB = mappingB.generatedColumn;
        return lineB > lineA || lineB == lineA && columnB >= columnA || util2.compareByGeneratedPositionsInflated(mappingA, mappingB) <= 0;
      }
      function MappingList() {
        this._array = [];
        this._sorted = true;
        this._last = { generatedLine: -1, generatedColumn: 0 };
      }
      MappingList.prototype.unsortedForEach = function MappingList_forEach(aCallback, aThisArg) {
        this._array.forEach(aCallback, aThisArg);
      };
      MappingList.prototype.add = function MappingList_add(aMapping) {
        if (generatedPositionAfter(this._last, aMapping)) {
          this._last = aMapping;
          this._array.push(aMapping);
        } else {
          this._sorted = false;
          this._array.push(aMapping);
        }
      };
      MappingList.prototype.toArray = function MappingList_toArray() {
        if (!this._sorted) {
          this._array.sort(util2.compareByGeneratedPositionsInflated);
          this._sorted = true;
        }
        return this._array;
      };
      exports.MappingList = MappingList;
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/source-map-generator.js
  var require_source_map_generator = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/source-map-generator.js"(exports) {
      "use strict";
      var base64VLQ = require_base64_vlq();
      var util2 = require_util();
      var ArraySet = require_array_set().ArraySet;
      var MappingList = require_mapping_list().MappingList;
      function SourceMapGenerator(aArgs) {
        if (!aArgs) {
          aArgs = {};
        }
        this._file = util2.getArg(aArgs, "file", null);
        this._sourceRoot = util2.getArg(aArgs, "sourceRoot", null);
        this._skipValidation = util2.getArg(aArgs, "skipValidation", false);
        this._sources = new ArraySet();
        this._names = new ArraySet();
        this._mappings = new MappingList();
        this._sourcesContents = null;
      }
      SourceMapGenerator.prototype._version = 3;
      SourceMapGenerator.fromSourceMap = function SourceMapGenerator_fromSourceMap(aSourceMapConsumer) {
        var sourceRoot = aSourceMapConsumer.sourceRoot;
        var generator = new SourceMapGenerator({
          file: aSourceMapConsumer.file,
          sourceRoot
        });
        aSourceMapConsumer.eachMapping(function(mapping) {
          var newMapping = {
            generated: {
              line: mapping.generatedLine,
              column: mapping.generatedColumn
            }
          };
          if (mapping.source != null) {
            newMapping.source = mapping.source;
            if (sourceRoot != null) {
              newMapping.source = util2.relative(sourceRoot, newMapping.source);
            }
            newMapping.original = {
              line: mapping.originalLine,
              column: mapping.originalColumn
            };
            if (mapping.name != null) {
              newMapping.name = mapping.name;
            }
          }
          generator.addMapping(newMapping);
        });
        aSourceMapConsumer.sources.forEach(function(sourceFile) {
          var sourceRelative = sourceFile;
          if (sourceRoot !== null) {
            sourceRelative = util2.relative(sourceRoot, sourceFile);
          }
          if (!generator._sources.has(sourceRelative)) {
            generator._sources.add(sourceRelative);
          }
          var content = aSourceMapConsumer.sourceContentFor(sourceFile);
          if (content != null) {
            generator.setSourceContent(sourceFile, content);
          }
        });
        return generator;
      };
      SourceMapGenerator.prototype.addMapping = function SourceMapGenerator_addMapping(aArgs) {
        var generated = util2.getArg(aArgs, "generated");
        var original = util2.getArg(aArgs, "original", null);
        var source = util2.getArg(aArgs, "source", null);
        var name2 = util2.getArg(aArgs, "name", null);
        if (!this._skipValidation) {
          this._validateMapping(generated, original, source, name2);
        }
        if (source != null) {
          source = String(source);
          if (!this._sources.has(source)) {
            this._sources.add(source);
          }
        }
        if (name2 != null) {
          name2 = String(name2);
          if (!this._names.has(name2)) {
            this._names.add(name2);
          }
        }
        this._mappings.add({
          generatedLine: generated.line,
          generatedColumn: generated.column,
          originalLine: original != null && original.line,
          originalColumn: original != null && original.column,
          source,
          name: name2
        });
      };
      SourceMapGenerator.prototype.setSourceContent = function SourceMapGenerator_setSourceContent(aSourceFile, aSourceContent) {
        var source = aSourceFile;
        if (this._sourceRoot != null) {
          source = util2.relative(this._sourceRoot, source);
        }
        if (aSourceContent != null) {
          if (!this._sourcesContents) {
            this._sourcesContents = /* @__PURE__ */ Object.create(null);
          }
          this._sourcesContents[util2.toSetString(source)] = aSourceContent;
        } else if (this._sourcesContents) {
          delete this._sourcesContents[util2.toSetString(source)];
          if (Object.keys(this._sourcesContents).length === 0) {
            this._sourcesContents = null;
          }
        }
      };
      SourceMapGenerator.prototype.applySourceMap = function SourceMapGenerator_applySourceMap(aSourceMapConsumer, aSourceFile, aSourceMapPath) {
        var sourceFile = aSourceFile;
        if (aSourceFile == null) {
          if (aSourceMapConsumer.file == null) {
            throw new Error(
              `SourceMapGenerator.prototype.applySourceMap requires either an explicit source file, or the source map's "file" property. Both were omitted.`
            );
          }
          sourceFile = aSourceMapConsumer.file;
        }
        var sourceRoot = this._sourceRoot;
        if (sourceRoot != null) {
          sourceFile = util2.relative(sourceRoot, sourceFile);
        }
        var newSources = new ArraySet();
        var newNames = new ArraySet();
        this._mappings.unsortedForEach(function(mapping) {
          if (mapping.source === sourceFile && mapping.originalLine != null) {
            var original = aSourceMapConsumer.originalPositionFor({
              line: mapping.originalLine,
              column: mapping.originalColumn
            });
            if (original.source != null) {
              mapping.source = original.source;
              if (aSourceMapPath != null) {
                mapping.source = util2.join(aSourceMapPath, mapping.source);
              }
              if (sourceRoot != null) {
                mapping.source = util2.relative(sourceRoot, mapping.source);
              }
              mapping.originalLine = original.line;
              mapping.originalColumn = original.column;
              if (original.name != null) {
                mapping.name = original.name;
              }
            }
          }
          var source = mapping.source;
          if (source != null && !newSources.has(source)) {
            newSources.add(source);
          }
          var name2 = mapping.name;
          if (name2 != null && !newNames.has(name2)) {
            newNames.add(name2);
          }
        }, this);
        this._sources = newSources;
        this._names = newNames;
        aSourceMapConsumer.sources.forEach(function(sourceFile2) {
          var content = aSourceMapConsumer.sourceContentFor(sourceFile2);
          if (content != null) {
            if (aSourceMapPath != null) {
              sourceFile2 = util2.join(aSourceMapPath, sourceFile2);
            }
            if (sourceRoot != null) {
              sourceFile2 = util2.relative(sourceRoot, sourceFile2);
            }
            this.setSourceContent(sourceFile2, content);
          }
        }, this);
      };
      SourceMapGenerator.prototype._validateMapping = function SourceMapGenerator_validateMapping(aGenerated, aOriginal, aSource, aName) {
        if (aOriginal && typeof aOriginal.line !== "number" && typeof aOriginal.column !== "number") {
          throw new Error(
            "original.line and original.column are not numbers -- you probably meant to omit the original mapping entirely and only map the generated position. If so, pass null for the original mapping instead of an object with empty or null values."
          );
        }
        if (aGenerated && "line" in aGenerated && "column" in aGenerated && aGenerated.line > 0 && aGenerated.column >= 0 && !aOriginal && !aSource && !aName) {
          return;
        } else if (aGenerated && "line" in aGenerated && "column" in aGenerated && aOriginal && "line" in aOriginal && "column" in aOriginal && aGenerated.line > 0 && aGenerated.column >= 0 && aOriginal.line > 0 && aOriginal.column >= 0 && aSource) {
          return;
        } else {
          throw new Error("Invalid mapping: " + JSON.stringify({
            generated: aGenerated,
            source: aSource,
            original: aOriginal,
            name: aName
          }));
        }
      };
      SourceMapGenerator.prototype._serializeMappings = function SourceMapGenerator_serializeMappings() {
        var previousGeneratedColumn = 0;
        var previousGeneratedLine = 1;
        var previousOriginalColumn = 0;
        var previousOriginalLine = 0;
        var previousName = 0;
        var previousSource = 0;
        var result = "";
        var next;
        var mapping;
        var nameIdx;
        var sourceIdx;
        var mappings = this._mappings.toArray();
        for (var i = 0, len = mappings.length; i < len; i++) {
          mapping = mappings[i];
          next = "";
          if (mapping.generatedLine !== previousGeneratedLine) {
            previousGeneratedColumn = 0;
            while (mapping.generatedLine !== previousGeneratedLine) {
              next += ";";
              previousGeneratedLine++;
            }
          } else {
            if (i > 0) {
              if (!util2.compareByGeneratedPositionsInflated(mapping, mappings[i - 1])) {
                continue;
              }
              next += ",";
            }
          }
          next += base64VLQ.encode(mapping.generatedColumn - previousGeneratedColumn);
          previousGeneratedColumn = mapping.generatedColumn;
          if (mapping.source != null) {
            sourceIdx = this._sources.indexOf(mapping.source);
            next += base64VLQ.encode(sourceIdx - previousSource);
            previousSource = sourceIdx;
            next += base64VLQ.encode(mapping.originalLine - 1 - previousOriginalLine);
            previousOriginalLine = mapping.originalLine - 1;
            next += base64VLQ.encode(mapping.originalColumn - previousOriginalColumn);
            previousOriginalColumn = mapping.originalColumn;
            if (mapping.name != null) {
              nameIdx = this._names.indexOf(mapping.name);
              next += base64VLQ.encode(nameIdx - previousName);
              previousName = nameIdx;
            }
          }
          result += next;
        }
        return result;
      };
      SourceMapGenerator.prototype._generateSourcesContent = function SourceMapGenerator_generateSourcesContent(aSources, aSourceRoot) {
        return aSources.map(function(source) {
          if (!this._sourcesContents) {
            return null;
          }
          if (aSourceRoot != null) {
            source = util2.relative(aSourceRoot, source);
          }
          var key = util2.toSetString(source);
          return Object.prototype.hasOwnProperty.call(this._sourcesContents, key) ? this._sourcesContents[key] : null;
        }, this);
      };
      SourceMapGenerator.prototype.toJSON = function SourceMapGenerator_toJSON() {
        var map = {
          version: this._version,
          sources: this._sources.toArray(),
          names: this._names.toArray(),
          mappings: this._serializeMappings()
        };
        if (this._file != null) {
          map.file = this._file;
        }
        if (this._sourceRoot != null) {
          map.sourceRoot = this._sourceRoot;
        }
        if (this._sourcesContents) {
          map.sourcesContent = this._generateSourcesContent(map.sources, map.sourceRoot);
        }
        return map;
      };
      SourceMapGenerator.prototype.toString = function SourceMapGenerator_toString() {
        return JSON.stringify(this.toJSON());
      };
      exports.SourceMapGenerator = SourceMapGenerator;
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/binary-search.js
  var require_binary_search = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/binary-search.js"(exports) {
      "use strict";
      exports.GREATEST_LOWER_BOUND = 1;
      exports.LEAST_UPPER_BOUND = 2;
      function recursiveSearch(aLow, aHigh, aNeedle, aHaystack, aCompare, aBias) {
        var mid = Math.floor((aHigh - aLow) / 2) + aLow;
        var cmp = aCompare(aNeedle, aHaystack[mid], true);
        if (cmp === 0) {
          return mid;
        } else if (cmp > 0) {
          if (aHigh - mid > 1) {
            return recursiveSearch(mid, aHigh, aNeedle, aHaystack, aCompare, aBias);
          }
          if (aBias == exports.LEAST_UPPER_BOUND) {
            return aHigh < aHaystack.length ? aHigh : -1;
          } else {
            return mid;
          }
        } else {
          if (mid - aLow > 1) {
            return recursiveSearch(aLow, mid, aNeedle, aHaystack, aCompare, aBias);
          }
          if (aBias == exports.LEAST_UPPER_BOUND) {
            return mid;
          } else {
            return aLow < 0 ? -1 : aLow;
          }
        }
      }
      exports.search = function search(aNeedle, aHaystack, aCompare, aBias) {
        if (aHaystack.length === 0) {
          return -1;
        }
        var index = recursiveSearch(
          -1,
          aHaystack.length,
          aNeedle,
          aHaystack,
          aCompare,
          aBias || exports.GREATEST_LOWER_BOUND
        );
        if (index < 0) {
          return -1;
        }
        while (index - 1 >= 0) {
          if (aCompare(aHaystack[index], aHaystack[index - 1], true) !== 0) {
            break;
          }
          --index;
        }
        return index;
      };
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/quick-sort.js
  var require_quick_sort = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/quick-sort.js"(exports) {
      "use strict";
      function swap(ary, x, y) {
        var temp = ary[x];
        ary[x] = ary[y];
        ary[y] = temp;
      }
      function randomIntInRange(low, high) {
        return Math.round(low + Math.random() * (high - low));
      }
      function doQuickSort(ary, comparator, p, r) {
        if (p < r) {
          var pivotIndex = randomIntInRange(p, r);
          var i = p - 1;
          swap(ary, pivotIndex, r);
          var pivot = ary[r];
          for (var j = p; j < r; j++) {
            if (comparator(ary[j], pivot) <= 0) {
              i += 1;
              swap(ary, i, j);
            }
          }
          swap(ary, i + 1, j);
          var q = i + 1;
          doQuickSort(ary, comparator, p, q - 1);
          doQuickSort(ary, comparator, q + 1, r);
        }
      }
      exports.quickSort = function(ary, comparator) {
        doQuickSort(ary, comparator, 0, ary.length - 1);
      };
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/source-map-consumer.js
  var require_source_map_consumer = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/source-map-consumer.js"(exports) {
      "use strict";
      var util2 = require_util();
      var binarySearch = require_binary_search();
      var ArraySet = require_array_set().ArraySet;
      var base64VLQ = require_base64_vlq();
      var quickSort = require_quick_sort().quickSort;
      function SourceMapConsumer(aSourceMap, aSourceMapURL) {
        var sourceMap = aSourceMap;
        if (typeof aSourceMap === "string") {
          sourceMap = util2.parseSourceMapInput(aSourceMap);
        }
        return sourceMap.sections != null ? new IndexedSourceMapConsumer(sourceMap, aSourceMapURL) : new BasicSourceMapConsumer(sourceMap, aSourceMapURL);
      }
      SourceMapConsumer.fromSourceMap = function(aSourceMap, aSourceMapURL) {
        return BasicSourceMapConsumer.fromSourceMap(aSourceMap, aSourceMapURL);
      };
      SourceMapConsumer.prototype._version = 3;
      SourceMapConsumer.prototype.__generatedMappings = null;
      Object.defineProperty(SourceMapConsumer.prototype, "_generatedMappings", {
        configurable: true,
        enumerable: true,
        get: function() {
          if (!this.__generatedMappings) {
            this._parseMappings(this._mappings, this.sourceRoot);
          }
          return this.__generatedMappings;
        }
      });
      SourceMapConsumer.prototype.__originalMappings = null;
      Object.defineProperty(SourceMapConsumer.prototype, "_originalMappings", {
        configurable: true,
        enumerable: true,
        get: function() {
          if (!this.__originalMappings) {
            this._parseMappings(this._mappings, this.sourceRoot);
          }
          return this.__originalMappings;
        }
      });
      SourceMapConsumer.prototype._charIsMappingSeparator = function SourceMapConsumer_charIsMappingSeparator(aStr, index) {
        var c = aStr.charAt(index);
        return c === ";" || c === ",";
      };
      SourceMapConsumer.prototype._parseMappings = function SourceMapConsumer_parseMappings(aStr, aSourceRoot) {
        throw new Error("Subclasses must implement _parseMappings");
      };
      SourceMapConsumer.GENERATED_ORDER = 1;
      SourceMapConsumer.ORIGINAL_ORDER = 2;
      SourceMapConsumer.GREATEST_LOWER_BOUND = 1;
      SourceMapConsumer.LEAST_UPPER_BOUND = 2;
      SourceMapConsumer.prototype.eachMapping = function SourceMapConsumer_eachMapping(aCallback, aContext, aOrder) {
        var context = aContext || null;
        var order = aOrder || SourceMapConsumer.GENERATED_ORDER;
        var mappings;
        switch (order) {
          case SourceMapConsumer.GENERATED_ORDER:
            mappings = this._generatedMappings;
            break;
          case SourceMapConsumer.ORIGINAL_ORDER:
            mappings = this._originalMappings;
            break;
          default:
            throw new Error("Unknown order of iteration.");
        }
        var sourceRoot = this.sourceRoot;
        mappings.map(function(mapping) {
          var source = mapping.source === null ? null : this._sources.at(mapping.source);
          source = util2.computeSourceURL(sourceRoot, source, this._sourceMapURL);
          return {
            source,
            generatedLine: mapping.generatedLine,
            generatedColumn: mapping.generatedColumn,
            originalLine: mapping.originalLine,
            originalColumn: mapping.originalColumn,
            name: mapping.name === null ? null : this._names.at(mapping.name)
          };
        }, this).forEach(aCallback, context);
      };
      SourceMapConsumer.prototype.allGeneratedPositionsFor = function SourceMapConsumer_allGeneratedPositionsFor(aArgs) {
        var line = util2.getArg(aArgs, "line");
        var needle = {
          source: util2.getArg(aArgs, "source"),
          originalLine: line,
          originalColumn: util2.getArg(aArgs, "column", 0)
        };
        needle.source = this._findSourceIndex(needle.source);
        if (needle.source < 0) {
          return [];
        }
        var mappings = [];
        var index = this._findMapping(
          needle,
          this._originalMappings,
          "originalLine",
          "originalColumn",
          util2.compareByOriginalPositions,
          binarySearch.LEAST_UPPER_BOUND
        );
        if (index >= 0) {
          var mapping = this._originalMappings[index];
          if (aArgs.column === void 0) {
            var originalLine = mapping.originalLine;
            while (mapping && mapping.originalLine === originalLine) {
              mappings.push({
                line: util2.getArg(mapping, "generatedLine", null),
                column: util2.getArg(mapping, "generatedColumn", null),
                lastColumn: util2.getArg(mapping, "lastGeneratedColumn", null)
              });
              mapping = this._originalMappings[++index];
            }
          } else {
            var originalColumn = mapping.originalColumn;
            while (mapping && mapping.originalLine === line && mapping.originalColumn == originalColumn) {
              mappings.push({
                line: util2.getArg(mapping, "generatedLine", null),
                column: util2.getArg(mapping, "generatedColumn", null),
                lastColumn: util2.getArg(mapping, "lastGeneratedColumn", null)
              });
              mapping = this._originalMappings[++index];
            }
          }
        }
        return mappings;
      };
      exports.SourceMapConsumer = SourceMapConsumer;
      function BasicSourceMapConsumer(aSourceMap, aSourceMapURL) {
        var sourceMap = aSourceMap;
        if (typeof aSourceMap === "string") {
          sourceMap = util2.parseSourceMapInput(aSourceMap);
        }
        var version = util2.getArg(sourceMap, "version");
        var sources = util2.getArg(sourceMap, "sources");
        var names = util2.getArg(sourceMap, "names", []);
        var sourceRoot = util2.getArg(sourceMap, "sourceRoot", null);
        var sourcesContent = util2.getArg(sourceMap, "sourcesContent", null);
        var mappings = util2.getArg(sourceMap, "mappings");
        var file = util2.getArg(sourceMap, "file", null);
        if (version != this._version) {
          throw new Error("Unsupported version: " + version);
        }
        if (sourceRoot) {
          sourceRoot = util2.normalize(sourceRoot);
        }
        sources = sources.map(String).map(util2.normalize).map(function(source) {
          return sourceRoot && util2.isAbsolute(sourceRoot) && util2.isAbsolute(source) ? util2.relative(sourceRoot, source) : source;
        });
        this._names = ArraySet.fromArray(names.map(String), true);
        this._sources = ArraySet.fromArray(sources, true);
        this._absoluteSources = this._sources.toArray().map(function(s) {
          return util2.computeSourceURL(sourceRoot, s, aSourceMapURL);
        });
        this.sourceRoot = sourceRoot;
        this.sourcesContent = sourcesContent;
        this._mappings = mappings;
        this._sourceMapURL = aSourceMapURL;
        this.file = file;
      }
      BasicSourceMapConsumer.prototype = Object.create(SourceMapConsumer.prototype);
      BasicSourceMapConsumer.prototype.consumer = SourceMapConsumer;
      BasicSourceMapConsumer.prototype._findSourceIndex = function(aSource) {
        var relativeSource = aSource;
        if (this.sourceRoot != null) {
          relativeSource = util2.relative(this.sourceRoot, relativeSource);
        }
        if (this._sources.has(relativeSource)) {
          return this._sources.indexOf(relativeSource);
        }
        var i;
        for (i = 0; i < this._absoluteSources.length; ++i) {
          if (this._absoluteSources[i] == aSource) {
            return i;
          }
        }
        return -1;
      };
      BasicSourceMapConsumer.fromSourceMap = function SourceMapConsumer_fromSourceMap(aSourceMap, aSourceMapURL) {
        var smc = Object.create(BasicSourceMapConsumer.prototype);
        var names = smc._names = ArraySet.fromArray(aSourceMap._names.toArray(), true);
        var sources = smc._sources = ArraySet.fromArray(aSourceMap._sources.toArray(), true);
        smc.sourceRoot = aSourceMap._sourceRoot;
        smc.sourcesContent = aSourceMap._generateSourcesContent(
          smc._sources.toArray(),
          smc.sourceRoot
        );
        smc.file = aSourceMap._file;
        smc._sourceMapURL = aSourceMapURL;
        smc._absoluteSources = smc._sources.toArray().map(function(s) {
          return util2.computeSourceURL(smc.sourceRoot, s, aSourceMapURL);
        });
        var generatedMappings = aSourceMap._mappings.toArray().slice();
        var destGeneratedMappings = smc.__generatedMappings = [];
        var destOriginalMappings = smc.__originalMappings = [];
        for (var i = 0, length = generatedMappings.length; i < length; i++) {
          var srcMapping = generatedMappings[i];
          var destMapping = new Mapping();
          destMapping.generatedLine = srcMapping.generatedLine;
          destMapping.generatedColumn = srcMapping.generatedColumn;
          if (srcMapping.source) {
            destMapping.source = sources.indexOf(srcMapping.source);
            destMapping.originalLine = srcMapping.originalLine;
            destMapping.originalColumn = srcMapping.originalColumn;
            if (srcMapping.name) {
              destMapping.name = names.indexOf(srcMapping.name);
            }
            destOriginalMappings.push(destMapping);
          }
          destGeneratedMappings.push(destMapping);
        }
        quickSort(smc.__originalMappings, util2.compareByOriginalPositions);
        return smc;
      };
      BasicSourceMapConsumer.prototype._version = 3;
      Object.defineProperty(BasicSourceMapConsumer.prototype, "sources", {
        get: function() {
          return this._absoluteSources.slice();
        }
      });
      function Mapping() {
        this.generatedLine = 0;
        this.generatedColumn = 0;
        this.source = null;
        this.originalLine = null;
        this.originalColumn = null;
        this.name = null;
      }
      BasicSourceMapConsumer.prototype._parseMappings = function SourceMapConsumer_parseMappings(aStr, aSourceRoot) {
        var generatedLine = 1;
        var previousGeneratedColumn = 0;
        var previousOriginalLine = 0;
        var previousOriginalColumn = 0;
        var previousSource = 0;
        var previousName = 0;
        var length = aStr.length;
        var index = 0;
        var cachedSegments = {};
        var temp = {};
        var originalMappings = [];
        var generatedMappings = [];
        var mapping, str, segment, end, value;
        while (index < length) {
          if (aStr.charAt(index) === ";") {
            generatedLine++;
            index++;
            previousGeneratedColumn = 0;
          } else if (aStr.charAt(index) === ",") {
            index++;
          } else {
            mapping = new Mapping();
            mapping.generatedLine = generatedLine;
            for (end = index; end < length; end++) {
              if (this._charIsMappingSeparator(aStr, end)) {
                break;
              }
            }
            str = aStr.slice(index, end);
            segment = cachedSegments[str];
            if (segment) {
              index += str.length;
            } else {
              segment = [];
              while (index < end) {
                base64VLQ.decode(aStr, index, temp);
                value = temp.value;
                index = temp.rest;
                segment.push(value);
              }
              if (segment.length === 2) {
                throw new Error("Found a source, but no line and column");
              }
              if (segment.length === 3) {
                throw new Error("Found a source and line, but no column");
              }
              cachedSegments[str] = segment;
            }
            mapping.generatedColumn = previousGeneratedColumn + segment[0];
            previousGeneratedColumn = mapping.generatedColumn;
            if (segment.length > 1) {
              mapping.source = previousSource + segment[1];
              previousSource += segment[1];
              mapping.originalLine = previousOriginalLine + segment[2];
              previousOriginalLine = mapping.originalLine;
              mapping.originalLine += 1;
              mapping.originalColumn = previousOriginalColumn + segment[3];
              previousOriginalColumn = mapping.originalColumn;
              if (segment.length > 4) {
                mapping.name = previousName + segment[4];
                previousName += segment[4];
              }
            }
            generatedMappings.push(mapping);
            if (typeof mapping.originalLine === "number") {
              originalMappings.push(mapping);
            }
          }
        }
        quickSort(generatedMappings, util2.compareByGeneratedPositionsDeflated);
        this.__generatedMappings = generatedMappings;
        quickSort(originalMappings, util2.compareByOriginalPositions);
        this.__originalMappings = originalMappings;
      };
      BasicSourceMapConsumer.prototype._findMapping = function SourceMapConsumer_findMapping(aNeedle, aMappings, aLineName, aColumnName, aComparator, aBias) {
        if (aNeedle[aLineName] <= 0) {
          throw new TypeError("Line must be greater than or equal to 1, got " + aNeedle[aLineName]);
        }
        if (aNeedle[aColumnName] < 0) {
          throw new TypeError("Column must be greater than or equal to 0, got " + aNeedle[aColumnName]);
        }
        return binarySearch.search(aNeedle, aMappings, aComparator, aBias);
      };
      BasicSourceMapConsumer.prototype.computeColumnSpans = function SourceMapConsumer_computeColumnSpans() {
        for (var index = 0; index < this._generatedMappings.length; ++index) {
          var mapping = this._generatedMappings[index];
          if (index + 1 < this._generatedMappings.length) {
            var nextMapping = this._generatedMappings[index + 1];
            if (mapping.generatedLine === nextMapping.generatedLine) {
              mapping.lastGeneratedColumn = nextMapping.generatedColumn - 1;
              continue;
            }
          }
          mapping.lastGeneratedColumn = Infinity;
        }
      };
      BasicSourceMapConsumer.prototype.originalPositionFor = function SourceMapConsumer_originalPositionFor(aArgs) {
        var needle = {
          generatedLine: util2.getArg(aArgs, "line"),
          generatedColumn: util2.getArg(aArgs, "column")
        };
        var index = this._findMapping(
          needle,
          this._generatedMappings,
          "generatedLine",
          "generatedColumn",
          util2.compareByGeneratedPositionsDeflated,
          util2.getArg(aArgs, "bias", SourceMapConsumer.GREATEST_LOWER_BOUND)
        );
        if (index >= 0) {
          var mapping = this._generatedMappings[index];
          if (mapping.generatedLine === needle.generatedLine) {
            var source = util2.getArg(mapping, "source", null);
            if (source !== null) {
              source = this._sources.at(source);
              source = util2.computeSourceURL(this.sourceRoot, source, this._sourceMapURL);
            }
            var name2 = util2.getArg(mapping, "name", null);
            if (name2 !== null) {
              name2 = this._names.at(name2);
            }
            return {
              source,
              line: util2.getArg(mapping, "originalLine", null),
              column: util2.getArg(mapping, "originalColumn", null),
              name: name2
            };
          }
        }
        return {
          source: null,
          line: null,
          column: null,
          name: null
        };
      };
      BasicSourceMapConsumer.prototype.hasContentsOfAllSources = function BasicSourceMapConsumer_hasContentsOfAllSources() {
        if (!this.sourcesContent) {
          return false;
        }
        return this.sourcesContent.length >= this._sources.size() && !this.sourcesContent.some(function(sc) {
          return sc == null;
        });
      };
      BasicSourceMapConsumer.prototype.sourceContentFor = function SourceMapConsumer_sourceContentFor(aSource, nullOnMissing) {
        if (!this.sourcesContent) {
          return null;
        }
        var index = this._findSourceIndex(aSource);
        if (index >= 0) {
          return this.sourcesContent[index];
        }
        var relativeSource = aSource;
        if (this.sourceRoot != null) {
          relativeSource = util2.relative(this.sourceRoot, relativeSource);
        }
        var url;
        if (this.sourceRoot != null && (url = util2.urlParse(this.sourceRoot))) {
          var fileUriAbsPath = relativeSource.replace(/^file:\/\//, "");
          if (url.scheme == "file" && this._sources.has(fileUriAbsPath)) {
            return this.sourcesContent[this._sources.indexOf(fileUriAbsPath)];
          }
          if ((!url.path || url.path == "/") && this._sources.has("/" + relativeSource)) {
            return this.sourcesContent[this._sources.indexOf("/" + relativeSource)];
          }
        }
        if (nullOnMissing) {
          return null;
        } else {
          throw new Error('"' + relativeSource + '" is not in the SourceMap.');
        }
      };
      BasicSourceMapConsumer.prototype.generatedPositionFor = function SourceMapConsumer_generatedPositionFor(aArgs) {
        var source = util2.getArg(aArgs, "source");
        source = this._findSourceIndex(source);
        if (source < 0) {
          return {
            line: null,
            column: null,
            lastColumn: null
          };
        }
        var needle = {
          source,
          originalLine: util2.getArg(aArgs, "line"),
          originalColumn: util2.getArg(aArgs, "column")
        };
        var index = this._findMapping(
          needle,
          this._originalMappings,
          "originalLine",
          "originalColumn",
          util2.compareByOriginalPositions,
          util2.getArg(aArgs, "bias", SourceMapConsumer.GREATEST_LOWER_BOUND)
        );
        if (index >= 0) {
          var mapping = this._originalMappings[index];
          if (mapping.source === needle.source) {
            return {
              line: util2.getArg(mapping, "generatedLine", null),
              column: util2.getArg(mapping, "generatedColumn", null),
              lastColumn: util2.getArg(mapping, "lastGeneratedColumn", null)
            };
          }
        }
        return {
          line: null,
          column: null,
          lastColumn: null
        };
      };
      exports.BasicSourceMapConsumer = BasicSourceMapConsumer;
      function IndexedSourceMapConsumer(aSourceMap, aSourceMapURL) {
        var sourceMap = aSourceMap;
        if (typeof aSourceMap === "string") {
          sourceMap = util2.parseSourceMapInput(aSourceMap);
        }
        var version = util2.getArg(sourceMap, "version");
        var sections = util2.getArg(sourceMap, "sections");
        if (version != this._version) {
          throw new Error("Unsupported version: " + version);
        }
        this._sources = new ArraySet();
        this._names = new ArraySet();
        var lastOffset = {
          line: -1,
          column: 0
        };
        this._sections = sections.map(function(s) {
          if (s.url) {
            throw new Error("Support for url field in sections not implemented.");
          }
          var offset = util2.getArg(s, "offset");
          var offsetLine = util2.getArg(offset, "line");
          var offsetColumn = util2.getArg(offset, "column");
          if (offsetLine < lastOffset.line || offsetLine === lastOffset.line && offsetColumn < lastOffset.column) {
            throw new Error("Section offsets must be ordered and non-overlapping.");
          }
          lastOffset = offset;
          return {
            generatedOffset: {
              // The offset fields are 0-based, but we use 1-based indices when
              // encoding/decoding from VLQ.
              generatedLine: offsetLine + 1,
              generatedColumn: offsetColumn + 1
            },
            consumer: new SourceMapConsumer(util2.getArg(s, "map"), aSourceMapURL)
          };
        });
      }
      IndexedSourceMapConsumer.prototype = Object.create(SourceMapConsumer.prototype);
      IndexedSourceMapConsumer.prototype.constructor = SourceMapConsumer;
      IndexedSourceMapConsumer.prototype._version = 3;
      Object.defineProperty(IndexedSourceMapConsumer.prototype, "sources", {
        get: function() {
          var sources = [];
          for (var i = 0; i < this._sections.length; i++) {
            for (var j = 0; j < this._sections[i].consumer.sources.length; j++) {
              sources.push(this._sections[i].consumer.sources[j]);
            }
          }
          return sources;
        }
      });
      IndexedSourceMapConsumer.prototype.originalPositionFor = function IndexedSourceMapConsumer_originalPositionFor(aArgs) {
        var needle = {
          generatedLine: util2.getArg(aArgs, "line"),
          generatedColumn: util2.getArg(aArgs, "column")
        };
        var sectionIndex = binarySearch.search(
          needle,
          this._sections,
          function(needle2, section2) {
            var cmp = needle2.generatedLine - section2.generatedOffset.generatedLine;
            if (cmp) {
              return cmp;
            }
            return needle2.generatedColumn - section2.generatedOffset.generatedColumn;
          }
        );
        var section = this._sections[sectionIndex];
        if (!section) {
          return {
            source: null,
            line: null,
            column: null,
            name: null
          };
        }
        return section.consumer.originalPositionFor({
          line: needle.generatedLine - (section.generatedOffset.generatedLine - 1),
          column: needle.generatedColumn - (section.generatedOffset.generatedLine === needle.generatedLine ? section.generatedOffset.generatedColumn - 1 : 0),
          bias: aArgs.bias
        });
      };
      IndexedSourceMapConsumer.prototype.hasContentsOfAllSources = function IndexedSourceMapConsumer_hasContentsOfAllSources() {
        return this._sections.every(function(s) {
          return s.consumer.hasContentsOfAllSources();
        });
      };
      IndexedSourceMapConsumer.prototype.sourceContentFor = function IndexedSourceMapConsumer_sourceContentFor(aSource, nullOnMissing) {
        for (var i = 0; i < this._sections.length; i++) {
          var section = this._sections[i];
          var content = section.consumer.sourceContentFor(aSource, true);
          if (content) {
            return content;
          }
        }
        if (nullOnMissing) {
          return null;
        } else {
          throw new Error('"' + aSource + '" is not in the SourceMap.');
        }
      };
      IndexedSourceMapConsumer.prototype.generatedPositionFor = function IndexedSourceMapConsumer_generatedPositionFor(aArgs) {
        for (var i = 0; i < this._sections.length; i++) {
          var section = this._sections[i];
          if (section.consumer._findSourceIndex(util2.getArg(aArgs, "source")) === -1) {
            continue;
          }
          var generatedPosition = section.consumer.generatedPositionFor(aArgs);
          if (generatedPosition) {
            var ret = {
              line: generatedPosition.line + (section.generatedOffset.generatedLine - 1),
              column: generatedPosition.column + (section.generatedOffset.generatedLine === generatedPosition.line ? section.generatedOffset.generatedColumn - 1 : 0)
            };
            return ret;
          }
        }
        return {
          line: null,
          column: null
        };
      };
      IndexedSourceMapConsumer.prototype._parseMappings = function IndexedSourceMapConsumer_parseMappings(aStr, aSourceRoot) {
        this.__generatedMappings = [];
        this.__originalMappings = [];
        for (var i = 0; i < this._sections.length; i++) {
          var section = this._sections[i];
          var sectionMappings = section.consumer._generatedMappings;
          for (var j = 0; j < sectionMappings.length; j++) {
            var mapping = sectionMappings[j];
            var source = section.consumer._sources.at(mapping.source);
            source = util2.computeSourceURL(section.consumer.sourceRoot, source, this._sourceMapURL);
            this._sources.add(source);
            source = this._sources.indexOf(source);
            var name2 = null;
            if (mapping.name) {
              name2 = section.consumer._names.at(mapping.name);
              this._names.add(name2);
              name2 = this._names.indexOf(name2);
            }
            var adjustedMapping = {
              source,
              generatedLine: mapping.generatedLine + (section.generatedOffset.generatedLine - 1),
              generatedColumn: mapping.generatedColumn + (section.generatedOffset.generatedLine === mapping.generatedLine ? section.generatedOffset.generatedColumn - 1 : 0),
              originalLine: mapping.originalLine,
              originalColumn: mapping.originalColumn,
              name: name2
            };
            this.__generatedMappings.push(adjustedMapping);
            if (typeof adjustedMapping.originalLine === "number") {
              this.__originalMappings.push(adjustedMapping);
            }
          }
        }
        quickSort(this.__generatedMappings, util2.compareByGeneratedPositionsDeflated);
        quickSort(this.__originalMappings, util2.compareByOriginalPositions);
      };
      exports.IndexedSourceMapConsumer = IndexedSourceMapConsumer;
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/source-node.js
  var require_source_node = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/lib/source-node.js"(exports) {
      "use strict";
      var SourceMapGenerator = require_source_map_generator().SourceMapGenerator;
      var util2 = require_util();
      var REGEX_NEWLINE = /(\r?\n)/;
      var NEWLINE_CODE = 10;
      var isSourceNode = "$$$isSourceNode$$$";
      function SourceNode(aLine, aColumn, aSource, aChunks, aName) {
        this.children = [];
        this.sourceContents = {};
        this.line = aLine == null ? null : aLine;
        this.column = aColumn == null ? null : aColumn;
        this.source = aSource == null ? null : aSource;
        this.name = aName == null ? null : aName;
        this[isSourceNode] = true;
        if (aChunks != null) this.add(aChunks);
      }
      SourceNode.fromStringWithSourceMap = function SourceNode_fromStringWithSourceMap(aGeneratedCode, aSourceMapConsumer, aRelativePath) {
        var node = new SourceNode();
        var remainingLines = aGeneratedCode.split(REGEX_NEWLINE);
        var remainingLinesIndex = 0;
        var shiftNextLine = function() {
          var lineContents = getNextLine();
          var newLine = getNextLine() || "";
          return lineContents + newLine;
          function getNextLine() {
            return remainingLinesIndex < remainingLines.length ? remainingLines[remainingLinesIndex++] : void 0;
          }
        };
        var lastGeneratedLine = 1, lastGeneratedColumn = 0;
        var lastMapping = null;
        aSourceMapConsumer.eachMapping(function(mapping) {
          if (lastMapping !== null) {
            if (lastGeneratedLine < mapping.generatedLine) {
              addMappingWithCode(lastMapping, shiftNextLine());
              lastGeneratedLine++;
              lastGeneratedColumn = 0;
            } else {
              var nextLine = remainingLines[remainingLinesIndex] || "";
              var code = nextLine.substr(0, mapping.generatedColumn - lastGeneratedColumn);
              remainingLines[remainingLinesIndex] = nextLine.substr(mapping.generatedColumn - lastGeneratedColumn);
              lastGeneratedColumn = mapping.generatedColumn;
              addMappingWithCode(lastMapping, code);
              lastMapping = mapping;
              return;
            }
          }
          while (lastGeneratedLine < mapping.generatedLine) {
            node.add(shiftNextLine());
            lastGeneratedLine++;
          }
          if (lastGeneratedColumn < mapping.generatedColumn) {
            var nextLine = remainingLines[remainingLinesIndex] || "";
            node.add(nextLine.substr(0, mapping.generatedColumn));
            remainingLines[remainingLinesIndex] = nextLine.substr(mapping.generatedColumn);
            lastGeneratedColumn = mapping.generatedColumn;
          }
          lastMapping = mapping;
        }, this);
        if (remainingLinesIndex < remainingLines.length) {
          if (lastMapping) {
            addMappingWithCode(lastMapping, shiftNextLine());
          }
          node.add(remainingLines.splice(remainingLinesIndex).join(""));
        }
        aSourceMapConsumer.sources.forEach(function(sourceFile) {
          var content = aSourceMapConsumer.sourceContentFor(sourceFile);
          if (content != null) {
            if (aRelativePath != null) {
              sourceFile = util2.join(aRelativePath, sourceFile);
            }
            node.setSourceContent(sourceFile, content);
          }
        });
        return node;
        function addMappingWithCode(mapping, code) {
          if (mapping === null || mapping.source === void 0) {
            node.add(code);
          } else {
            var source = aRelativePath ? util2.join(aRelativePath, mapping.source) : mapping.source;
            node.add(new SourceNode(
              mapping.originalLine,
              mapping.originalColumn,
              source,
              code,
              mapping.name
            ));
          }
        }
      };
      SourceNode.prototype.add = function SourceNode_add(aChunk) {
        if (Array.isArray(aChunk)) {
          aChunk.forEach(function(chunk) {
            this.add(chunk);
          }, this);
        } else if (aChunk[isSourceNode] || typeof aChunk === "string") {
          if (aChunk) {
            this.children.push(aChunk);
          }
        } else {
          throw new TypeError(
            "Expected a SourceNode, string, or an array of SourceNodes and strings. Got " + aChunk
          );
        }
        return this;
      };
      SourceNode.prototype.prepend = function SourceNode_prepend(aChunk) {
        if (Array.isArray(aChunk)) {
          for (var i = aChunk.length - 1; i >= 0; i--) {
            this.prepend(aChunk[i]);
          }
        } else if (aChunk[isSourceNode] || typeof aChunk === "string") {
          this.children.unshift(aChunk);
        } else {
          throw new TypeError(
            "Expected a SourceNode, string, or an array of SourceNodes and strings. Got " + aChunk
          );
        }
        return this;
      };
      SourceNode.prototype.walk = function SourceNode_walk(aFn) {
        var chunk;
        for (var i = 0, len = this.children.length; i < len; i++) {
          chunk = this.children[i];
          if (chunk[isSourceNode]) {
            chunk.walk(aFn);
          } else {
            if (chunk !== "") {
              aFn(chunk, {
                source: this.source,
                line: this.line,
                column: this.column,
                name: this.name
              });
            }
          }
        }
      };
      SourceNode.prototype.join = function SourceNode_join(aSep) {
        var newChildren;
        var i;
        var len = this.children.length;
        if (len > 0) {
          newChildren = [];
          for (i = 0; i < len - 1; i++) {
            newChildren.push(this.children[i]);
            newChildren.push(aSep);
          }
          newChildren.push(this.children[i]);
          this.children = newChildren;
        }
        return this;
      };
      SourceNode.prototype.replaceRight = function SourceNode_replaceRight(aPattern, aReplacement) {
        var lastChild = this.children[this.children.length - 1];
        if (lastChild[isSourceNode]) {
          lastChild.replaceRight(aPattern, aReplacement);
        } else if (typeof lastChild === "string") {
          this.children[this.children.length - 1] = lastChild.replace(aPattern, aReplacement);
        } else {
          this.children.push("".replace(aPattern, aReplacement));
        }
        return this;
      };
      SourceNode.prototype.setSourceContent = function SourceNode_setSourceContent(aSourceFile, aSourceContent) {
        this.sourceContents[util2.toSetString(aSourceFile)] = aSourceContent;
      };
      SourceNode.prototype.walkSourceContents = function SourceNode_walkSourceContents(aFn) {
        for (var i = 0, len = this.children.length; i < len; i++) {
          if (this.children[i][isSourceNode]) {
            this.children[i].walkSourceContents(aFn);
          }
        }
        var sources = Object.keys(this.sourceContents);
        for (var i = 0, len = sources.length; i < len; i++) {
          aFn(util2.fromSetString(sources[i]), this.sourceContents[sources[i]]);
        }
      };
      SourceNode.prototype.toString = function SourceNode_toString() {
        var str = "";
        this.walk(function(chunk) {
          str += chunk;
        });
        return str;
      };
      SourceNode.prototype.toStringWithSourceMap = function SourceNode_toStringWithSourceMap(aArgs) {
        var generated = {
          code: "",
          line: 1,
          column: 0
        };
        var map = new SourceMapGenerator(aArgs);
        var sourceMappingActive = false;
        var lastOriginalSource = null;
        var lastOriginalLine = null;
        var lastOriginalColumn = null;
        var lastOriginalName = null;
        this.walk(function(chunk, original) {
          generated.code += chunk;
          if (original.source !== null && original.line !== null && original.column !== null) {
            if (lastOriginalSource !== original.source || lastOriginalLine !== original.line || lastOriginalColumn !== original.column || lastOriginalName !== original.name) {
              map.addMapping({
                source: original.source,
                original: {
                  line: original.line,
                  column: original.column
                },
                generated: {
                  line: generated.line,
                  column: generated.column
                },
                name: original.name
              });
            }
            lastOriginalSource = original.source;
            lastOriginalLine = original.line;
            lastOriginalColumn = original.column;
            lastOriginalName = original.name;
            sourceMappingActive = true;
          } else if (sourceMappingActive) {
            map.addMapping({
              generated: {
                line: generated.line,
                column: generated.column
              }
            });
            lastOriginalSource = null;
            sourceMappingActive = false;
          }
          for (var idx = 0, length = chunk.length; idx < length; idx++) {
            if (chunk.charCodeAt(idx) === NEWLINE_CODE) {
              generated.line++;
              generated.column = 0;
              if (idx + 1 === length) {
                lastOriginalSource = null;
                sourceMappingActive = false;
              } else if (sourceMappingActive) {
                map.addMapping({
                  source: original.source,
                  original: {
                    line: original.line,
                    column: original.column
                  },
                  generated: {
                    line: generated.line,
                    column: generated.column
                  },
                  name: original.name
                });
              }
            } else {
              generated.column++;
            }
          }
        });
        this.walkSourceContents(function(sourceFile, sourceContent) {
          map.setSourceContent(sourceFile, sourceContent);
        });
        return { code: generated.code, map };
      };
      exports.SourceNode = SourceNode;
    }
  });

  // node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/source-map.js
  var require_source_map = __commonJS({
    "node_modules/.pnpm/source-map@0.6.1/node_modules/source-map/source-map.js"(exports) {
      "use strict";
      exports.SourceMapGenerator = require_source_map_generator().SourceMapGenerator;
      exports.SourceMapConsumer = require_source_map_consumer().SourceMapConsumer;
      exports.SourceNode = require_source_node().SourceNode;
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/code-gen.js
  var require_code_gen = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/code-gen.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      var _utils = require_utils();
      var SourceNode = void 0;
      try {
        if (typeof define !== "function" || !define.amd) {
          SourceMap = require_source_map();
          SourceNode = SourceMap.SourceNode;
        }
      } catch (err) {
      }
      var SourceMap;
      if (!SourceNode) {
        SourceNode = function(line, column, srcFile, chunks) {
          this.src = "";
          if (chunks) {
            this.add(chunks);
          }
        };
        SourceNode.prototype = {
          add: function add(chunks) {
            if (_utils.isArray(chunks)) {
              chunks = chunks.join("");
            }
            this.src += chunks;
          },
          prepend: function prepend(chunks) {
            if (_utils.isArray(chunks)) {
              chunks = chunks.join("");
            }
            this.src = chunks + this.src;
          },
          toStringWithSourceMap: function toStringWithSourceMap() {
            return { code: this.toString() };
          },
          toString: function toString() {
            return this.src;
          }
        };
      }
      function castChunk(chunk, codeGen, loc) {
        if (_utils.isArray(chunk)) {
          var ret = [];
          for (var i = 0, len = chunk.length; i < len; i++) {
            ret.push(codeGen.wrap(chunk[i], loc));
          }
          return ret;
        } else if (typeof chunk === "boolean" || typeof chunk === "number") {
          return chunk + "";
        }
        return chunk;
      }
      function CodeGen(srcFile) {
        this.srcFile = srcFile;
        this.source = [];
      }
      CodeGen.prototype = {
        isEmpty: function isEmpty() {
          return !this.source.length;
        },
        prepend: function prepend(source, loc) {
          this.source.unshift(this.wrap(source, loc));
        },
        push: function push(source, loc) {
          this.source.push(this.wrap(source, loc));
        },
        merge: function merge() {
          var source = this.empty();
          this.each(function(line) {
            source.add(["  ", line, "\n"]);
          });
          return source;
        },
        each: function each(iter) {
          for (var i = 0, len = this.source.length; i < len; i++) {
            iter(this.source[i]);
          }
        },
        empty: function empty() {
          var loc = this.currentLocation || { start: {} };
          return new SourceNode(loc.start.line, loc.start.column, this.srcFile);
        },
        wrap: function wrap(chunk) {
          var loc = arguments.length <= 1 || arguments[1] === void 0 ? this.currentLocation || { start: {} } : arguments[1];
          if (chunk instanceof SourceNode) {
            return chunk;
          }
          chunk = castChunk(chunk, this, loc);
          return new SourceNode(loc.start.line, loc.start.column, this.srcFile, chunk);
        },
        functionCall: function functionCall(fn2, type, params) {
          params = this.generateList(params);
          return this.wrap([fn2, type ? "." + type + "(" : "(", params, ")"]);
        },
        quotedString: function quotedString(str) {
          return '"' + (str + "").replace(/\\/g, "\\\\").replace(/"/g, '\\"').replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\u2028/g, "\\u2028").replace(/\u2029/g, "\\u2029") + '"';
        },
        objectLiteral: function objectLiteral(obj) {
          var _this = this;
          var pairs = [];
          Object.keys(obj).forEach(function(key) {
            var value = castChunk(obj[key], _this);
            if (value !== "undefined") {
              pairs.push([_this.quotedString(key), ":", value]);
            }
          });
          var ret = this.generateList(pairs);
          ret.prepend("{");
          ret.add("}");
          return ret;
        },
        generateList: function generateList(entries) {
          var ret = this.empty();
          for (var i = 0, len = entries.length; i < len; i++) {
            if (i) {
              ret.add(",");
            }
            ret.add(castChunk(entries[i], this));
          }
          return ret;
        },
        generateArray: function generateArray(entries) {
          var ret = this.generateList(entries);
          ret.prepend("[");
          ret.add("]");
          return ret;
        }
      };
      exports["default"] = CodeGen;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/javascript-compiler.js
  var require_javascript_compiler = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars/compiler/javascript-compiler.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _base = require_base();
      var _exception = require_exception();
      var _exception2 = _interopRequireDefault(_exception);
      var _utils = require_utils();
      var _codeGen = require_code_gen();
      var _codeGen2 = _interopRequireDefault(_codeGen);
      function Literal(value) {
        this.value = value;
      }
      function JavaScriptCompiler() {
      }
      JavaScriptCompiler.prototype = {
        // PUBLIC API: You can override these methods in a subclass to provide
        // alternative compiled forms for name lookup and buffering semantics
        nameLookup: function nameLookup(parent, name2) {
          return this.internalNameLookup(parent, name2);
        },
        depthedLookup: function depthedLookup(name2) {
          return [this.aliasable("container.lookup"), "(depths, ", JSON.stringify(name2), ")"];
        },
        compilerInfo: function compilerInfo() {
          var revision = _base.COMPILER_REVISION, versions = _base.REVISION_CHANGES[revision];
          return [revision, versions];
        },
        appendToBuffer: function appendToBuffer(source, location, explicit) {
          if (!_utils.isArray(source)) {
            source = [source];
          }
          source = this.source.wrap(source, location);
          if (this.environment.isSimple) {
            return ["return ", source, ";"];
          } else if (explicit) {
            return ["buffer += ", source, ";"];
          } else {
            source.appendToBuffer = true;
            return source;
          }
        },
        initializeBuffer: function initializeBuffer() {
          return this.quotedString("");
        },
        // END PUBLIC API
        internalNameLookup: function internalNameLookup(parent, name2) {
          this.lookupPropertyFunctionIsUsed = true;
          return ["lookupProperty(", parent, ",", JSON.stringify(name2), ")"];
        },
        lookupPropertyFunctionIsUsed: false,
        compile: function compile(environment, options, context, asObject) {
          this.environment = environment;
          this.options = options;
          this.stringParams = this.options.stringParams;
          this.trackIds = this.options.trackIds;
          this.precompile = !asObject;
          this.name = this.environment.name;
          this.isChild = !!context;
          this.context = context || {
            decorators: [],
            programs: [],
            environments: []
          };
          this.preamble();
          this.stackSlot = 0;
          this.stackVars = [];
          this.aliases = {};
          this.registers = { list: [] };
          this.hashes = [];
          this.compileStack = [];
          this.inlineStack = [];
          this.blockParams = [];
          this.compileChildren(environment, options);
          this.useDepths = this.useDepths || environment.useDepths || environment.useDecorators || this.options.compat;
          this.useBlockParams = this.useBlockParams || environment.useBlockParams;
          var opcodes = environment.opcodes, opcode = void 0, firstLoc = void 0, i = void 0, l = void 0;
          for (i = 0, l = opcodes.length; i < l; i++) {
            opcode = opcodes[i];
            this.source.currentLocation = opcode.loc;
            firstLoc = firstLoc || opcode.loc;
            this[opcode.opcode].apply(this, opcode.args);
          }
          this.source.currentLocation = firstLoc;
          this.pushSource("");
          if (this.stackSlot || this.inlineStack.length || this.compileStack.length) {
            throw new _exception2["default"]("Compile completed with content left on stack");
          }
          if (!this.decorators.isEmpty()) {
            this.useDecorators = true;
            this.decorators.prepend(["var decorators = container.decorators, ", this.lookupPropertyFunctionVarDeclaration(), ";\n"]);
            this.decorators.push("return fn;");
            if (asObject) {
              this.decorators = Function.apply(this, ["fn", "props", "container", "depth0", "data", "blockParams", "depths", this.decorators.merge()]);
            } else {
              this.decorators.prepend("function(fn, props, container, depth0, data, blockParams, depths) {\n");
              this.decorators.push("}\n");
              this.decorators = this.decorators.merge();
            }
          } else {
            this.decorators = void 0;
          }
          var fn2 = this.createFunctionContext(asObject);
          if (!this.isChild) {
            var ret = {
              compiler: this.compilerInfo(),
              main: fn2
            };
            if (this.decorators) {
              ret.main_d = this.decorators;
              ret.useDecorators = true;
            }
            var _context = this.context;
            var programs = _context.programs;
            var decorators = _context.decorators;
            for (i = 0, l = programs.length; i < l; i++) {
              if (programs[i]) {
                ret[i] = programs[i];
                if (decorators[i]) {
                  ret[i + "_d"] = decorators[i];
                  ret.useDecorators = true;
                }
              }
            }
            if (this.environment.usePartial) {
              ret.usePartial = true;
            }
            if (this.options.data) {
              ret.useData = true;
            }
            if (this.useDepths) {
              ret.useDepths = true;
            }
            if (this.useBlockParams) {
              ret.useBlockParams = true;
            }
            if (this.options.compat) {
              ret.compat = true;
            }
            if (!asObject) {
              ret.compiler = JSON.stringify(ret.compiler);
              this.source.currentLocation = { start: { line: 1, column: 0 } };
              ret = this.objectLiteral(ret);
              if (options.srcName) {
                ret = ret.toStringWithSourceMap({ file: options.destName });
                ret.map = ret.map && ret.map.toString();
              } else {
                ret = ret.toString();
              }
            } else {
              ret.compilerOptions = this.options;
            }
            return ret;
          } else {
            return fn2;
          }
        },
        preamble: function preamble() {
          this.lastContext = 0;
          this.source = new _codeGen2["default"](this.options.srcName);
          this.decorators = new _codeGen2["default"](this.options.srcName);
        },
        createFunctionContext: function createFunctionContext(asObject) {
          var _this = this;
          var varDeclarations = "";
          var locals = this.stackVars.concat(this.registers.list);
          if (locals.length > 0) {
            varDeclarations += ", " + locals.join(", ");
          }
          var aliasCount = 0;
          Object.keys(this.aliases).forEach(function(alias) {
            var node = _this.aliases[alias];
            if (node.children && node.referenceCount > 1) {
              varDeclarations += ", alias" + ++aliasCount + "=" + alias;
              node.children[0] = "alias" + aliasCount;
            }
          });
          if (this.lookupPropertyFunctionIsUsed) {
            varDeclarations += ", " + this.lookupPropertyFunctionVarDeclaration();
          }
          var params = ["container", "depth0", "helpers", "partials", "data"];
          if (this.useBlockParams || this.useDepths) {
            params.push("blockParams");
          }
          if (this.useDepths) {
            params.push("depths");
          }
          var source = this.mergeSource(varDeclarations);
          if (asObject) {
            params.push(source);
            return Function.apply(this, params);
          } else {
            return this.source.wrap(["function(", params.join(","), ") {\n  ", source, "}"]);
          }
        },
        mergeSource: function mergeSource(varDeclarations) {
          var isSimple = this.environment.isSimple, appendOnly = !this.forceBuffer, appendFirst = void 0, sourceSeen = void 0, bufferStart = void 0, bufferEnd = void 0;
          this.source.each(function(line) {
            if (line.appendToBuffer) {
              if (bufferStart) {
                line.prepend("  + ");
              } else {
                bufferStart = line;
              }
              bufferEnd = line;
            } else {
              if (bufferStart) {
                if (!sourceSeen) {
                  appendFirst = true;
                } else {
                  bufferStart.prepend("buffer += ");
                }
                bufferEnd.add(";");
                bufferStart = bufferEnd = void 0;
              }
              sourceSeen = true;
              if (!isSimple) {
                appendOnly = false;
              }
            }
          });
          if (appendOnly) {
            if (bufferStart) {
              bufferStart.prepend("return ");
              bufferEnd.add(";");
            } else if (!sourceSeen) {
              this.source.push('return "";');
            }
          } else {
            varDeclarations += ", buffer = " + (appendFirst ? "" : this.initializeBuffer());
            if (bufferStart) {
              bufferStart.prepend("return buffer + ");
              bufferEnd.add(";");
            } else {
              this.source.push("return buffer;");
            }
          }
          if (varDeclarations) {
            this.source.prepend("var " + varDeclarations.substring(2) + (appendFirst ? "" : ";\n"));
          }
          return this.source.merge();
        },
        lookupPropertyFunctionVarDeclaration: function lookupPropertyFunctionVarDeclaration() {
          return "\n      lookupProperty = container.lookupProperty || function(parent, propertyName) {\n        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {\n          return parent[propertyName];\n        }\n        return undefined\n    }\n    ".trim();
        },
        // [blockValue]
        //
        // On stack, before: hash, inverse, program, value
        // On stack, after: return value of blockHelperMissing
        //
        // The purpose of this opcode is to take a block of the form
        // `{{#this.foo}}...{{/this.foo}}`, resolve the value of `foo`, and
        // replace it on the stack with the result of properly
        // invoking blockHelperMissing.
        blockValue: function blockValue(name2) {
          var blockHelperMissing = this.aliasable("container.hooks.blockHelperMissing"), params = [this.contextName(0)];
          this.setupHelperArgs(name2, 0, params);
          var blockName = this.popStack();
          params.splice(1, 0, blockName);
          this.push(this.source.functionCall(blockHelperMissing, "call", params));
        },
        // [ambiguousBlockValue]
        //
        // On stack, before: hash, inverse, program, value
        // Compiler value, before: lastHelper=value of last found helper, if any
        // On stack, after, if no lastHelper: same as [blockValue]
        // On stack, after, if lastHelper: value
        ambiguousBlockValue: function ambiguousBlockValue() {
          var blockHelperMissing = this.aliasable("container.hooks.blockHelperMissing"), params = [this.contextName(0)];
          this.setupHelperArgs("", 0, params, true);
          this.flushInline();
          var current = this.topStack();
          params.splice(1, 0, current);
          this.pushSource(["if (!", this.lastHelper, ") { ", current, " = ", this.source.functionCall(blockHelperMissing, "call", params), "}"]);
        },
        // [appendContent]
        //
        // On stack, before: ...
        // On stack, after: ...
        //
        // Appends the string value of `content` to the current buffer
        appendContent: function appendContent(content) {
          if (this.pendingContent) {
            content = this.pendingContent + content;
          } else {
            this.pendingLocation = this.source.currentLocation;
          }
          this.pendingContent = content;
        },
        // [append]
        //
        // On stack, before: value, ...
        // On stack, after: ...
        //
        // Coerces `value` to a String and appends it to the current buffer.
        //
        // If `value` is truthy, or 0, it is coerced into a string and appended
        // Otherwise, the empty string is appended
        append: function append() {
          if (this.isInline()) {
            this.replaceStack(function(current) {
              return [" != null ? ", current, ' : ""'];
            });
            this.pushSource(this.appendToBuffer(this.popStack()));
          } else {
            var local = this.popStack();
            this.pushSource(["if (", local, " != null) { ", this.appendToBuffer(local, void 0, true), " }"]);
            if (this.environment.isSimple) {
              this.pushSource(["else { ", this.appendToBuffer("''", void 0, true), " }"]);
            }
          }
        },
        // [appendEscaped]
        //
        // On stack, before: value, ...
        // On stack, after: ...
        //
        // Escape `value` and append it to the buffer
        appendEscaped: function appendEscaped() {
          this.pushSource(this.appendToBuffer([this.aliasable("container.escapeExpression"), "(", this.popStack(), ")"]));
        },
        // [getContext]
        //
        // On stack, before: ...
        // On stack, after: ...
        // Compiler value, after: lastContext=depth
        //
        // Set the value of the `lastContext` compiler value to the depth
        getContext: function getContext(depth) {
          this.lastContext = depth;
        },
        // [pushContext]
        //
        // On stack, before: ...
        // On stack, after: currentContext, ...
        //
        // Pushes the value of the current context onto the stack.
        pushContext: function pushContext() {
          this.pushStackLiteral(this.contextName(this.lastContext));
        },
        // [lookupOnContext]
        //
        // On stack, before: ...
        // On stack, after: currentContext[name], ...
        //
        // Looks up the value of `name` on the current context and pushes
        // it onto the stack.
        lookupOnContext: function lookupOnContext(parts, falsy, strict, scoped) {
          var i = 0;
          if (!scoped && this.options.compat && !this.lastContext) {
            this.push(this.depthedLookup(parts[i++]));
          } else {
            this.pushContext();
          }
          this.resolvePath("context", parts, i, falsy, strict);
        },
        // [lookupBlockParam]
        //
        // On stack, before: ...
        // On stack, after: blockParam[name], ...
        //
        // Looks up the value of `parts` on the given block param and pushes
        // it onto the stack.
        lookupBlockParam: function lookupBlockParam(blockParamId, parts) {
          this.useBlockParams = true;
          this.push(["blockParams[", blockParamId[0], "][", blockParamId[1], "]"]);
          this.resolvePath("context", parts, 1);
        },
        // [lookupData]
        //
        // On stack, before: ...
        // On stack, after: data, ...
        //
        // Push the data lookup operator
        lookupData: function lookupData(depth, parts, strict) {
          if (!depth) {
            this.pushStackLiteral("data");
          } else {
            this.pushStackLiteral("container.data(data, " + depth + ")");
          }
          this.resolvePath("data", parts, 0, true, strict);
        },
        resolvePath: function resolvePath(type, parts, i, falsy, strict) {
          var _this2 = this;
          if (this.options.strict || this.options.assumeObjects) {
            this.push(strictLookup(this.options.strict && strict, this, parts, i, type));
            return;
          }
          var len = parts.length;
          for (; i < len; i++) {
            this.replaceStack(function(current) {
              var lookup = _this2.nameLookup(current, parts[i], type);
              if (!falsy) {
                return [" != null ? ", lookup, " : ", current];
              } else {
                return [" && ", lookup];
              }
            });
          }
        },
        // [resolvePossibleLambda]
        //
        // On stack, before: value, ...
        // On stack, after: resolved value, ...
        //
        // If the `value` is a lambda, replace it on the stack by
        // the return value of the lambda
        resolvePossibleLambda: function resolvePossibleLambda() {
          this.push([this.aliasable("container.lambda"), "(", this.popStack(), ", ", this.contextName(0), ")"]);
        },
        // [pushStringParam]
        //
        // On stack, before: ...
        // On stack, after: string, currentContext, ...
        //
        // This opcode is designed for use in string mode, which
        // provides the string value of a parameter along with its
        // depth rather than resolving it immediately.
        pushStringParam: function pushStringParam(string, type) {
          this.pushContext();
          this.pushString(type);
          if (type !== "SubExpression") {
            if (typeof string === "string") {
              this.pushString(string);
            } else {
              this.pushStackLiteral(string);
            }
          }
        },
        emptyHash: function emptyHash(omitEmpty) {
          if (this.trackIds) {
            this.push("{}");
          }
          if (this.stringParams) {
            this.push("{}");
            this.push("{}");
          }
          this.pushStackLiteral(omitEmpty ? "undefined" : "{}");
        },
        pushHash: function pushHash() {
          if (this.hash) {
            this.hashes.push(this.hash);
          }
          this.hash = { values: {}, types: [], contexts: [], ids: [] };
        },
        popHash: function popHash() {
          var hash = this.hash;
          this.hash = this.hashes.pop();
          if (this.trackIds) {
            this.push(this.objectLiteral(hash.ids));
          }
          if (this.stringParams) {
            this.push(this.objectLiteral(hash.contexts));
            this.push(this.objectLiteral(hash.types));
          }
          this.push(this.objectLiteral(hash.values));
        },
        // [pushString]
        //
        // On stack, before: ...
        // On stack, after: quotedString(string), ...
        //
        // Push a quoted version of `string` onto the stack
        pushString: function pushString(string) {
          this.pushStackLiteral(this.quotedString(string));
        },
        // [pushLiteral]
        //
        // On stack, before: ...
        // On stack, after: value, ...
        //
        // Pushes a value onto the stack. This operation prevents
        // the compiler from creating a temporary variable to hold
        // it.
        pushLiteral: function pushLiteral(value) {
          this.pushStackLiteral(value);
        },
        // [pushProgram]
        //
        // On stack, before: ...
        // On stack, after: program(guid), ...
        //
        // Push a program expression onto the stack. This takes
        // a compile-time guid and converts it into a runtime-accessible
        // expression.
        pushProgram: function pushProgram(guid) {
          if (guid != null) {
            this.pushStackLiteral(this.programExpression(guid));
          } else {
            this.pushStackLiteral(null);
          }
        },
        // [registerDecorator]
        //
        // On stack, before: hash, program, params..., ...
        // On stack, after: ...
        //
        // Pops off the decorator's parameters, invokes the decorator,
        // and inserts the decorator into the decorators list.
        registerDecorator: function registerDecorator(paramSize, name2) {
          var foundDecorator = this.nameLookup("decorators", name2, "decorator"), options = this.setupHelperArgs(name2, paramSize);
          this.decorators.push(["fn = ", this.decorators.functionCall(foundDecorator, "", ["fn", "props", "container", options]), " || fn;"]);
        },
        // [invokeHelper]
        //
        // On stack, before: hash, inverse, program, params..., ...
        // On stack, after: result of helper invocation
        //
        // Pops off the helper's parameters, invokes the helper,
        // and pushes the helper's return value onto the stack.
        //
        // If the helper is not found, `helperMissing` is called.
        invokeHelper: function invokeHelper(paramSize, name2, isSimple) {
          var nonHelper = this.popStack(), helper = this.setupHelper(paramSize, name2);
          var possibleFunctionCalls = [];
          if (isSimple) {
            possibleFunctionCalls.push(helper.name);
          }
          possibleFunctionCalls.push(nonHelper);
          if (!this.options.strict) {
            possibleFunctionCalls.push(this.aliasable("container.hooks.helperMissing"));
          }
          var functionLookupCode = ["(", this.itemsSeparatedBy(possibleFunctionCalls, "||"), ")"];
          var functionCall = this.source.functionCall(functionLookupCode, "call", helper.callParams);
          this.push(functionCall);
        },
        itemsSeparatedBy: function itemsSeparatedBy(items, separator) {
          var result = [];
          result.push(items[0]);
          for (var i = 1; i < items.length; i++) {
            result.push(separator, items[i]);
          }
          return result;
        },
        // [invokeKnownHelper]
        //
        // On stack, before: hash, inverse, program, params..., ...
        // On stack, after: result of helper invocation
        //
        // This operation is used when the helper is known to exist,
        // so a `helperMissing` fallback is not required.
        invokeKnownHelper: function invokeKnownHelper(paramSize, name2) {
          var helper = this.setupHelper(paramSize, name2);
          this.push(this.source.functionCall(helper.name, "call", helper.callParams));
        },
        // [invokeAmbiguous]
        //
        // On stack, before: hash, inverse, program, params..., ...
        // On stack, after: result of disambiguation
        //
        // This operation is used when an expression like `{{foo}}`
        // is provided, but we don't know at compile-time whether it
        // is a helper or a path.
        //
        // This operation emits more code than the other options,
        // and can be avoided by passing the `knownHelpers` and
        // `knownHelpersOnly` flags at compile-time.
        invokeAmbiguous: function invokeAmbiguous(name2, helperCall) {
          this.useRegister("helper");
          var nonHelper = this.popStack();
          this.emptyHash();
          var helper = this.setupHelper(0, name2, helperCall);
          var helperName = this.lastHelper = this.nameLookup("helpers", name2, "helper");
          var lookup = ["(", "(helper = ", helperName, " || ", nonHelper, ")"];
          if (!this.options.strict) {
            lookup[0] = "(helper = ";
            lookup.push(" != null ? helper : ", this.aliasable("container.hooks.helperMissing"));
          }
          this.push(["(", lookup, helper.paramsInit ? ["),(", helper.paramsInit] : [], "),", "(typeof helper === ", this.aliasable('"function"'), " ? ", this.source.functionCall("helper", "call", helper.callParams), " : helper))"]);
        },
        // [invokePartial]
        //
        // On stack, before: context, ...
        // On stack after: result of partial invocation
        //
        // This operation pops off a context, invokes a partial with that context,
        // and pushes the result of the invocation back.
        invokePartial: function invokePartial(isDynamic, name2, indent) {
          var params = [], options = this.setupParams(name2, 1, params);
          if (isDynamic) {
            name2 = this.popStack();
            delete options.name;
          }
          if (indent) {
            options.indent = JSON.stringify(indent);
          }
          options.helpers = "helpers";
          options.partials = "partials";
          options.decorators = "container.decorators";
          if (!isDynamic) {
            params.unshift(this.nameLookup("partials", name2, "partial"));
          } else {
            params.unshift(name2);
          }
          if (this.options.compat) {
            options.depths = "depths";
          }
          options = this.objectLiteral(options);
          params.push(options);
          this.push(this.source.functionCall("container.invokePartial", "", params));
        },
        // [assignToHash]
        //
        // On stack, before: value, ..., hash, ...
        // On stack, after: ..., hash, ...
        //
        // Pops a value off the stack and assigns it to the current hash
        assignToHash: function assignToHash(key) {
          var value = this.popStack(), context = void 0, type = void 0, id = void 0;
          if (this.trackIds) {
            id = this.popStack();
          }
          if (this.stringParams) {
            type = this.popStack();
            context = this.popStack();
          }
          var hash = this.hash;
          if (context) {
            hash.contexts[key] = context;
          }
          if (type) {
            hash.types[key] = type;
          }
          if (id) {
            hash.ids[key] = id;
          }
          hash.values[key] = value;
        },
        pushId: function pushId(type, name2, child) {
          if (type === "BlockParam") {
            this.pushStackLiteral("blockParams[" + name2[0] + "].path[" + name2[1] + "]" + (child ? " + " + JSON.stringify("." + child) : ""));
          } else if (type === "PathExpression") {
            this.pushString(name2);
          } else if (type === "SubExpression") {
            this.pushStackLiteral("true");
          } else {
            this.pushStackLiteral("null");
          }
        },
        // HELPERS
        compiler: JavaScriptCompiler,
        compileChildren: function compileChildren(environment, options) {
          var children = environment.children, child = void 0, compiler = void 0;
          for (var i = 0, l = children.length; i < l; i++) {
            child = children[i];
            compiler = new this.compiler();
            var existing = this.matchExistingProgram(child);
            if (existing == null) {
              this.context.programs.push("");
              var index = this.context.programs.length;
              child.index = index;
              child.name = "program" + index;
              this.context.programs[index] = compiler.compile(child, options, this.context, !this.precompile);
              this.context.decorators[index] = compiler.decorators;
              this.context.environments[index] = child;
              this.useDepths = this.useDepths || compiler.useDepths;
              this.useBlockParams = this.useBlockParams || compiler.useBlockParams;
              child.useDepths = this.useDepths;
              child.useBlockParams = this.useBlockParams;
            } else {
              child.index = existing.index;
              child.name = "program" + existing.index;
              this.useDepths = this.useDepths || existing.useDepths;
              this.useBlockParams = this.useBlockParams || existing.useBlockParams;
            }
          }
        },
        matchExistingProgram: function matchExistingProgram(child) {
          for (var i = 0, len = this.context.environments.length; i < len; i++) {
            var environment = this.context.environments[i];
            if (environment && environment.equals(child)) {
              return environment;
            }
          }
        },
        programExpression: function programExpression(guid) {
          var child = this.environment.children[guid], programParams = [child.index, "data", child.blockParams];
          if (this.useBlockParams || this.useDepths) {
            programParams.push("blockParams");
          }
          if (this.useDepths) {
            programParams.push("depths");
          }
          return "container.program(" + programParams.join(", ") + ")";
        },
        useRegister: function useRegister(name2) {
          if (!this.registers[name2]) {
            this.registers[name2] = true;
            this.registers.list.push(name2);
          }
        },
        push: function push(expr) {
          if (!(expr instanceof Literal)) {
            expr = this.source.wrap(expr);
          }
          this.inlineStack.push(expr);
          return expr;
        },
        pushStackLiteral: function pushStackLiteral(item) {
          this.push(new Literal(item));
        },
        pushSource: function pushSource(source) {
          if (this.pendingContent) {
            this.source.push(this.appendToBuffer(this.source.quotedString(this.pendingContent), this.pendingLocation));
            this.pendingContent = void 0;
          }
          if (source) {
            this.source.push(source);
          }
        },
        replaceStack: function replaceStack(callback) {
          var prefix = ["("], stack = void 0, createdStack = void 0, usedLiteral = void 0;
          if (!this.isInline()) {
            throw new _exception2["default"]("replaceStack on non-inline");
          }
          var top = this.popStack(true);
          if (top instanceof Literal) {
            stack = [top.value];
            prefix = ["(", stack];
            usedLiteral = true;
          } else {
            createdStack = true;
            var _name = this.incrStack();
            prefix = ["((", this.push(_name), " = ", top, ")"];
            stack = this.topStack();
          }
          var item = callback.call(this, stack);
          if (!usedLiteral) {
            this.popStack();
          }
          if (createdStack) {
            this.stackSlot--;
          }
          this.push(prefix.concat(item, ")"));
        },
        incrStack: function incrStack() {
          this.stackSlot++;
          if (this.stackSlot > this.stackVars.length) {
            this.stackVars.push("stack" + this.stackSlot);
          }
          return this.topStackName();
        },
        topStackName: function topStackName() {
          return "stack" + this.stackSlot;
        },
        flushInline: function flushInline() {
          var inlineStack = this.inlineStack;
          this.inlineStack = [];
          for (var i = 0, len = inlineStack.length; i < len; i++) {
            var entry = inlineStack[i];
            if (entry instanceof Literal) {
              this.compileStack.push(entry);
            } else {
              var stack = this.incrStack();
              this.pushSource([stack, " = ", entry, ";"]);
              this.compileStack.push(stack);
            }
          }
        },
        isInline: function isInline() {
          return this.inlineStack.length;
        },
        popStack: function popStack(wrapped) {
          var inline = this.isInline(), item = (inline ? this.inlineStack : this.compileStack).pop();
          if (!wrapped && item instanceof Literal) {
            return item.value;
          } else {
            if (!inline) {
              if (!this.stackSlot) {
                throw new _exception2["default"]("Invalid stack pop");
              }
              this.stackSlot--;
            }
            return item;
          }
        },
        topStack: function topStack() {
          var stack = this.isInline() ? this.inlineStack : this.compileStack, item = stack[stack.length - 1];
          if (item instanceof Literal) {
            return item.value;
          } else {
            return item;
          }
        },
        contextName: function contextName(context) {
          if (this.useDepths && context) {
            return "depths[" + context + "]";
          } else {
            return "depth" + context;
          }
        },
        quotedString: function quotedString(str) {
          return this.source.quotedString(str);
        },
        objectLiteral: function objectLiteral(obj) {
          return this.source.objectLiteral(obj);
        },
        aliasable: function aliasable(name2) {
          var ret = this.aliases[name2];
          if (ret) {
            ret.referenceCount++;
            return ret;
          }
          ret = this.aliases[name2] = this.source.wrap(name2);
          ret.aliasable = true;
          ret.referenceCount = 1;
          return ret;
        },
        setupHelper: function setupHelper(paramSize, name2, blockHelper) {
          var params = [], paramsInit = this.setupHelperArgs(name2, paramSize, params, blockHelper);
          var foundHelper = this.nameLookup("helpers", name2, "helper"), callContext = this.aliasable(this.contextName(0) + " != null ? " + this.contextName(0) + " : (container.nullContext || {})");
          return {
            params,
            paramsInit,
            name: foundHelper,
            callParams: [callContext].concat(params)
          };
        },
        setupParams: function setupParams(helper, paramSize, params) {
          var options = {}, contexts = [], types = [], ids = [], objectArgs = !params, param = void 0;
          if (objectArgs) {
            params = [];
          }
          options.name = this.quotedString(helper);
          options.hash = this.popStack();
          if (this.trackIds) {
            options.hashIds = this.popStack();
          }
          if (this.stringParams) {
            options.hashTypes = this.popStack();
            options.hashContexts = this.popStack();
          }
          var inverse = this.popStack(), program = this.popStack();
          if (program || inverse) {
            options.fn = program || "container.noop";
            options.inverse = inverse || "container.noop";
          }
          var i = paramSize;
          while (i--) {
            param = this.popStack();
            params[i] = param;
            if (this.trackIds) {
              ids[i] = this.popStack();
            }
            if (this.stringParams) {
              types[i] = this.popStack();
              contexts[i] = this.popStack();
            }
          }
          if (objectArgs) {
            options.args = this.source.generateArray(params);
          }
          if (this.trackIds) {
            options.ids = this.source.generateArray(ids);
          }
          if (this.stringParams) {
            options.types = this.source.generateArray(types);
            options.contexts = this.source.generateArray(contexts);
          }
          if (this.options.data) {
            options.data = "data";
          }
          if (this.useBlockParams) {
            options.blockParams = "blockParams";
          }
          return options;
        },
        setupHelperArgs: function setupHelperArgs(helper, paramSize, params, useRegister) {
          var options = this.setupParams(helper, paramSize, params);
          options.loc = JSON.stringify(this.source.currentLocation);
          options = this.objectLiteral(options);
          if (useRegister) {
            this.useRegister("options");
            params.push("options");
            return ["options=", options];
          } else if (params) {
            params.push(options);
            return "";
          } else {
            return options;
          }
        }
      };
      (function() {
        var reservedWords = "break else new var case finally return void catch for switch while continue function this with default if throw delete in try do instanceof typeof abstract enum int short boolean export interface static byte extends long super char final native synchronized class float package throws const goto private transient debugger implements protected volatile double import public let yield await null true false".split(" ");
        var compilerWords = JavaScriptCompiler.RESERVED_WORDS = {};
        for (var i = 0, l = reservedWords.length; i < l; i++) {
          compilerWords[reservedWords[i]] = true;
        }
      })();
      JavaScriptCompiler.isValidJavaScriptVariableName = function(name2) {
        return !JavaScriptCompiler.RESERVED_WORDS[name2] && /^[a-zA-Z_$][0-9a-zA-Z_$]*$/.test(name2);
      };
      function strictLookup(requireTerminal, compiler, parts, i, type) {
        var stack = compiler.popStack(), len = parts.length;
        if (requireTerminal) {
          len--;
        }
        for (; i < len; i++) {
          stack = compiler.nameLookup(stack, parts[i], type);
        }
        if (requireTerminal) {
          return [compiler.aliasable("container.strict"), "(", stack, ", ", compiler.quotedString(parts[i]), ", ", JSON.stringify(compiler.source.currentLocation), " )"];
        } else {
          return stack;
        }
      }
      exports["default"] = JavaScriptCompiler;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars.js
  var require_handlebars = __commonJS({
    "node_modules/.pnpm/handlebars@4.7.8/node_modules/handlebars/dist/cjs/handlebars.js"(exports, module) {
      "use strict";
      exports.__esModule = true;
      function _interopRequireDefault(obj) {
        return obj && obj.__esModule ? obj : { "default": obj };
      }
      var _handlebarsRuntime = require_handlebars_runtime();
      var _handlebarsRuntime2 = _interopRequireDefault(_handlebarsRuntime);
      var _handlebarsCompilerAst = require_ast();
      var _handlebarsCompilerAst2 = _interopRequireDefault(_handlebarsCompilerAst);
      var _handlebarsCompilerBase = require_base2();
      var _handlebarsCompilerCompiler = require_compiler();
      var _handlebarsCompilerJavascriptCompiler = require_javascript_compiler();
      var _handlebarsCompilerJavascriptCompiler2 = _interopRequireDefault(_handlebarsCompilerJavascriptCompiler);
      var _handlebarsCompilerVisitor = require_visitor();
      var _handlebarsCompilerVisitor2 = _interopRequireDefault(_handlebarsCompilerVisitor);
      var _handlebarsNoConflict = require_no_conflict();
      var _handlebarsNoConflict2 = _interopRequireDefault(_handlebarsNoConflict);
      var _create = _handlebarsRuntime2["default"].create;
      function create() {
        var hb = _create();
        hb.compile = function(input, options) {
          return _handlebarsCompilerCompiler.compile(input, options, hb);
        };
        hb.precompile = function(input, options) {
          return _handlebarsCompilerCompiler.precompile(input, options, hb);
        };
        hb.AST = _handlebarsCompilerAst2["default"];
        hb.Compiler = _handlebarsCompilerCompiler.Compiler;
        hb.JavaScriptCompiler = _handlebarsCompilerJavascriptCompiler2["default"];
        hb.Parser = _handlebarsCompilerBase.parser;
        hb.parse = _handlebarsCompilerBase.parse;
        hb.parseWithoutProcessing = _handlebarsCompilerBase.parseWithoutProcessing;
        return hb;
      }
      var inst = create();
      inst.create = create;
      _handlebarsNoConflict2["default"](inst);
      inst.Visitor = _handlebarsCompilerVisitor2["default"];
      inst["default"] = inst;
      exports["default"] = inst;
      module.exports = exports["default"];
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/conventions.js
  var require_conventions = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/conventions.js"(exports) {
      "use strict";
      function find(list, predicate, ac) {
        if (ac === void 0) {
          ac = Array.prototype;
        }
        if (list && typeof ac.find === "function") {
          return ac.find.call(list, predicate);
        }
        for (var i = 0; i < list.length; i++) {
          if (hasOwn(list, i)) {
            var item = list[i];
            if (predicate.call(void 0, item, i, list)) {
              return item;
            }
          }
        }
      }
      function freeze(object, oc) {
        if (oc === void 0) {
          oc = Object;
        }
        if (oc && typeof oc.getOwnPropertyDescriptors === "function") {
          object = oc.create(null, oc.getOwnPropertyDescriptors(object));
        }
        return oc && typeof oc.freeze === "function" ? oc.freeze(object) : object;
      }
      function hasOwn(object, key) {
        return Object.prototype.hasOwnProperty.call(object, key);
      }
      function assign(target, source) {
        if (target === null || typeof target !== "object") {
          throw new TypeError("target is not an object");
        }
        for (var key in source) {
          if (hasOwn(source, key)) {
            target[key] = source[key];
          }
        }
        return target;
      }
      var HTML_BOOLEAN_ATTRIBUTES = freeze({
        allowfullscreen: true,
        async: true,
        autofocus: true,
        autoplay: true,
        checked: true,
        controls: true,
        default: true,
        defer: true,
        disabled: true,
        formnovalidate: true,
        hidden: true,
        ismap: true,
        itemscope: true,
        loop: true,
        multiple: true,
        muted: true,
        nomodule: true,
        novalidate: true,
        open: true,
        playsinline: true,
        readonly: true,
        required: true,
        reversed: true,
        selected: true
      });
      function isHTMLBooleanAttribute(name2) {
        return hasOwn(HTML_BOOLEAN_ATTRIBUTES, name2.toLowerCase());
      }
      var HTML_VOID_ELEMENTS = freeze({
        area: true,
        base: true,
        br: true,
        col: true,
        embed: true,
        hr: true,
        img: true,
        input: true,
        link: true,
        meta: true,
        param: true,
        source: true,
        track: true,
        wbr: true
      });
      function isHTMLVoidElement(tagName) {
        return hasOwn(HTML_VOID_ELEMENTS, tagName.toLowerCase());
      }
      var HTML_RAW_TEXT_ELEMENTS = freeze({
        script: false,
        style: false,
        textarea: true,
        title: true
      });
      function isHTMLRawTextElement(tagName) {
        var key = tagName.toLowerCase();
        return hasOwn(HTML_RAW_TEXT_ELEMENTS, key) && !HTML_RAW_TEXT_ELEMENTS[key];
      }
      function isHTMLEscapableRawTextElement(tagName) {
        var key = tagName.toLowerCase();
        return hasOwn(HTML_RAW_TEXT_ELEMENTS, key) && HTML_RAW_TEXT_ELEMENTS[key];
      }
      function isHTMLMimeType(mimeType) {
        return mimeType === MIME_TYPE.HTML;
      }
      function hasDefaultHTMLNamespace(mimeType) {
        return isHTMLMimeType(mimeType) || mimeType === MIME_TYPE.XML_XHTML_APPLICATION;
      }
      var MIME_TYPE = freeze({
        /**
         * `text/html`, the only mime type that triggers treating an XML document as HTML.
         *
         * @see https://www.iana.org/assignments/media-types/text/html IANA MimeType registration
         * @see https://en.wikipedia.org/wiki/HTML Wikipedia
         * @see https://developer.mozilla.org/en-US/docs/Web/API/DOMParser/parseFromString MDN
         * @see https://html.spec.whatwg.org/multipage/dynamic-markup-insertion.html#dom-domparser-parsefromstring
         *      WHATWG HTML Spec
         */
        HTML: "text/html",
        /**
         * `application/xml`, the standard mime type for XML documents.
         *
         * @see https://www.iana.org/assignments/media-types/application/xml IANA MimeType
         *      registration
         * @see https://tools.ietf.org/html/rfc7303#section-9.1 RFC 7303
         * @see https://en.wikipedia.org/wiki/XML_and_MIME Wikipedia
         */
        XML_APPLICATION: "application/xml",
        /**
         * `text/xml`, an alias for `application/xml`.
         *
         * @see https://tools.ietf.org/html/rfc7303#section-9.2 RFC 7303
         * @see https://www.iana.org/assignments/media-types/text/xml IANA MimeType registration
         * @see https://en.wikipedia.org/wiki/XML_and_MIME Wikipedia
         */
        XML_TEXT: "text/xml",
        /**
         * `application/xhtml+xml`, indicates an XML document that has the default HTML namespace,
         * but is parsed as an XML document.
         *
         * @see https://www.iana.org/assignments/media-types/application/xhtml+xml IANA MimeType
         *      registration
         * @see https://dom.spec.whatwg.org/#dom-domimplementation-createdocument WHATWG DOM Spec
         * @see https://en.wikipedia.org/wiki/XHTML Wikipedia
         */
        XML_XHTML_APPLICATION: "application/xhtml+xml",
        /**
         * `image/svg+xml`,
         *
         * @see https://www.iana.org/assignments/media-types/image/svg+xml IANA MimeType registration
         * @see https://www.w3.org/TR/SVG11/ W3C SVG 1.1
         * @see https://en.wikipedia.org/wiki/Scalable_Vector_Graphics Wikipedia
         */
        XML_SVG_IMAGE: "image/svg+xml"
      });
      var _MIME_TYPES = Object.keys(MIME_TYPE).map(function(key) {
        return MIME_TYPE[key];
      });
      function isValidMimeType(mimeType) {
        return _MIME_TYPES.indexOf(mimeType) > -1;
      }
      var NAMESPACE = freeze({
        /**
         * The XHTML namespace.
         *
         * @see http://www.w3.org/1999/xhtml
         */
        HTML: "http://www.w3.org/1999/xhtml",
        /**
         * The SVG namespace.
         *
         * @see http://www.w3.org/2000/svg
         */
        SVG: "http://www.w3.org/2000/svg",
        /**
         * The `xml:` namespace.
         *
         * @see http://www.w3.org/XML/1998/namespace
         */
        XML: "http://www.w3.org/XML/1998/namespace",
        /**
         * The `xmlns:` namespace.
         *
         * @see https://www.w3.org/2000/xmlns/
         */
        XMLNS: "http://www.w3.org/2000/xmlns/"
      });
      exports.assign = assign;
      exports.find = find;
      exports.freeze = freeze;
      exports.HTML_BOOLEAN_ATTRIBUTES = HTML_BOOLEAN_ATTRIBUTES;
      exports.HTML_RAW_TEXT_ELEMENTS = HTML_RAW_TEXT_ELEMENTS;
      exports.HTML_VOID_ELEMENTS = HTML_VOID_ELEMENTS;
      exports.hasDefaultHTMLNamespace = hasDefaultHTMLNamespace;
      exports.hasOwn = hasOwn;
      exports.isHTMLBooleanAttribute = isHTMLBooleanAttribute;
      exports.isHTMLRawTextElement = isHTMLRawTextElement;
      exports.isHTMLEscapableRawTextElement = isHTMLEscapableRawTextElement;
      exports.isHTMLMimeType = isHTMLMimeType;
      exports.isHTMLVoidElement = isHTMLVoidElement;
      exports.isValidMimeType = isValidMimeType;
      exports.MIME_TYPE = MIME_TYPE;
      exports.NAMESPACE = NAMESPACE;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/errors.js
  var require_errors = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/errors.js"(exports) {
      "use strict";
      var conventions = require_conventions();
      function extendError(constructor, writableName) {
        constructor.prototype = Object.create(Error.prototype, {
          constructor: { value: constructor },
          name: { value: constructor.name, enumerable: true, writable: writableName }
        });
      }
      var DOMExceptionName = conventions.freeze({
        /**
         * the default value as defined by the spec
         */
        Error: "Error",
        /**
         * @deprecated
         * Use RangeError instead.
         */
        IndexSizeError: "IndexSizeError",
        /**
         * @deprecated
         * Just to match the related static code, not part of the spec.
         */
        DomstringSizeError: "DomstringSizeError",
        HierarchyRequestError: "HierarchyRequestError",
        WrongDocumentError: "WrongDocumentError",
        InvalidCharacterError: "InvalidCharacterError",
        /**
         * @deprecated
         * Just to match the related static code, not part of the spec.
         */
        NoDataAllowedError: "NoDataAllowedError",
        NoModificationAllowedError: "NoModificationAllowedError",
        NotFoundError: "NotFoundError",
        NotSupportedError: "NotSupportedError",
        InUseAttributeError: "InUseAttributeError",
        InvalidStateError: "InvalidStateError",
        SyntaxError: "SyntaxError",
        InvalidModificationError: "InvalidModificationError",
        NamespaceError: "NamespaceError",
        /**
         * @deprecated
         * Use TypeError for invalid arguments,
         * "NotSupportedError" DOMException for unsupported operations,
         * and "NotAllowedError" DOMException for denied requests instead.
         */
        InvalidAccessError: "InvalidAccessError",
        /**
         * @deprecated
         * Just to match the related static code, not part of the spec.
         */
        ValidationError: "ValidationError",
        /**
         * @deprecated
         * Use TypeError instead.
         */
        TypeMismatchError: "TypeMismatchError",
        SecurityError: "SecurityError",
        NetworkError: "NetworkError",
        AbortError: "AbortError",
        /**
         * @deprecated
         * Just to match the related static code, not part of the spec.
         */
        URLMismatchError: "URLMismatchError",
        QuotaExceededError: "QuotaExceededError",
        TimeoutError: "TimeoutError",
        InvalidNodeTypeError: "InvalidNodeTypeError",
        DataCloneError: "DataCloneError",
        EncodingError: "EncodingError",
        NotReadableError: "NotReadableError",
        UnknownError: "UnknownError",
        ConstraintError: "ConstraintError",
        DataError: "DataError",
        TransactionInactiveError: "TransactionInactiveError",
        ReadOnlyError: "ReadOnlyError",
        VersionError: "VersionError",
        OperationError: "OperationError",
        NotAllowedError: "NotAllowedError",
        OptOutError: "OptOutError"
      });
      var DOMExceptionNames = Object.keys(DOMExceptionName);
      function isValidDomExceptionCode(value) {
        return typeof value === "number" && value >= 1 && value <= 25;
      }
      function endsWithError(value) {
        return typeof value === "string" && value.substring(value.length - DOMExceptionName.Error.length) === DOMExceptionName.Error;
      }
      function DOMException(messageOrCode, nameOrMessage) {
        if (isValidDomExceptionCode(messageOrCode)) {
          this.name = DOMExceptionNames[messageOrCode];
          this.message = nameOrMessage || "";
        } else {
          this.message = messageOrCode;
          this.name = endsWithError(nameOrMessage) ? nameOrMessage : DOMExceptionName.Error;
        }
        if (Error.captureStackTrace) Error.captureStackTrace(this, DOMException);
      }
      extendError(DOMException, true);
      Object.defineProperties(DOMException.prototype, {
        code: {
          enumerable: true,
          get: function() {
            var code = DOMExceptionNames.indexOf(this.name);
            if (isValidDomExceptionCode(code)) return code;
            return 0;
          }
        }
      });
      var ExceptionCode = {
        INDEX_SIZE_ERR: 1,
        DOMSTRING_SIZE_ERR: 2,
        HIERARCHY_REQUEST_ERR: 3,
        WRONG_DOCUMENT_ERR: 4,
        INVALID_CHARACTER_ERR: 5,
        NO_DATA_ALLOWED_ERR: 6,
        NO_MODIFICATION_ALLOWED_ERR: 7,
        NOT_FOUND_ERR: 8,
        NOT_SUPPORTED_ERR: 9,
        INUSE_ATTRIBUTE_ERR: 10,
        INVALID_STATE_ERR: 11,
        SYNTAX_ERR: 12,
        INVALID_MODIFICATION_ERR: 13,
        NAMESPACE_ERR: 14,
        INVALID_ACCESS_ERR: 15,
        VALIDATION_ERR: 16,
        TYPE_MISMATCH_ERR: 17,
        SECURITY_ERR: 18,
        NETWORK_ERR: 19,
        ABORT_ERR: 20,
        URL_MISMATCH_ERR: 21,
        QUOTA_EXCEEDED_ERR: 22,
        TIMEOUT_ERR: 23,
        INVALID_NODE_TYPE_ERR: 24,
        DATA_CLONE_ERR: 25
      };
      var entries = Object.entries(ExceptionCode);
      for (i = 0; i < entries.length; i++) {
        key = entries[i][0];
        DOMException[key] = entries[i][1];
      }
      var key;
      var i;
      function ParseError(message, locator) {
        this.message = message;
        this.locator = locator;
        if (Error.captureStackTrace) Error.captureStackTrace(this, ParseError);
      }
      extendError(ParseError);
      exports.DOMException = DOMException;
      exports.DOMExceptionName = DOMExceptionName;
      exports.ExceptionCode = ExceptionCode;
      exports.ParseError = ParseError;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/grammar.js
  var require_grammar = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/grammar.js"(exports) {
      "use strict";
      function detectUnicodeSupport(RegExpImpl) {
        try {
          if (typeof RegExpImpl !== "function") {
            RegExpImpl = RegExp;
          }
          var match = new RegExpImpl("\u{1D306}", "u").exec("\u{1D306}");
          return !!match && match[0].length === 2;
        } catch (error) {
        }
        return false;
      }
      var UNICODE_SUPPORT = detectUnicodeSupport();
      function chars(regexp) {
        if (regexp.source[0] !== "[") {
          throw new Error(regexp + " can not be used with chars");
        }
        return regexp.source.slice(1, regexp.source.lastIndexOf("]"));
      }
      function chars_without(regexp, search) {
        if (regexp.source[0] !== "[") {
          throw new Error("/" + regexp.source + "/ can not be used with chars_without");
        }
        if (!search || typeof search !== "string") {
          throw new Error(JSON.stringify(search) + " is not a valid search");
        }
        if (regexp.source.indexOf(search) === -1) {
          throw new Error('"' + search + '" is not is /' + regexp.source + "/");
        }
        if (search === "-" && regexp.source.indexOf(search) !== 1) {
          throw new Error('"' + search + '" is not at the first postion of /' + regexp.source + "/");
        }
        return new RegExp(regexp.source.replace(search, ""), UNICODE_SUPPORT ? "u" : "");
      }
      function reg(args) {
        var self = this;
        return new RegExp(
          Array.prototype.slice.call(arguments).map(function(part) {
            var isStr = typeof part === "string";
            if (isStr && self === void 0 && part === "|") {
              throw new Error("use regg instead of reg to wrap expressions with `|`!");
            }
            return isStr ? part : part.source;
          }).join(""),
          UNICODE_SUPPORT ? "mu" : "m"
        );
      }
      function regg(args) {
        if (arguments.length === 0) {
          throw new Error("no parameters provided");
        }
        return reg.apply(regg, ["(?:"].concat(Array.prototype.slice.call(arguments), [")"]));
      }
      var UNICODE_REPLACEMENT_CHARACTER = "\uFFFD";
      var Char = /[-\x09\x0A\x0D\x20-\x2C\x2E-\uD7FF\uE000-\uFFFD]/;
      if (UNICODE_SUPPORT) {
        Char = reg("[", chars(Char), "\\u{10000}-\\u{10FFFF}", "]");
      }
      var _SChar = /[\x20\x09\x0D\x0A]/;
      var SChar_s = chars(_SChar);
      var S = reg(_SChar, "+");
      var S_OPT = reg(_SChar, "*");
      var NameStartChar = /[:_a-zA-Z\xC0-\xD6\xD8-\xF6\xF8-\u02FF\u0370-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD]/;
      if (UNICODE_SUPPORT) {
        NameStartChar = reg("[", chars(NameStartChar), "\\u{10000}-\\u{10FFFF}", "]");
      }
      var NameStartChar_s = chars(NameStartChar);
      var NameChar = reg("[", NameStartChar_s, chars(/[-.0-9\xB7]/), chars(/[\u0300-\u036F\u203F-\u2040]/), "]");
      var Name = reg(NameStartChar, NameChar, "*");
      var Nmtoken = reg(NameChar, "+");
      var EntityRef = reg("&", Name, ";");
      var CharRef = regg(/&#[0-9]+;|&#x[0-9a-fA-F]+;/);
      var Reference = regg(EntityRef, "|", CharRef);
      var PEReference = reg("%", Name, ";");
      var EntityValue = regg(
        reg('"', regg(/[^%&"]/, "|", PEReference, "|", Reference), "*", '"'),
        "|",
        reg("'", regg(/[^%&']/, "|", PEReference, "|", Reference), "*", "'")
      );
      var AttValue = regg('"', regg(/[^<&"]/, "|", Reference), "*", '"', "|", "'", regg(/[^<&']/, "|", Reference), "*", "'");
      var NCNameStartChar = chars_without(NameStartChar, ":");
      var NCNameChar = chars_without(NameChar, ":");
      var NCName = reg(NCNameStartChar, NCNameChar, "*");
      var QName = reg(NCName, regg(":", NCName), "?");
      var QName_exact = reg("^", QName, "$");
      var QName_group = reg("(", QName, ")");
      var SystemLiteral = regg(/"[^"]*"|'[^']*'/);
      var PI = reg(/^<\?/, "(", Name, ")", regg(S, "(", Char, "*?)"), "?", /\?>/);
      var PubidChar = /[\x20\x0D\x0Aa-zA-Z0-9-'()+,./:=?;!*#@$_%]/;
      var PubidLiteral = regg('"', PubidChar, '*"', "|", "'", chars_without(PubidChar, "'"), "*'");
      var COMMENT_START = "<!--";
      var COMMENT_END = "-->";
      var Comment = reg(COMMENT_START, regg(chars_without(Char, "-"), "|", reg("-", chars_without(Char, "-"))), "*", COMMENT_END);
      var PCDATA = "#PCDATA";
      var Mixed = regg(
        reg(/\(/, S_OPT, PCDATA, regg(S_OPT, /\|/, S_OPT, QName), "*", S_OPT, /\)\*/),
        "|",
        reg(/\(/, S_OPT, PCDATA, S_OPT, /\)/)
      );
      var _children_quantity = /[?*+]?/;
      var children = reg(
        /\([^>]+\)/,
        _children_quantity
        /*regg(choice, '|', seq), _children_quantity*/
      );
      var contentspec = regg("EMPTY", "|", "ANY", "|", Mixed, "|", children);
      var ELEMENTDECL_START = "<!ELEMENT";
      var elementdecl = reg(ELEMENTDECL_START, S, regg(QName, "|", PEReference), S, regg(contentspec, "|", PEReference), S_OPT, ">");
      var NotationType = reg("NOTATION", S, /\(/, S_OPT, Name, regg(S_OPT, /\|/, S_OPT, Name), "*", S_OPT, /\)/);
      var Enumeration = reg(/\(/, S_OPT, Nmtoken, regg(S_OPT, /\|/, S_OPT, Nmtoken), "*", S_OPT, /\)/);
      var EnumeratedType = regg(NotationType, "|", Enumeration);
      var AttType = regg(/CDATA|ID|IDREF|IDREFS|ENTITY|ENTITIES|NMTOKEN|NMTOKENS/, "|", EnumeratedType);
      var DefaultDecl = regg(/#REQUIRED|#IMPLIED/, "|", regg(regg("#FIXED", S), "?", AttValue));
      var AttDef = regg(S, Name, S, AttType, S, DefaultDecl);
      var ATTLIST_DECL_START = "<!ATTLIST";
      var AttlistDecl = reg(ATTLIST_DECL_START, S, Name, AttDef, "*", S_OPT, ">");
      var ABOUT_LEGACY_COMPAT = "about:legacy-compat";
      var ABOUT_LEGACY_COMPAT_SystemLiteral = regg('"' + ABOUT_LEGACY_COMPAT + '"', "|", "'" + ABOUT_LEGACY_COMPAT + "'");
      var SYSTEM = "SYSTEM";
      var PUBLIC = "PUBLIC";
      var ExternalID = regg(regg(SYSTEM, S, SystemLiteral), "|", regg(PUBLIC, S, PubidLiteral, S, SystemLiteral));
      var ExternalID_match = reg(
        "^",
        regg(
          regg(SYSTEM, S, "(?<SystemLiteralOnly>", SystemLiteral, ")"),
          "|",
          regg(PUBLIC, S, "(?<PubidLiteral>", PubidLiteral, ")", S, "(?<SystemLiteral>", SystemLiteral, ")")
        )
      );
      var NDataDecl = regg(S, "NDATA", S, Name);
      var EntityDef = regg(EntityValue, "|", regg(ExternalID, NDataDecl, "?"));
      var ENTITY_DECL_START = "<!ENTITY";
      var GEDecl = reg(ENTITY_DECL_START, S, Name, S, EntityDef, S_OPT, ">");
      var PEDef = regg(EntityValue, "|", ExternalID);
      var PEDecl = reg(ENTITY_DECL_START, S, "%", S, Name, S, PEDef, S_OPT, ">");
      var EntityDecl = regg(GEDecl, "|", PEDecl);
      var PublicID = reg(PUBLIC, S, PubidLiteral);
      var NotationDecl = reg("<!NOTATION", S, Name, S, regg(ExternalID, "|", PublicID), S_OPT, ">");
      var Eq = reg(S_OPT, "=", S_OPT);
      var VersionNum = /1[.]\d+/;
      var VersionInfo = reg(S, "version", Eq, regg("'", VersionNum, "'", "|", '"', VersionNum, '"'));
      var EncName = /[A-Za-z][-A-Za-z0-9._]*/;
      var EncodingDecl = regg(S, "encoding", Eq, regg('"', EncName, '"', "|", "'", EncName, "'"));
      var SDDecl = regg(S, "standalone", Eq, regg("'", regg("yes", "|", "no"), "'", "|", '"', regg("yes", "|", "no"), '"'));
      var XMLDecl = reg(/^<\?xml/, VersionInfo, EncodingDecl, "?", SDDecl, "?", S_OPT, /\?>/);
      var DOCTYPE_DECL_START = "<!DOCTYPE";
      var CDATA_START = "<![CDATA[";
      var CDATA_END = "]]>";
      var CDStart = /<!\[CDATA\[/;
      var CDEnd = /\]\]>/;
      var CData = reg(Char, "*?", CDEnd);
      var CDSect = reg(CDStart, CData);
      exports.chars = chars;
      exports.chars_without = chars_without;
      exports.detectUnicodeSupport = detectUnicodeSupport;
      exports.reg = reg;
      exports.regg = regg;
      exports.ABOUT_LEGACY_COMPAT = ABOUT_LEGACY_COMPAT;
      exports.ABOUT_LEGACY_COMPAT_SystemLiteral = ABOUT_LEGACY_COMPAT_SystemLiteral;
      exports.AttlistDecl = AttlistDecl;
      exports.CDATA_START = CDATA_START;
      exports.CDATA_END = CDATA_END;
      exports.CDSect = CDSect;
      exports.Char = Char;
      exports.Comment = Comment;
      exports.COMMENT_START = COMMENT_START;
      exports.COMMENT_END = COMMENT_END;
      exports.DOCTYPE_DECL_START = DOCTYPE_DECL_START;
      exports.elementdecl = elementdecl;
      exports.EntityDecl = EntityDecl;
      exports.EntityValue = EntityValue;
      exports.ExternalID = ExternalID;
      exports.ExternalID_match = ExternalID_match;
      exports.Name = Name;
      exports.NotationDecl = NotationDecl;
      exports.Reference = Reference;
      exports.PEReference = PEReference;
      exports.PI = PI;
      exports.PUBLIC = PUBLIC;
      exports.PubidLiteral = PubidLiteral;
      exports.QName = QName;
      exports.QName_exact = QName_exact;
      exports.QName_group = QName_group;
      exports.S = S;
      exports.SChar_s = SChar_s;
      exports.S_OPT = S_OPT;
      exports.SYSTEM = SYSTEM;
      exports.SystemLiteral = SystemLiteral;
      exports.UNICODE_REPLACEMENT_CHARACTER = UNICODE_REPLACEMENT_CHARACTER;
      exports.UNICODE_SUPPORT = UNICODE_SUPPORT;
      exports.XMLDecl = XMLDecl;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/dom.js
  var require_dom = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/dom.js"(exports) {
      "use strict";
      var conventions = require_conventions();
      var find = conventions.find;
      var hasDefaultHTMLNamespace = conventions.hasDefaultHTMLNamespace;
      var hasOwn = conventions.hasOwn;
      var isHTMLMimeType = conventions.isHTMLMimeType;
      var isHTMLRawTextElement = conventions.isHTMLRawTextElement;
      var isHTMLVoidElement = conventions.isHTMLVoidElement;
      var MIME_TYPE = conventions.MIME_TYPE;
      var NAMESPACE = conventions.NAMESPACE;
      var PDC = Symbol();
      var errors = require_errors();
      var DOMException = errors.DOMException;
      var DOMExceptionName = errors.DOMExceptionName;
      var g = require_grammar();
      function checkSymbol(symbol) {
        if (symbol !== PDC) {
          throw new TypeError("Illegal constructor");
        }
      }
      function notEmptyString(input) {
        return input !== "";
      }
      function splitOnASCIIWhitespace(input) {
        return input ? input.split(/[\t\n\f\r ]+/).filter(notEmptyString) : [];
      }
      function orderedSetReducer(current, element) {
        if (!hasOwn(current, element)) {
          current[element] = true;
        }
        return current;
      }
      function toOrderedSet(input) {
        if (!input) return [];
        var list = splitOnASCIIWhitespace(input);
        return Object.keys(list.reduce(orderedSetReducer, {}));
      }
      function arrayIncludes(list) {
        return function(element) {
          return list && list.indexOf(element) !== -1;
        };
      }
      function validateQualifiedName(qualifiedName) {
        if (!g.QName_exact.test(qualifiedName)) {
          throw new DOMException(DOMException.INVALID_CHARACTER_ERR, 'invalid character in qualified name "' + qualifiedName + '"');
        }
      }
      function validateAndExtract(namespace, qualifiedName) {
        validateQualifiedName(qualifiedName);
        namespace = namespace || null;
        var prefix = null;
        var localName = qualifiedName;
        if (qualifiedName.indexOf(":") >= 0) {
          var splitResult = qualifiedName.split(":");
          prefix = splitResult[0];
          localName = splitResult[1];
        }
        if (prefix !== null && namespace === null) {
          throw new DOMException(DOMException.NAMESPACE_ERR, "prefix is non-null and namespace is null");
        }
        if (prefix === "xml" && namespace !== conventions.NAMESPACE.XML) {
          throw new DOMException(DOMException.NAMESPACE_ERR, 'prefix is "xml" and namespace is not the XML namespace');
        }
        if ((prefix === "xmlns" || qualifiedName === "xmlns") && namespace !== conventions.NAMESPACE.XMLNS) {
          throw new DOMException(
            DOMException.NAMESPACE_ERR,
            'either qualifiedName or prefix is "xmlns" and namespace is not the XMLNS namespace'
          );
        }
        if (namespace === conventions.NAMESPACE.XMLNS && prefix !== "xmlns" && qualifiedName !== "xmlns") {
          throw new DOMException(
            DOMException.NAMESPACE_ERR,
            'namespace is the XMLNS namespace and neither qualifiedName nor prefix is "xmlns"'
          );
        }
        return [namespace, prefix, localName];
      }
      function copy(src, dest) {
        for (var p in src) {
          if (hasOwn(src, p)) {
            dest[p] = src[p];
          }
        }
      }
      function _extends(Class, Super) {
        var pt = Class.prototype;
        if (!(pt instanceof Super)) {
          let t = function() {
          };
          t.prototype = Super.prototype;
          t = new t();
          copy(pt, t);
          Class.prototype = pt = t;
        }
        if (pt.constructor != Class) {
          if (typeof Class != "function") {
            console.error("unknown Class:" + Class);
          }
          pt.constructor = Class;
        }
      }
      var NodeType = {};
      var ELEMENT_NODE = NodeType.ELEMENT_NODE = 1;
      var ATTRIBUTE_NODE = NodeType.ATTRIBUTE_NODE = 2;
      var TEXT_NODE = NodeType.TEXT_NODE = 3;
      var CDATA_SECTION_NODE = NodeType.CDATA_SECTION_NODE = 4;
      var ENTITY_REFERENCE_NODE = NodeType.ENTITY_REFERENCE_NODE = 5;
      var ENTITY_NODE = NodeType.ENTITY_NODE = 6;
      var PROCESSING_INSTRUCTION_NODE = NodeType.PROCESSING_INSTRUCTION_NODE = 7;
      var COMMENT_NODE = NodeType.COMMENT_NODE = 8;
      var DOCUMENT_NODE = NodeType.DOCUMENT_NODE = 9;
      var DOCUMENT_TYPE_NODE = NodeType.DOCUMENT_TYPE_NODE = 10;
      var DOCUMENT_FRAGMENT_NODE = NodeType.DOCUMENT_FRAGMENT_NODE = 11;
      var NOTATION_NODE = NodeType.NOTATION_NODE = 12;
      var DocumentPosition = conventions.freeze({
        DOCUMENT_POSITION_DISCONNECTED: 1,
        DOCUMENT_POSITION_PRECEDING: 2,
        DOCUMENT_POSITION_FOLLOWING: 4,
        DOCUMENT_POSITION_CONTAINS: 8,
        DOCUMENT_POSITION_CONTAINED_BY: 16,
        DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC: 32
      });
      function commonAncestor(a, b) {
        if (b.length < a.length) return commonAncestor(b, a);
        var c = null;
        for (var n in a) {
          if (a[n] !== b[n]) return c;
          c = a[n];
        }
        return c;
      }
      function docGUID(doc) {
        if (!doc.guid) doc.guid = Math.random();
        return doc.guid;
      }
      function NodeList() {
      }
      NodeList.prototype = {
        /**
         * The number of nodes in the list. The range of valid child node indices is 0 to length-1
         * inclusive.
         *
         * @type {number}
         */
        length: 0,
        /**
         * Returns the item at `index`. If index is greater than or equal to the number of nodes in
         * the list, this returns null.
         *
         * @param index
         * Unsigned long Index into the collection.
         * @returns {Node | null}
         * The node at position `index` in the NodeList,
         * or null if that is not a valid index.
         */
        item: function(index) {
          return index >= 0 && index < this.length ? this[index] : null;
        },
        /**
         * Returns a string representation of the NodeList.
         *
         * @param {unknown} nodeFilter
         * __A filter function? Not implemented according to the spec?__.
         * @returns {string}
         * A string representation of the NodeList.
         */
        toString: function(nodeFilter) {
          for (var buf = [], i = 0; i < this.length; i++) {
            serializeToString(this[i], buf, nodeFilter);
          }
          return buf.join("");
        },
        /**
         * Filters the NodeList based on a predicate.
         *
         * @param {function(Node): boolean} predicate
         * - A predicate function to filter the NodeList.
         * @returns {Node[]}
         * An array of nodes that satisfy the predicate.
         * @private
         */
        filter: function(predicate) {
          return Array.prototype.filter.call(this, predicate);
        },
        /**
         * Returns the first index at which a given node can be found in the NodeList, or -1 if it is
         * not present.
         *
         * @param {Node} item
         * - The Node item to locate in the NodeList.
         * @returns {number}
         * The first index of the node in the NodeList; -1 if not found.
         * @private
         */
        indexOf: function(item) {
          return Array.prototype.indexOf.call(this, item);
        }
      };
      NodeList.prototype[Symbol.iterator] = function() {
        var me = this;
        var index = 0;
        return {
          next: function() {
            if (index < me.length) {
              return {
                value: me[index++],
                done: false
              };
            } else {
              return {
                done: true
              };
            }
          },
          return: function() {
            return {
              done: true
            };
          }
        };
      };
      function LiveNodeList(node, refresh) {
        this._node = node;
        this._refresh = refresh;
        _updateLiveList(this);
      }
      function _updateLiveList(list) {
        var inc = list._node._inc || list._node.ownerDocument._inc;
        if (list._inc !== inc) {
          var ls = list._refresh(list._node);
          __set__(list, "length", ls.length);
          if (!list.$$length || ls.length < list.$$length) {
            for (var i = ls.length; i in list; i++) {
              if (hasOwn(list, i)) {
                delete list[i];
              }
            }
          }
          copy(ls, list);
          list._inc = inc;
        }
      }
      LiveNodeList.prototype.item = function(i) {
        _updateLiveList(this);
        return this[i] || null;
      };
      _extends(LiveNodeList, NodeList);
      function NamedNodeMap() {
      }
      function _findNodeIndex(list, node) {
        var i = 0;
        while (i < list.length) {
          if (list[i] === node) {
            return i;
          }
          i++;
        }
      }
      function _addNamedNode(el, list, newAttr, oldAttr) {
        if (oldAttr) {
          list[_findNodeIndex(list, oldAttr)] = newAttr;
        } else {
          list[list.length] = newAttr;
          list.length++;
        }
        if (el) {
          newAttr.ownerElement = el;
          var doc = el.ownerDocument;
          if (doc) {
            oldAttr && _onRemoveAttribute(doc, el, oldAttr);
            _onAddAttribute(doc, el, newAttr);
          }
        }
      }
      function _removeNamedNode(el, list, attr) {
        var i = _findNodeIndex(list, attr);
        if (i >= 0) {
          var lastIndex = list.length - 1;
          while (i <= lastIndex) {
            list[i] = list[++i];
          }
          list.length = lastIndex;
          if (el) {
            var doc = el.ownerDocument;
            if (doc) {
              _onRemoveAttribute(doc, el, attr);
            }
            attr.ownerElement = null;
          }
        }
      }
      NamedNodeMap.prototype = {
        length: 0,
        item: NodeList.prototype.item,
        /**
         * Get an attribute by name. Note: Name is in lower case in case of HTML namespace and
         * document.
         *
         * @param {string} localName
         * The local name of the attribute.
         * @returns {Attr | null}
         * The attribute with the given local name, or null if no such attribute exists.
         * @see https://dom.spec.whatwg.org/#concept-element-attributes-get-by-name
         */
        getNamedItem: function(localName) {
          if (this._ownerElement && this._ownerElement._isInHTMLDocumentAndNamespace()) {
            localName = localName.toLowerCase();
          }
          var i = 0;
          while (i < this.length) {
            var attr = this[i];
            if (attr.nodeName === localName) {
              return attr;
            }
            i++;
          }
          return null;
        },
        /**
         * Set an attribute.
         *
         * @param {Attr} attr
         * The attribute to set.
         * @returns {Attr | null}
         * The old attribute with the same local name and namespace URI as the new one, or null if no
         * such attribute exists.
         * @throws {DOMException}
         * With code:
         * - {@link INUSE_ATTRIBUTE_ERR} - If the attribute is already an attribute of another
         * element.
         * @see https://dom.spec.whatwg.org/#concept-element-attributes-set
         */
        setNamedItem: function(attr) {
          var el = attr.ownerElement;
          if (el && el !== this._ownerElement) {
            throw new DOMException(DOMException.INUSE_ATTRIBUTE_ERR);
          }
          var oldAttr = this.getNamedItemNS(attr.namespaceURI, attr.localName);
          if (oldAttr === attr) {
            return attr;
          }
          _addNamedNode(this._ownerElement, this, attr, oldAttr);
          return oldAttr;
        },
        /**
         * Set an attribute, replacing an existing attribute with the same local name and namespace
         * URI if one exists.
         *
         * @param {Attr} attr
         * The attribute to set.
         * @returns {Attr | null}
         * The old attribute with the same local name and namespace URI as the new one, or null if no
         * such attribute exists.
         * @throws {DOMException}
         * Throws a DOMException with the name "InUseAttributeError" if the attribute is already an
         * attribute of another element.
         * @see https://dom.spec.whatwg.org/#concept-element-attributes-set
         */
        setNamedItemNS: function(attr) {
          return this.setNamedItem(attr);
        },
        /**
         * Removes an attribute specified by the local name.
         *
         * @param {string} localName
         * The local name of the attribute to be removed.
         * @returns {Attr}
         * The attribute node that was removed.
         * @throws {DOMException}
         * With code:
         * - {@link DOMException.NOT_FOUND_ERR} if no attribute with the given name is found.
         * @see https://dom.spec.whatwg.org/#dom-namednodemap-removenameditem
         * @see https://dom.spec.whatwg.org/#concept-element-attributes-remove-by-name
         */
        removeNamedItem: function(localName) {
          var attr = this.getNamedItem(localName);
          if (!attr) {
            throw new DOMException(DOMException.NOT_FOUND_ERR, localName);
          }
          _removeNamedNode(this._ownerElement, this, attr);
          return attr;
        },
        /**
         * Removes an attribute specified by the namespace and local name.
         *
         * @param {string | null} namespaceURI
         * The namespace URI of the attribute to be removed.
         * @param {string} localName
         * The local name of the attribute to be removed.
         * @returns {Attr}
         * The attribute node that was removed.
         * @throws {DOMException}
         * With code:
         * - {@link DOMException.NOT_FOUND_ERR} if no attribute with the given namespace URI and local
         * name is found.
         * @see https://dom.spec.whatwg.org/#dom-namednodemap-removenameditemns
         * @see https://dom.spec.whatwg.org/#concept-element-attributes-remove-by-namespace
         */
        removeNamedItemNS: function(namespaceURI, localName) {
          var attr = this.getNamedItemNS(namespaceURI, localName);
          if (!attr) {
            throw new DOMException(DOMException.NOT_FOUND_ERR, namespaceURI ? namespaceURI + " : " + localName : localName);
          }
          _removeNamedNode(this._ownerElement, this, attr);
          return attr;
        },
        /**
         * Get an attribute by namespace and local name.
         *
         * @param {string | null} namespaceURI
         * The namespace URI of the attribute.
         * @param {string} localName
         * The local name of the attribute.
         * @returns {Attr | null}
         * The attribute with the given namespace URI and local name, or null if no such attribute
         * exists.
         * @see https://dom.spec.whatwg.org/#concept-element-attributes-get-by-namespace
         */
        getNamedItemNS: function(namespaceURI, localName) {
          if (!namespaceURI) {
            namespaceURI = null;
          }
          var i = 0;
          while (i < this.length) {
            var node = this[i];
            if (node.localName === localName && node.namespaceURI === namespaceURI) {
              return node;
            }
            i++;
          }
          return null;
        }
      };
      NamedNodeMap.prototype[Symbol.iterator] = function() {
        var me = this;
        var index = 0;
        return {
          next: function() {
            if (index < me.length) {
              return {
                value: me[index++],
                done: false
              };
            } else {
              return {
                done: true
              };
            }
          },
          return: function() {
            return {
              done: true
            };
          }
        };
      };
      function DOMImplementation() {
      }
      DOMImplementation.prototype = {
        /**
         * Test if the DOM implementation implements a specific feature and version, as specified in
         * {@link https://www.w3.org/TR/DOM-Level-3-Core/core.html#DOMFeatures DOM Features}.
         *
         * The DOMImplementation.hasFeature() method returns a Boolean flag indicating if a given
         * feature is supported. The different implementations fairly diverged in what kind of
         * features were reported. The latest version of the spec settled to force this method to
         * always return true, where the functionality was accurate and in use.
         *
         * @deprecated
         * It is deprecated and modern browsers return true in all cases.
         * @function DOMImplementation#hasFeature
         * @param {string} feature
         * The name of the feature to test.
         * @param {string} [version]
         * This is the version number of the feature to test.
         * @returns {boolean}
         * Always returns true.
         * @see https://developer.mozilla.org/en-US/docs/Web/API/DOMImplementation/hasFeature MDN
         * @see https://www.w3.org/TR/REC-DOM-Level-1/level-one-core.html#ID-5CED94D7 DOM Level 1 Core
         * @see https://dom.spec.whatwg.org/#dom-domimplementation-hasfeature DOM Living Standard
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#ID-5CED94D7 DOM Level 3 Core
         */
        hasFeature: function(feature, version) {
          return true;
        },
        /**
         * Creates a DOM Document object of the specified type with its document element. Note that
         * based on the {@link DocumentType}
         * given to create the document, the implementation may instantiate specialized
         * {@link Document} objects that support additional features than the "Core", such as "HTML"
         * {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#DOM2HTML DOM Level 2 HTML}.
         * On the other hand, setting the {@link DocumentType} after the document was created makes
         * this very unlikely to happen. Alternatively, specialized {@link Document} creation methods,
         * such as createHTMLDocument
         * {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#DOM2HTML DOM Level 2 HTML},
         * can be used to obtain specific types of {@link Document} objects.
         *
         * __It behaves slightly different from the description in the living standard__:
         * - There is no interface/class `XMLDocument`, it returns a `Document`
         * instance (with it's `type` set to `'xml'`).
         * - `encoding`, `mode`, `origin`, `url` fields are currently not declared.
         *
         * @function DOMImplementation.createDocument
         * @param {string | null} namespaceURI
         * The
         * {@link https://www.w3.org/TR/DOM-Level-3-Core/glossary.html#dt-namespaceURI namespace URI}
         * of the document element to create or null.
         * @param {string | null} qualifiedName
         * The
         * {@link https://www.w3.org/TR/DOM-Level-3-Core/glossary.html#dt-qualifiedname qualified name}
         * of the document element to be created or null.
         * @param {DocumentType | null} [doctype=null]
         * The type of document to be created or null. When doctype is not null, its
         * {@link Node#ownerDocument} attribute is set to the document being created. Default is
         * `null`
         * @returns {Document}
         * A new {@link Document} object with its document element. If the NamespaceURI,
         * qualifiedName, and doctype are null, the returned {@link Document} is empty with no
         * document element.
         * @throws {DOMException}
         * With code:
         *
         * - `INVALID_CHARACTER_ERR`: Raised if the specified qualified name is not an XML name
         * according to {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#XML XML 1.0}.
         * - `NAMESPACE_ERR`: Raised if the qualifiedName is malformed, if the qualifiedName has a
         * prefix and the namespaceURI is null, or if the qualifiedName is null and the namespaceURI
         * is different from null, or if the qualifiedName has a prefix that is "xml" and the
         * namespaceURI is different from "{@link http://www.w3.org/XML/1998/namespace}"
         * {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#Namespaces XML Namespaces},
         * or if the DOM implementation does not support the "XML" feature but a non-null namespace
         * URI was provided, since namespaces were defined by XML.
         * - `WRONG_DOCUMENT_ERR`: Raised if doctype has already been used with a different document
         * or was created from a different implementation.
         * - `NOT_SUPPORTED_ERR`: May be raised if the implementation does not support the feature
         * "XML" and the language exposed through the Document does not support XML Namespaces (such
         * as {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#HTML40 HTML 4.01}).
         * @since DOM Level 2.
         * @see {@link #createHTMLDocument}
         * @see https://developer.mozilla.org/en-US/docs/Web/API/DOMImplementation/createDocument MDN
         * @see https://dom.spec.whatwg.org/#dom-domimplementation-createdocument DOM Living Standard
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#Level-2-Core-DOM-createDocument DOM
         *      Level 3 Core
         * @see https://www.w3.org/TR/DOM-Level-2-Core/core.html#Level-2-Core-DOM-createDocument DOM
         *      Level 2 Core (initial)
         */
        createDocument: function(namespaceURI, qualifiedName, doctype) {
          var contentType = MIME_TYPE.XML_APPLICATION;
          if (namespaceURI === NAMESPACE.HTML) {
            contentType = MIME_TYPE.XML_XHTML_APPLICATION;
          } else if (namespaceURI === NAMESPACE.SVG) {
            contentType = MIME_TYPE.XML_SVG_IMAGE;
          }
          var doc = new Document(PDC, { contentType });
          doc.implementation = this;
          doc.childNodes = new NodeList();
          doc.doctype = doctype || null;
          if (doctype) {
            doc.appendChild(doctype);
          }
          if (qualifiedName) {
            var root = doc.createElementNS(namespaceURI, qualifiedName);
            doc.appendChild(root);
          }
          return doc;
        },
        /**
         * Creates an empty DocumentType node. Entity declarations and notations are not made
         * available. Entity reference expansions and default attribute additions do not occur.
         *
         * **This behavior is slightly different from the one in the specs**:
         * - `encoding`, `mode`, `origin`, `url` fields are currently not declared.
         * - `publicId` and `systemId` contain the raw data including any possible quotes,
         *   so they can always be serialized back to the original value
         * - `internalSubset` contains the raw string between `[` and `]` if present,
         *   but is not parsed or validated in any form.
         *
         * @function DOMImplementation#createDocumentType
         * @param {string} qualifiedName
         * The {@link https://www.w3.org/TR/DOM-Level-3-Core/glossary.html#dt-qualifiedname qualified
         * name} of the document type to be created.
         * @param {string} [publicId]
         * The external subset public identifier.
         * @param {string} [systemId]
         * The external subset system identifier.
         * @param {string} [internalSubset]
         * the internal subset or an empty string if it is not present
         * @returns {DocumentType}
         * A new {@link DocumentType} node with {@link Node#ownerDocument} set to null.
         * @throws {DOMException}
         * With code:
         *
         * - `INVALID_CHARACTER_ERR`: Raised if the specified qualified name is not an XML name
         * according to {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#XML XML 1.0}.
         * - `NAMESPACE_ERR`: Raised if the qualifiedName is malformed.
         * - `NOT_SUPPORTED_ERR`: May be raised if the implementation does not support the feature
         * "XML" and the language exposed through the Document does not support XML Namespaces (such
         * as {@link https://www.w3.org/TR/DOM-Level-3-Core/references.html#HTML40 HTML 4.01}).
         * @since DOM Level 2.
         * @see https://developer.mozilla.org/en-US/docs/Web/API/DOMImplementation/createDocumentType
         *      MDN
         * @see https://dom.spec.whatwg.org/#dom-domimplementation-createdocumenttype DOM Living
         *      Standard
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#Level-3-Core-DOM-createDocType DOM
         *      Level 3 Core
         * @see https://www.w3.org/TR/DOM-Level-2-Core/core.html#Level-2-Core-DOM-createDocType DOM
         *      Level 2 Core
         * @see https://github.com/xmldom/xmldom/blob/master/CHANGELOG.md#050
         * @see https://www.w3.org/TR/DOM-Level-2-Core/#core-ID-Core-DocType-internalSubset
         * @prettierignore
         */
        createDocumentType: function(qualifiedName, publicId, systemId, internalSubset) {
          validateQualifiedName(qualifiedName);
          var node = new DocumentType(PDC);
          node.name = qualifiedName;
          node.nodeName = qualifiedName;
          node.publicId = publicId || "";
          node.systemId = systemId || "";
          node.internalSubset = internalSubset || "";
          node.childNodes = new NodeList();
          return node;
        },
        /**
         * Returns an HTML document, that might already have a basic DOM structure.
         *
         * __It behaves slightly different from the description in the living standard__:
         * - If the first argument is `false` no initial nodes are added (steps 3-7 in the specs are
         * omitted)
         * - `encoding`, `mode`, `origin`, `url` fields are currently not declared.
         *
         * @param {string | false} [title]
         * A string containing the title to give the new HTML document.
         * @returns {Document}
         * The HTML document.
         * @since WHATWG Living Standard.
         * @see {@link #createDocument}
         * @see https://dom.spec.whatwg.org/#dom-domimplementation-createhtmldocument
         * @see https://dom.spec.whatwg.org/#html-document
         */
        createHTMLDocument: function(title) {
          var doc = new Document(PDC, { contentType: MIME_TYPE.HTML });
          doc.implementation = this;
          doc.childNodes = new NodeList();
          if (title !== false) {
            doc.doctype = this.createDocumentType("html");
            doc.doctype.ownerDocument = doc;
            doc.appendChild(doc.doctype);
            var htmlNode = doc.createElement("html");
            doc.appendChild(htmlNode);
            var headNode = doc.createElement("head");
            htmlNode.appendChild(headNode);
            if (typeof title === "string") {
              var titleNode = doc.createElement("title");
              titleNode.appendChild(doc.createTextNode(title));
              headNode.appendChild(titleNode);
            }
            htmlNode.appendChild(doc.createElement("body"));
          }
          return doc;
        }
      };
      function Node(symbol) {
        checkSymbol(symbol);
      }
      Node.prototype = {
        /**
         * The first child of this node.
         *
         * @type {Node | null}
         */
        firstChild: null,
        /**
         * The last child of this node.
         *
         * @type {Node | null}
         */
        lastChild: null,
        /**
         * The previous sibling of this node.
         *
         * @type {Node | null}
         */
        previousSibling: null,
        /**
         * The next sibling of this node.
         *
         * @type {Node | null}
         */
        nextSibling: null,
        /**
         * The parent node of this node.
         *
         * @type {Node | null}
         */
        parentNode: null,
        /**
         * The parent element of this node.
         *
         * @type {Element | null}
         */
        get parentElement() {
          return this.parentNode && this.parentNode.nodeType === this.ELEMENT_NODE ? this.parentNode : null;
        },
        /**
         * The child nodes of this node.
         *
         * @type {NodeList}
         */
        childNodes: null,
        /**
         * The document object associated with this node.
         *
         * @type {Document | null}
         */
        ownerDocument: null,
        /**
         * The value of this node.
         *
         * @type {string | null}
         */
        nodeValue: null,
        /**
         * The namespace URI of this node.
         *
         * @type {string | null}
         */
        namespaceURI: null,
        /**
         * The prefix of the namespace for this node.
         *
         * @type {string | null}
         */
        prefix: null,
        /**
         * The local part of the qualified name of this node.
         *
         * @type {string | null}
         */
        localName: null,
        /**
         * The baseURI is currently always `about:blank`,
         * since that's what happens when you create a document from scratch.
         *
         * @type {'about:blank'}
         */
        baseURI: "about:blank",
        /**
         * Is true if this node is part of a document.
         *
         * @type {boolean}
         */
        get isConnected() {
          var rootNode = this.getRootNode();
          return rootNode && rootNode.nodeType === rootNode.DOCUMENT_NODE;
        },
        /**
         * Checks whether `other` is an inclusive descendant of this node.
         *
         * @param {Node | null | undefined} other
         * The node to check.
         * @returns {boolean}
         * True if `other` is an inclusive descendant of this node; false otherwise.
         * @see https://dom.spec.whatwg.org/#dom-node-contains
         */
        contains: function(other) {
          if (!other) return false;
          var parent = other;
          do {
            if (this === parent) return true;
            parent = other.parentNode;
          } while (parent);
          return false;
        },
        /**
         * @typedef GetRootNodeOptions
         * @property {boolean} [composed=false]
         */
        /**
         * Searches for the root node of this node.
         *
         * **This behavior is slightly different from the in the specs**:
         * - ignores `options.composed`, since `ShadowRoot`s are unsupported, always returns root.
         *
         * @param {GetRootNodeOptions} [options]
         * @returns {Node}
         * Root node.
         * @see https://dom.spec.whatwg.org/#dom-node-getrootnode
         * @see https://dom.spec.whatwg.org/#concept-shadow-including-root
         */
        getRootNode: function(options) {
          var parent = this;
          do {
            if (!parent.parentNode) {
              return parent;
            }
            parent = parent.parentNode;
          } while (parent);
        },
        /**
         * Checks whether the given node is equal to this node.
         *
         * @param {Node} [otherNode]
         * @see https://dom.spec.whatwg.org/#concept-node-equals
         */
        isEqualNode: function(otherNode) {
          if (!otherNode) return false;
          if (this.nodeType !== otherNode.nodeType) return false;
          switch (this.nodeType) {
            case this.DOCUMENT_TYPE_NODE:
              if (this.name !== otherNode.name) return false;
              if (this.publicId !== otherNode.publicId) return false;
              if (this.systemId !== otherNode.systemId) return false;
              break;
            case this.ELEMENT_NODE:
              if (this.namespaceURI !== otherNode.namespaceURI) return false;
              if (this.prefix !== otherNode.prefix) return false;
              if (this.localName !== otherNode.localName) return false;
              if (this.attributes.length !== otherNode.attributes.length) return false;
              for (var i = 0; i < this.attributes.length; i++) {
                var attr = this.attributes.item(i);
                if (!attr.isEqualNode(otherNode.getAttributeNodeNS(attr.namespaceURI, attr.localName))) {
                  return false;
                }
              }
              break;
            case this.ATTRIBUTE_NODE:
              if (this.namespaceURI !== otherNode.namespaceURI) return false;
              if (this.localName !== otherNode.localName) return false;
              if (this.value !== otherNode.value) return false;
              break;
            case this.PROCESSING_INSTRUCTION_NODE:
              if (this.target !== otherNode.target || this.data !== otherNode.data) {
                return false;
              }
              break;
            case this.TEXT_NODE:
            case this.COMMENT_NODE:
              if (this.data !== otherNode.data) return false;
              break;
          }
          if (this.childNodes.length !== otherNode.childNodes.length) {
            return false;
          }
          for (var i = 0; i < this.childNodes.length; i++) {
            if (!this.childNodes[i].isEqualNode(otherNode.childNodes[i])) {
              return false;
            }
          }
          return true;
        },
        /**
         * Checks whether or not the given node is this node.
         *
         * @param {Node} [otherNode]
         */
        isSameNode: function(otherNode) {
          return this === otherNode;
        },
        /**
         * Inserts a node before a reference node as a child of this node.
         *
         * @param {Node} newChild
         * The new child node to be inserted.
         * @param {Node | null} refChild
         * The reference node before which newChild will be inserted.
         * @returns {Node}
         * The new child node successfully inserted.
         * @throws {DOMException}
         * Throws a DOMException if inserting the node would result in a DOM tree that is not
         * well-formed, or if `child` is provided but is not a child of `parent`.
         * See {@link _insertBefore} for more details.
         * @since Modified in DOM L2
         */
        insertBefore: function(newChild, refChild) {
          return _insertBefore(this, newChild, refChild);
        },
        /**
         * Replaces an old child node with a new child node within this node.
         *
         * @param {Node} newChild
         * The new node that is to replace the old node.
         * If it already exists in the DOM, it is removed from its original position.
         * @param {Node} oldChild
         * The existing child node to be replaced.
         * @returns {Node}
         * Returns the replaced child node.
         * @throws {DOMException}
         * Throws a DOMException if replacing the node would result in a DOM tree that is not
         * well-formed, or if `oldChild` is not a child of `this`.
         * This can also occur if the pre-replacement validity assertion fails.
         * See {@link _insertBefore}, {@link Node.removeChild}, and
         * {@link assertPreReplacementValidityInDocument} for more details.
         * @see https://dom.spec.whatwg.org/#concept-node-replace
         */
        replaceChild: function(newChild, oldChild) {
          _insertBefore(this, newChild, oldChild, assertPreReplacementValidityInDocument);
          if (oldChild) {
            this.removeChild(oldChild);
          }
        },
        /**
         * Removes an existing child node from this node.
         *
         * @param {Node} oldChild
         * The child node to be removed.
         * @returns {Node}
         * Returns the removed child node.
         * @throws {DOMException}
         * Throws a DOMException if `oldChild` is not a child of `this`.
         * See {@link _removeChild} for more details.
         */
        removeChild: function(oldChild) {
          return _removeChild(this, oldChild);
        },
        /**
         * Appends a child node to this node.
         *
         * @param {Node} newChild
         * The child node to be appended to this node.
         * If it already exists in the DOM, it is removed from its original position.
         * @returns {Node}
         * Returns the appended child node.
         * @throws {DOMException}
         * Throws a DOMException if appending the node would result in a DOM tree that is not
         * well-formed, or if `newChild` is not a valid Node.
         * See {@link insertBefore} for more details.
         */
        appendChild: function(newChild) {
          return this.insertBefore(newChild, null);
        },
        /**
         * Determines whether this node has any child nodes.
         *
         * @returns {boolean}
         * Returns true if this node has any child nodes, and false otherwise.
         */
        hasChildNodes: function() {
          return this.firstChild != null;
        },
        /**
         * Creates a copy of the calling node.
         *
         * @param {boolean} deep
         * If true, the contents of the node are recursively copied.
         * If false, only the node itself (and its attributes, if it is an element) are copied.
         * @returns {Node}
         * Returns the newly created copy of the node.
         * @throws {DOMException}
         * May throw a DOMException if operations within {@link Element#setAttributeNode} or
         * {@link Node#appendChild} (which are potentially invoked in this method) do not meet their
         * specific constraints.
         * @see {@link cloneNode}
         */
        cloneNode: function(deep) {
          return cloneNode(this.ownerDocument || this, this, deep);
        },
        /**
         * Puts the specified node and all of its subtree into a "normalized" form. In a normalized
         * subtree, no text nodes in the subtree are empty and there are no adjacent text nodes.
         *
         * Specifically, this method merges any adjacent text nodes (i.e., nodes for which `nodeType`
         * is `TEXT_NODE`) into a single node with the combined data. It also removes any empty text
         * nodes.
         *
         * This method operates recursively, so it also normalizes any and all descendent nodes within
         * the subtree.
         *
         * @throws {DOMException}
         * May throw a DOMException if operations within removeChild or appendData (which are
         * potentially invoked in this method) do not meet their specific constraints.
         * @since Modified in DOM Level 2
         * @see {@link Node.removeChild}
         * @see {@link CharacterData.appendData}
         */
        normalize: function() {
          var child = this.firstChild;
          while (child) {
            var next = child.nextSibling;
            if (next && next.nodeType == TEXT_NODE && child.nodeType == TEXT_NODE) {
              this.removeChild(next);
              child.appendData(next.data);
            } else {
              child.normalize();
              child = next;
            }
          }
        },
        /**
         * Checks whether the DOM implementation implements a specific feature and its version.
         *
         * @deprecated
         * Since `DOMImplementation.hasFeature` is deprecated and always returns true.
         * @param {string} feature
         * The package name of the feature to test. This is the same name that can be passed to the
         * method `hasFeature` on `DOMImplementation`.
         * @param {string} version
         * This is the version number of the package name to test.
         * @returns {boolean}
         * Returns true in all cases in the current implementation.
         * @since Introduced in DOM Level 2
         * @see {@link DOMImplementation.hasFeature}
         */
        isSupported: function(feature, version) {
          return this.ownerDocument.implementation.hasFeature(feature, version);
        },
        /**
         * Look up the prefix associated to the given namespace URI, starting from this node.
         * **The default namespace declarations are ignored by this method.**
         * See Namespace Prefix Lookup for details on the algorithm used by this method.
         *
         * **This behavior is different from the in the specs**:
         * - no node type specific handling
         * - uses the internal attribute _nsMap for resolving namespaces that is updated when changing attributes
         *
         * @param {string | null} namespaceURI
         * The namespace URI for which to find the associated prefix.
         * @returns {string | null}
         * The associated prefix, if found; otherwise, null.
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#Node3-lookupNamespacePrefix
         * @see https://www.w3.org/TR/DOM-Level-3-Core/namespaces-algorithms.html#lookupNamespacePrefixAlgo
         * @see https://dom.spec.whatwg.org/#dom-node-lookupprefix
         * @see https://github.com/xmldom/xmldom/issues/322
         * @prettierignore
         */
        lookupPrefix: function(namespaceURI) {
          var el = this;
          while (el) {
            var map = el._nsMap;
            if (map) {
              for (var n in map) {
                if (hasOwn(map, n) && map[n] === namespaceURI) {
                  return n;
                }
              }
            }
            el = el.nodeType == ATTRIBUTE_NODE ? el.ownerDocument : el.parentNode;
          }
          return null;
        },
        /**
         * This function is used to look up the namespace URI associated with the given prefix,
         * starting from this node.
         *
         * **This behavior is different from the in the specs**:
         * - no node type specific handling
         * - uses the internal attribute _nsMap for resolving namespaces that is updated when changing attributes
         *
         * @param {string | null} prefix
         * The prefix for which to find the associated namespace URI.
         * @returns {string | null}
         * The associated namespace URI, if found; otherwise, null.
         * @since DOM Level 3
         * @see https://dom.spec.whatwg.org/#dom-node-lookupnamespaceuri
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#Node3-lookupNamespaceURI
         * @prettierignore
         */
        lookupNamespaceURI: function(prefix) {
          var el = this;
          while (el) {
            var map = el._nsMap;
            if (map) {
              if (hasOwn(map, prefix)) {
                return map[prefix];
              }
            }
            el = el.nodeType == ATTRIBUTE_NODE ? el.ownerDocument : el.parentNode;
          }
          return null;
        },
        /**
         * Determines whether the given namespace URI is the default namespace.
         *
         * The function works by looking up the prefix associated with the given namespace URI. If no
         * prefix is found (i.e., the namespace URI is not registered in the namespace map of this
         * node or any of its ancestors), it returns `true`, implying the namespace URI is considered
         * the default.
         *
         * **This behavior is different from the in the specs**:
         * - no node type specific handling
         * - uses the internal attribute _nsMap for resolving namespaces that is updated when changing attributes
         *
         * @param {string | null} namespaceURI
         * The namespace URI to be checked.
         * @returns {boolean}
         * Returns true if the given namespace URI is the default namespace, false otherwise.
         * @since DOM Level 3
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#Node3-isDefaultNamespace
         * @see https://dom.spec.whatwg.org/#dom-node-isdefaultnamespace
         * @prettierignore
         */
        isDefaultNamespace: function(namespaceURI) {
          var prefix = this.lookupPrefix(namespaceURI);
          return prefix == null;
        },
        /**
         * Compares the reference node with a node with regard to their position in the document and
         * according to the document order.
         *
         * @param {Node} other
         * The node to compare the reference node to.
         * @returns {number}
         * Returns how the node is positioned relatively to the reference node according to the
         * bitmask. 0 if reference node and given node are the same.
         * @since DOM Level 3
         * @see https://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407/core.html#Node3-compare
         * @see https://dom.spec.whatwg.org/#dom-node-comparedocumentposition
         */
        compareDocumentPosition: function(other) {
          if (this === other) return 0;
          var node1 = other;
          var node2 = this;
          var attr1 = null;
          var attr2 = null;
          if (node1 instanceof Attr) {
            attr1 = node1;
            node1 = attr1.ownerElement;
          }
          if (node2 instanceof Attr) {
            attr2 = node2;
            node2 = attr2.ownerElement;
            if (attr1 && node1 && node2 === node1) {
              for (var i = 0, attr; attr = node2.attributes[i]; i++) {
                if (attr === attr1)
                  return DocumentPosition.DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC + DocumentPosition.DOCUMENT_POSITION_PRECEDING;
                if (attr === attr2)
                  return DocumentPosition.DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC + DocumentPosition.DOCUMENT_POSITION_FOLLOWING;
              }
            }
          }
          if (!node1 || !node2 || node2.ownerDocument !== node1.ownerDocument) {
            return DocumentPosition.DOCUMENT_POSITION_DISCONNECTED + DocumentPosition.DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC + (docGUID(node2.ownerDocument) > docGUID(node1.ownerDocument) ? DocumentPosition.DOCUMENT_POSITION_FOLLOWING : DocumentPosition.DOCUMENT_POSITION_PRECEDING);
          }
          if (attr2 && node1 === node2) {
            return DocumentPosition.DOCUMENT_POSITION_CONTAINS + DocumentPosition.DOCUMENT_POSITION_PRECEDING;
          }
          if (attr1 && node1 === node2) {
            return DocumentPosition.DOCUMENT_POSITION_CONTAINED_BY + DocumentPosition.DOCUMENT_POSITION_FOLLOWING;
          }
          var chain1 = [];
          var ancestor1 = node1.parentNode;
          while (ancestor1) {
            if (!attr2 && ancestor1 === node2) {
              return DocumentPosition.DOCUMENT_POSITION_CONTAINED_BY + DocumentPosition.DOCUMENT_POSITION_FOLLOWING;
            }
            chain1.push(ancestor1);
            ancestor1 = ancestor1.parentNode;
          }
          chain1.reverse();
          var chain2 = [];
          var ancestor2 = node2.parentNode;
          while (ancestor2) {
            if (!attr1 && ancestor2 === node1) {
              return DocumentPosition.DOCUMENT_POSITION_CONTAINS + DocumentPosition.DOCUMENT_POSITION_PRECEDING;
            }
            chain2.push(ancestor2);
            ancestor2 = ancestor2.parentNode;
          }
          chain2.reverse();
          var ca = commonAncestor(chain1, chain2);
          for (var n in ca.childNodes) {
            var child = ca.childNodes[n];
            if (child === node2) return DocumentPosition.DOCUMENT_POSITION_FOLLOWING;
            if (child === node1) return DocumentPosition.DOCUMENT_POSITION_PRECEDING;
            if (chain2.indexOf(child) >= 0) return DocumentPosition.DOCUMENT_POSITION_FOLLOWING;
            if (chain1.indexOf(child) >= 0) return DocumentPosition.DOCUMENT_POSITION_PRECEDING;
          }
          return 0;
        }
      };
      function _xmlEncoder(c) {
        return c == "<" && "&lt;" || c == ">" && "&gt;" || c == "&" && "&amp;" || c == '"' && "&quot;" || "&#" + c.charCodeAt() + ";";
      }
      copy(NodeType, Node);
      copy(NodeType, Node.prototype);
      copy(DocumentPosition, Node);
      copy(DocumentPosition, Node.prototype);
      function _visitNode(node, callback) {
        if (callback(node)) {
          return true;
        }
        if (node = node.firstChild) {
          do {
            if (_visitNode(node, callback)) {
              return true;
            }
          } while (node = node.nextSibling);
        }
      }
      function Document(symbol, options) {
        checkSymbol(symbol);
        var opt = options || {};
        this.ownerDocument = this;
        this.contentType = opt.contentType || MIME_TYPE.XML_APPLICATION;
        this.type = isHTMLMimeType(this.contentType) ? "html" : "xml";
      }
      function _onAddAttribute(doc, el, newAttr) {
        doc && doc._inc++;
        var ns = newAttr.namespaceURI;
        if (ns === NAMESPACE.XMLNS) {
          el._nsMap[newAttr.prefix ? newAttr.localName : ""] = newAttr.value;
        }
      }
      function _onRemoveAttribute(doc, el, newAttr, remove) {
        doc && doc._inc++;
        var ns = newAttr.namespaceURI;
        if (ns === NAMESPACE.XMLNS) {
          delete el._nsMap[newAttr.prefix ? newAttr.localName : ""];
        }
      }
      function _onUpdateChild(doc, parent, newChild) {
        if (doc && doc._inc) {
          doc._inc++;
          var childNodes = parent.childNodes;
          if (newChild && !newChild.nextSibling) {
            childNodes[childNodes.length++] = newChild;
          } else {
            var child = parent.firstChild;
            var i = 0;
            while (child) {
              childNodes[i++] = child;
              child = child.nextSibling;
            }
            childNodes.length = i;
            delete childNodes[childNodes.length];
          }
        }
      }
      function _removeChild(parentNode, child) {
        if (parentNode !== child.parentNode) {
          throw new DOMException(DOMException.NOT_FOUND_ERR, "child's parent is not parent");
        }
        var oldPreviousSibling = child.previousSibling;
        var oldNextSibling = child.nextSibling;
        if (oldPreviousSibling) {
          oldPreviousSibling.nextSibling = oldNextSibling;
        } else {
          parentNode.firstChild = oldNextSibling;
        }
        if (oldNextSibling) {
          oldNextSibling.previousSibling = oldPreviousSibling;
        } else {
          parentNode.lastChild = oldPreviousSibling;
        }
        _onUpdateChild(parentNode.ownerDocument, parentNode);
        child.parentNode = null;
        child.previousSibling = null;
        child.nextSibling = null;
        return child;
      }
      function hasValidParentNodeType(node) {
        return node && (node.nodeType === Node.DOCUMENT_NODE || node.nodeType === Node.DOCUMENT_FRAGMENT_NODE || node.nodeType === Node.ELEMENT_NODE);
      }
      function hasInsertableNodeType(node) {
        return node && (node.nodeType === Node.CDATA_SECTION_NODE || node.nodeType === Node.COMMENT_NODE || node.nodeType === Node.DOCUMENT_FRAGMENT_NODE || node.nodeType === Node.DOCUMENT_TYPE_NODE || node.nodeType === Node.ELEMENT_NODE || node.nodeType === Node.PROCESSING_INSTRUCTION_NODE || node.nodeType === Node.TEXT_NODE);
      }
      function isDocTypeNode(node) {
        return node && node.nodeType === Node.DOCUMENT_TYPE_NODE;
      }
      function isElementNode(node) {
        return node && node.nodeType === Node.ELEMENT_NODE;
      }
      function isTextNode(node) {
        return node && node.nodeType === Node.TEXT_NODE;
      }
      function isElementInsertionPossible(doc, child) {
        var parentChildNodes = doc.childNodes || [];
        if (find(parentChildNodes, isElementNode) || isDocTypeNode(child)) {
          return false;
        }
        var docTypeNode = find(parentChildNodes, isDocTypeNode);
        return !(child && docTypeNode && parentChildNodes.indexOf(docTypeNode) > parentChildNodes.indexOf(child));
      }
      function isElementReplacementPossible(doc, child) {
        var parentChildNodes = doc.childNodes || [];
        function hasElementChildThatIsNotChild(node) {
          return isElementNode(node) && node !== child;
        }
        if (find(parentChildNodes, hasElementChildThatIsNotChild)) {
          return false;
        }
        var docTypeNode = find(parentChildNodes, isDocTypeNode);
        return !(child && docTypeNode && parentChildNodes.indexOf(docTypeNode) > parentChildNodes.indexOf(child));
      }
      function assertPreInsertionValidity1to5(parent, node, child) {
        if (!hasValidParentNodeType(parent)) {
          throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Unexpected parent node type " + parent.nodeType);
        }
        if (child && child.parentNode !== parent) {
          throw new DOMException(DOMException.NOT_FOUND_ERR, "child not in parent");
        }
        if (
          // 4. If `node` is not a DocumentFragment, DocumentType, Element, or CharacterData node, then throw a "HierarchyRequestError" DOMException.
          !hasInsertableNodeType(node) || // 5. If either `node` is a Text node and `parent` is a document,
          // the sax parser currently adds top level text nodes, this will be fixed in 0.9.0
          // || (node.nodeType === Node.TEXT_NODE && parent.nodeType === Node.DOCUMENT_NODE)
          // or `node` is a doctype and `parent` is not a document, then throw a "HierarchyRequestError" DOMException.
          isDocTypeNode(node) && parent.nodeType !== Node.DOCUMENT_NODE
        ) {
          throw new DOMException(
            DOMException.HIERARCHY_REQUEST_ERR,
            "Unexpected node type " + node.nodeType + " for parent node type " + parent.nodeType
          );
        }
      }
      function assertPreInsertionValidityInDocument(parent, node, child) {
        var parentChildNodes = parent.childNodes || [];
        var nodeChildNodes = node.childNodes || [];
        if (node.nodeType === Node.DOCUMENT_FRAGMENT_NODE) {
          var nodeChildElements = nodeChildNodes.filter(isElementNode);
          if (nodeChildElements.length > 1 || find(nodeChildNodes, isTextNode)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "More than one element or text in fragment");
          }
          if (nodeChildElements.length === 1 && !isElementInsertionPossible(parent, child)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Element in fragment can not be inserted before doctype");
          }
        }
        if (isElementNode(node)) {
          if (!isElementInsertionPossible(parent, child)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Only one element can be added and only after doctype");
          }
        }
        if (isDocTypeNode(node)) {
          if (find(parentChildNodes, isDocTypeNode)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Only one doctype is allowed");
          }
          var parentElementChild = find(parentChildNodes, isElementNode);
          if (child && parentChildNodes.indexOf(parentElementChild) < parentChildNodes.indexOf(child)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Doctype can only be inserted before an element");
          }
          if (!child && parentElementChild) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Doctype can not be appended since element is present");
          }
        }
      }
      function assertPreReplacementValidityInDocument(parent, node, child) {
        var parentChildNodes = parent.childNodes || [];
        var nodeChildNodes = node.childNodes || [];
        if (node.nodeType === Node.DOCUMENT_FRAGMENT_NODE) {
          var nodeChildElements = nodeChildNodes.filter(isElementNode);
          if (nodeChildElements.length > 1 || find(nodeChildNodes, isTextNode)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "More than one element or text in fragment");
          }
          if (nodeChildElements.length === 1 && !isElementReplacementPossible(parent, child)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Element in fragment can not be inserted before doctype");
          }
        }
        if (isElementNode(node)) {
          if (!isElementReplacementPossible(parent, child)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Only one element can be added and only after doctype");
          }
        }
        if (isDocTypeNode(node)) {
          let hasDoctypeChildThatIsNotChild = function(node2) {
            return isDocTypeNode(node2) && node2 !== child;
          };
          if (find(parentChildNodes, hasDoctypeChildThatIsNotChild)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Only one doctype is allowed");
          }
          var parentElementChild = find(parentChildNodes, isElementNode);
          if (child && parentChildNodes.indexOf(parentElementChild) < parentChildNodes.indexOf(child)) {
            throw new DOMException(DOMException.HIERARCHY_REQUEST_ERR, "Doctype can only be inserted before an element");
          }
        }
      }
      function _insertBefore(parent, node, child, _inDocumentAssertion) {
        assertPreInsertionValidity1to5(parent, node, child);
        if (parent.nodeType === Node.DOCUMENT_NODE) {
          (_inDocumentAssertion || assertPreInsertionValidityInDocument)(parent, node, child);
        }
        var cp = node.parentNode;
        if (cp) {
          cp.removeChild(node);
        }
        if (node.nodeType === DOCUMENT_FRAGMENT_NODE) {
          var newFirst = node.firstChild;
          if (newFirst == null) {
            return node;
          }
          var newLast = node.lastChild;
        } else {
          newFirst = newLast = node;
        }
        var pre = child ? child.previousSibling : parent.lastChild;
        newFirst.previousSibling = pre;
        newLast.nextSibling = child;
        if (pre) {
          pre.nextSibling = newFirst;
        } else {
          parent.firstChild = newFirst;
        }
        if (child == null) {
          parent.lastChild = newLast;
        } else {
          child.previousSibling = newLast;
        }
        do {
          newFirst.parentNode = parent;
        } while (newFirst !== newLast && (newFirst = newFirst.nextSibling));
        _onUpdateChild(parent.ownerDocument || parent, parent, node);
        if (node.nodeType == DOCUMENT_FRAGMENT_NODE) {
          node.firstChild = node.lastChild = null;
        }
        return node;
      }
      Document.prototype = {
        /**
         * The implementation that created this document.
         *
         * @type DOMImplementation
         * @readonly
         */
        implementation: null,
        nodeName: "#document",
        nodeType: DOCUMENT_NODE,
        /**
         * The DocumentType node of the document.
         *
         * @type DocumentType
         * @readonly
         */
        doctype: null,
        documentElement: null,
        _inc: 1,
        insertBefore: function(newChild, refChild) {
          if (newChild.nodeType === DOCUMENT_FRAGMENT_NODE) {
            var child = newChild.firstChild;
            while (child) {
              var next = child.nextSibling;
              this.insertBefore(child, refChild);
              child = next;
            }
            return newChild;
          }
          _insertBefore(this, newChild, refChild);
          newChild.ownerDocument = this;
          if (this.documentElement === null && newChild.nodeType === ELEMENT_NODE) {
            this.documentElement = newChild;
          }
          return newChild;
        },
        removeChild: function(oldChild) {
          var removed = _removeChild(this, oldChild);
          if (removed === this.documentElement) {
            this.documentElement = null;
          }
          return removed;
        },
        replaceChild: function(newChild, oldChild) {
          _insertBefore(this, newChild, oldChild, assertPreReplacementValidityInDocument);
          newChild.ownerDocument = this;
          if (oldChild) {
            this.removeChild(oldChild);
          }
          if (isElementNode(newChild)) {
            this.documentElement = newChild;
          }
        },
        // Introduced in DOM Level 2:
        importNode: function(importedNode, deep) {
          return importNode(this, importedNode, deep);
        },
        // Introduced in DOM Level 2:
        getElementById: function(id) {
          var rtv = null;
          _visitNode(this.documentElement, function(node) {
            if (node.nodeType == ELEMENT_NODE) {
              if (node.getAttribute("id") == id) {
                rtv = node;
                return true;
              }
            }
          });
          return rtv;
        },
        /**
         * Creates a new `Element` that is owned by this `Document`.
         * In HTML Documents `localName` is the lower cased `tagName`,
         * otherwise no transformation is being applied.
         * When `contentType` implies the HTML namespace, it will be set as `namespaceURI`.
         *
         * __This implementation differs from the specification:__ - The provided name is not checked
         * against the `Name` production,
         * so no related error will be thrown.
         * - There is no interface `HTMLElement`, it is always an `Element`.
         * - There is no support for a second argument to indicate using custom elements.
         *
         * @param {string} tagName
         * @returns {Element}
         * @see https://developer.mozilla.org/en-US/docs/Web/API/Document/createElement
         * @see https://dom.spec.whatwg.org/#dom-document-createelement
         * @see https://dom.spec.whatwg.org/#concept-create-element
         */
        createElement: function(tagName) {
          var node = new Element(PDC);
          node.ownerDocument = this;
          if (this.type === "html") {
            tagName = tagName.toLowerCase();
          }
          if (hasDefaultHTMLNamespace(this.contentType)) {
            node.namespaceURI = NAMESPACE.HTML;
          }
          node.nodeName = tagName;
          node.tagName = tagName;
          node.localName = tagName;
          node.childNodes = new NodeList();
          var attrs = node.attributes = new NamedNodeMap();
          attrs._ownerElement = node;
          return node;
        },
        /**
         * @returns {DocumentFragment}
         */
        createDocumentFragment: function() {
          var node = new DocumentFragment(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          return node;
        },
        /**
         * @param {string} data
         * @returns {Text}
         */
        createTextNode: function(data) {
          var node = new Text(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.appendData(data);
          return node;
        },
        /**
         * @param {string} data
         * @returns {Comment}
         */
        createComment: function(data) {
          var node = new Comment(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.appendData(data);
          return node;
        },
        /**
         * @param {string} data
         * @returns {CDATASection}
         */
        createCDATASection: function(data) {
          var node = new CDATASection(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.appendData(data);
          return node;
        },
        /**
         * @param {string} target
         * @param {string} data
         * @returns {ProcessingInstruction}
         */
        createProcessingInstruction: function(target, data) {
          var node = new ProcessingInstruction(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.nodeName = node.target = target;
          node.nodeValue = node.data = data;
          return node;
        },
        /**
         * Creates an `Attr` node that is owned by this document.
         * In HTML Documents `localName` is the lower cased `name`,
         * otherwise no transformation is being applied.
         *
         * __This implementation differs from the specification:__ - The provided name is not checked
         * against the `Name` production,
         * so no related error will be thrown.
         *
         * @param {string} name
         * @returns {Attr}
         * @see https://developer.mozilla.org/en-US/docs/Web/API/Document/createAttribute
         * @see https://dom.spec.whatwg.org/#dom-document-createattribute
         */
        createAttribute: function(name2) {
          if (!g.QName_exact.test(name2)) {
            throw new DOMException(DOMException.INVALID_CHARACTER_ERR, 'invalid character in name "' + name2 + '"');
          }
          if (this.type === "html") {
            name2 = name2.toLowerCase();
          }
          return this._createAttribute(name2);
        },
        _createAttribute: function(name2) {
          var node = new Attr(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.name = name2;
          node.nodeName = name2;
          node.localName = name2;
          node.specified = true;
          return node;
        },
        /**
         * Creates an EntityReference object.
         * The current implementation does not fill the `childNodes` with those of the corresponding
         * `Entity`
         *
         * @deprecated
         * In DOM Level 4.
         * @param {string} name
         * The name of the entity to reference. No namespace well-formedness checks are performed.
         * @returns {EntityReference}
         * @throws {DOMException}
         * With code `INVALID_CHARACTER_ERR` when `name` is not valid.
         * @throws {DOMException}
         * with code `NOT_SUPPORTED_ERR` when the document is of type `html`
         * @see https://www.w3.org/TR/DOM-Level-3-Core/core.html#ID-392B75AE
         */
        createEntityReference: function(name2) {
          if (!g.Name.test(name2)) {
            throw new DOMException(DOMException.INVALID_CHARACTER_ERR, 'not a valid xml name "' + name2 + '"');
          }
          if (this.type === "html") {
            throw new DOMException("document is an html document", DOMExceptionName.NotSupportedError);
          }
          var node = new EntityReference(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.nodeName = name2;
          return node;
        },
        // Introduced in DOM Level 2:
        /**
         * @param {string} namespaceURI
         * @param {string} qualifiedName
         * @returns {Element}
         */
        createElementNS: function(namespaceURI, qualifiedName) {
          var validated = validateAndExtract(namespaceURI, qualifiedName);
          var node = new Element(PDC);
          var attrs = node.attributes = new NamedNodeMap();
          node.childNodes = new NodeList();
          node.ownerDocument = this;
          node.nodeName = qualifiedName;
          node.tagName = qualifiedName;
          node.namespaceURI = validated[0];
          node.prefix = validated[1];
          node.localName = validated[2];
          attrs._ownerElement = node;
          return node;
        },
        // Introduced in DOM Level 2:
        /**
         * @param {string} namespaceURI
         * @param {string} qualifiedName
         * @returns {Attr}
         */
        createAttributeNS: function(namespaceURI, qualifiedName) {
          var validated = validateAndExtract(namespaceURI, qualifiedName);
          var node = new Attr(PDC);
          node.ownerDocument = this;
          node.childNodes = new NodeList();
          node.nodeName = qualifiedName;
          node.name = qualifiedName;
          node.specified = true;
          node.namespaceURI = validated[0];
          node.prefix = validated[1];
          node.localName = validated[2];
          return node;
        }
      };
      _extends(Document, Node);
      function Element(symbol) {
        checkSymbol(symbol);
        this._nsMap = /* @__PURE__ */ Object.create(null);
      }
      Element.prototype = {
        nodeType: ELEMENT_NODE,
        /**
         * The attributes of this element.
         *
         * @type {NamedNodeMap | null}
         */
        attributes: null,
        getQualifiedName: function() {
          return this.prefix ? this.prefix + ":" + this.localName : this.localName;
        },
        _isInHTMLDocumentAndNamespace: function() {
          return this.ownerDocument.type === "html" && this.namespaceURI === NAMESPACE.HTML;
        },
        /**
         * Implementaton of Level2 Core function hasAttributes.
         *
         * @returns {boolean}
         * True if attribute list is not empty.
         * @see https://www.w3.org/TR/DOM-Level-2-Core/#core-ID-NodeHasAttrs
         */
        hasAttributes: function() {
          return !!(this.attributes && this.attributes.length);
        },
        hasAttribute: function(name2) {
          return !!this.getAttributeNode(name2);
        },
        /**
         * Returns element’s first attribute whose qualified name is `name`, and `null`
         * if there is no such attribute.
         *
         * @param {string} name
         * @returns {string | null}
         */
        getAttribute: function(name2) {
          var attr = this.getAttributeNode(name2);
          return attr ? attr.value : null;
        },
        getAttributeNode: function(name2) {
          if (this._isInHTMLDocumentAndNamespace()) {
            name2 = name2.toLowerCase();
          }
          return this.attributes.getNamedItem(name2);
        },
        /**
         * Sets the value of element’s first attribute whose qualified name is qualifiedName to value.
         *
         * @param {string} name
         * @param {string} value
         */
        setAttribute: function(name2, value) {
          if (this._isInHTMLDocumentAndNamespace()) {
            name2 = name2.toLowerCase();
          }
          var attr = this.getAttributeNode(name2);
          if (attr) {
            attr.value = attr.nodeValue = "" + value;
          } else {
            attr = this.ownerDocument._createAttribute(name2);
            attr.value = attr.nodeValue = "" + value;
            this.setAttributeNode(attr);
          }
        },
        removeAttribute: function(name2) {
          var attr = this.getAttributeNode(name2);
          attr && this.removeAttributeNode(attr);
        },
        setAttributeNode: function(newAttr) {
          return this.attributes.setNamedItem(newAttr);
        },
        setAttributeNodeNS: function(newAttr) {
          return this.attributes.setNamedItemNS(newAttr);
        },
        removeAttributeNode: function(oldAttr) {
          return this.attributes.removeNamedItem(oldAttr.nodeName);
        },
        //get real attribute name,and remove it by removeAttributeNode
        removeAttributeNS: function(namespaceURI, localName) {
          var old = this.getAttributeNodeNS(namespaceURI, localName);
          old && this.removeAttributeNode(old);
        },
        hasAttributeNS: function(namespaceURI, localName) {
          return this.getAttributeNodeNS(namespaceURI, localName) != null;
        },
        /**
         * Returns element’s attribute whose namespace is `namespaceURI` and local name is
         * `localName`,
         * or `null` if there is no such attribute.
         *
         * @param {string} namespaceURI
         * @param {string} localName
         * @returns {string | null}
         */
        getAttributeNS: function(namespaceURI, localName) {
          var attr = this.getAttributeNodeNS(namespaceURI, localName);
          return attr ? attr.value : null;
        },
        /**
         * Sets the value of element’s attribute whose namespace is `namespaceURI` and local name is
         * `localName` to value.
         *
         * @param {string} namespaceURI
         * @param {string} qualifiedName
         * @param {string} value
         * @see https://dom.spec.whatwg.org/#dom-element-setattributens
         */
        setAttributeNS: function(namespaceURI, qualifiedName, value) {
          var validated = validateAndExtract(namespaceURI, qualifiedName);
          var localName = validated[2];
          var attr = this.getAttributeNodeNS(namespaceURI, localName);
          if (attr) {
            attr.value = attr.nodeValue = "" + value;
          } else {
            attr = this.ownerDocument.createAttributeNS(namespaceURI, qualifiedName);
            attr.value = attr.nodeValue = "" + value;
            this.setAttributeNode(attr);
          }
        },
        getAttributeNodeNS: function(namespaceURI, localName) {
          return this.attributes.getNamedItemNS(namespaceURI, localName);
        },
        /**
         * Returns a LiveNodeList of all child elements which have **all** of the given class name(s).
         *
         * Returns an empty list if `classNames` is an empty string or only contains HTML white space
         * characters.
         *
         * Warning: This returns a live LiveNodeList.
         * Changes in the DOM will reflect in the array as the changes occur.
         * If an element selected by this array no longer qualifies for the selector,
         * it will automatically be removed. Be aware of this for iteration purposes.
         *
         * @param {string} classNames
         * Is a string representing the class name(s) to match; multiple class names are separated by
         * (ASCII-)whitespace.
         * @see https://developer.mozilla.org/en-US/docs/Web/API/Element/getElementsByClassName
         * @see https://developer.mozilla.org/en-US/docs/Web/API/Document/getElementsByClassName
         * @see https://dom.spec.whatwg.org/#concept-getelementsbyclassname
         */
        getElementsByClassName: function(classNames) {
          var classNamesSet = toOrderedSet(classNames);
          return new LiveNodeList(this, function(base) {
            var ls = [];
            if (classNamesSet.length > 0) {
              _visitNode(base, function(node) {
                if (node !== base && node.nodeType === ELEMENT_NODE) {
                  var nodeClassNames = node.getAttribute("class");
                  if (nodeClassNames) {
                    var matches = classNames === nodeClassNames;
                    if (!matches) {
                      var nodeClassNamesSet = toOrderedSet(nodeClassNames);
                      matches = classNamesSet.every(arrayIncludes(nodeClassNamesSet));
                    }
                    if (matches) {
                      ls.push(node);
                    }
                  }
                }
              });
            }
            return ls;
          });
        },
        /**
         * Returns a LiveNodeList of elements with the given qualifiedName.
         * Searching for all descendants can be done by passing `*` as `qualifiedName`.
         *
         * All descendants of the specified element are searched, but not the element itself.
         * The returned list is live, which means it updates itself with the DOM tree automatically.
         * Therefore, there is no need to call `Element.getElementsByTagName()`
         * with the same element and arguments repeatedly if the DOM changes in between calls.
         *
         * When called on an HTML element in an HTML document,
         * `getElementsByTagName` lower-cases the argument before searching for it.
         * This is undesirable when trying to match camel-cased SVG elements (such as
         * `<linearGradient>`) in an HTML document.
         * Instead, use `Element.getElementsByTagNameNS()`,
         * which preserves the capitalization of the tag name.
         *
         * `Element.getElementsByTagName` is similar to `Document.getElementsByTagName()`,
         * except that it only searches for elements that are descendants of the specified element.
         *
         * @param {string} qualifiedName
         * @returns {LiveNodeList}
         * @see https://developer.mozilla.org/en-US/docs/Web/API/Element/getElementsByTagName
         * @see https://dom.spec.whatwg.org/#concept-getelementsbytagname
         */
        getElementsByTagName: function(qualifiedName) {
          var isHTMLDocument = (this.nodeType === DOCUMENT_NODE ? this : this.ownerDocument).type === "html";
          var lowerQualifiedName = qualifiedName.toLowerCase();
          return new LiveNodeList(this, function(base) {
            var ls = [];
            _visitNode(base, function(node) {
              if (node === base || node.nodeType !== ELEMENT_NODE) {
                return;
              }
              if (qualifiedName === "*") {
                ls.push(node);
              } else {
                var nodeQualifiedName = node.getQualifiedName();
                var matchingQName = isHTMLDocument && node.namespaceURI === NAMESPACE.HTML ? lowerQualifiedName : qualifiedName;
                if (nodeQualifiedName === matchingQName) {
                  ls.push(node);
                }
              }
            });
            return ls;
          });
        },
        getElementsByTagNameNS: function(namespaceURI, localName) {
          return new LiveNodeList(this, function(base) {
            var ls = [];
            _visitNode(base, function(node) {
              if (node !== base && node.nodeType === ELEMENT_NODE && (namespaceURI === "*" || node.namespaceURI === namespaceURI) && (localName === "*" || node.localName == localName)) {
                ls.push(node);
              }
            });
            return ls;
          });
        }
      };
      Document.prototype.getElementsByClassName = Element.prototype.getElementsByClassName;
      Document.prototype.getElementsByTagName = Element.prototype.getElementsByTagName;
      Document.prototype.getElementsByTagNameNS = Element.prototype.getElementsByTagNameNS;
      _extends(Element, Node);
      function Attr(symbol) {
        checkSymbol(symbol);
        this.namespaceURI = null;
        this.prefix = null;
        this.ownerElement = null;
      }
      Attr.prototype.nodeType = ATTRIBUTE_NODE;
      _extends(Attr, Node);
      function CharacterData(symbol) {
        checkSymbol(symbol);
      }
      CharacterData.prototype = {
        data: "",
        substringData: function(offset, count) {
          return this.data.substring(offset, offset + count);
        },
        appendData: function(text) {
          text = this.data + text;
          this.nodeValue = this.data = text;
          this.length = text.length;
        },
        insertData: function(offset, text) {
          this.replaceData(offset, 0, text);
        },
        deleteData: function(offset, count) {
          this.replaceData(offset, count, "");
        },
        replaceData: function(offset, count, text) {
          var start = this.data.substring(0, offset);
          var end = this.data.substring(offset + count);
          text = start + text + end;
          this.nodeValue = this.data = text;
          this.length = text.length;
        }
      };
      _extends(CharacterData, Node);
      function Text(symbol) {
        checkSymbol(symbol);
      }
      Text.prototype = {
        nodeName: "#text",
        nodeType: TEXT_NODE,
        splitText: function(offset) {
          var text = this.data;
          var newText = text.substring(offset);
          text = text.substring(0, offset);
          this.data = this.nodeValue = text;
          this.length = text.length;
          var newNode = this.ownerDocument.createTextNode(newText);
          if (this.parentNode) {
            this.parentNode.insertBefore(newNode, this.nextSibling);
          }
          return newNode;
        }
      };
      _extends(Text, CharacterData);
      function Comment(symbol) {
        checkSymbol(symbol);
      }
      Comment.prototype = {
        nodeName: "#comment",
        nodeType: COMMENT_NODE
      };
      _extends(Comment, CharacterData);
      function CDATASection(symbol) {
        checkSymbol(symbol);
      }
      CDATASection.prototype = {
        nodeName: "#cdata-section",
        nodeType: CDATA_SECTION_NODE
      };
      _extends(CDATASection, Text);
      function DocumentType(symbol) {
        checkSymbol(symbol);
      }
      DocumentType.prototype.nodeType = DOCUMENT_TYPE_NODE;
      _extends(DocumentType, Node);
      function Notation(symbol) {
        checkSymbol(symbol);
      }
      Notation.prototype.nodeType = NOTATION_NODE;
      _extends(Notation, Node);
      function Entity(symbol) {
        checkSymbol(symbol);
      }
      Entity.prototype.nodeType = ENTITY_NODE;
      _extends(Entity, Node);
      function EntityReference(symbol) {
        checkSymbol(symbol);
      }
      EntityReference.prototype.nodeType = ENTITY_REFERENCE_NODE;
      _extends(EntityReference, Node);
      function DocumentFragment(symbol) {
        checkSymbol(symbol);
      }
      DocumentFragment.prototype.nodeName = "#document-fragment";
      DocumentFragment.prototype.nodeType = DOCUMENT_FRAGMENT_NODE;
      _extends(DocumentFragment, Node);
      function ProcessingInstruction(symbol) {
        checkSymbol(symbol);
      }
      ProcessingInstruction.prototype.nodeType = PROCESSING_INSTRUCTION_NODE;
      _extends(ProcessingInstruction, CharacterData);
      function XMLSerializer() {
      }
      XMLSerializer.prototype.serializeToString = function(node, nodeFilter) {
        return nodeSerializeToString.call(node, nodeFilter);
      };
      Node.prototype.toString = nodeSerializeToString;
      function nodeSerializeToString(nodeFilter) {
        var buf = [];
        var refNode = this.nodeType === DOCUMENT_NODE && this.documentElement || this;
        var prefix = refNode.prefix;
        var uri = refNode.namespaceURI;
        if (uri && prefix == null) {
          var prefix = refNode.lookupPrefix(uri);
          if (prefix == null) {
            var visibleNamespaces = [
              { namespace: uri, prefix: null }
              //{namespace:uri,prefix:''}
            ];
          }
        }
        serializeToString(this, buf, nodeFilter, visibleNamespaces);
        return buf.join("");
      }
      function needNamespaceDefine(node, isHTML, visibleNamespaces) {
        var prefix = node.prefix || "";
        var uri = node.namespaceURI;
        if (!uri) {
          return false;
        }
        if (prefix === "xml" && uri === NAMESPACE.XML || uri === NAMESPACE.XMLNS) {
          return false;
        }
        var i = visibleNamespaces.length;
        while (i--) {
          var ns = visibleNamespaces[i];
          if (ns.prefix === prefix) {
            return ns.namespace !== uri;
          }
        }
        return true;
      }
      function addSerializedAttribute(buf, qualifiedName, value) {
        buf.push(" ", qualifiedName, '="', value.replace(/[<>&"\t\n\r]/g, _xmlEncoder), '"');
      }
      function serializeToString(node, buf, nodeFilter, visibleNamespaces) {
        if (!visibleNamespaces) {
          visibleNamespaces = [];
        }
        var doc = node.nodeType === DOCUMENT_NODE ? node : node.ownerDocument;
        var isHTML = doc.type === "html";
        if (nodeFilter) {
          node = nodeFilter(node);
          if (node) {
            if (typeof node == "string") {
              buf.push(node);
              return;
            }
          } else {
            return;
          }
        }
        switch (node.nodeType) {
          case ELEMENT_NODE:
            var attrs = node.attributes;
            var len = attrs.length;
            var child = node.firstChild;
            var nodeName = node.tagName;
            var prefixedNodeName = nodeName;
            if (!isHTML && !node.prefix && node.namespaceURI) {
              var defaultNS;
              for (var ai = 0; ai < attrs.length; ai++) {
                if (attrs.item(ai).name === "xmlns") {
                  defaultNS = attrs.item(ai).value;
                  break;
                }
              }
              if (!defaultNS) {
                for (var nsi = visibleNamespaces.length - 1; nsi >= 0; nsi--) {
                  var namespace = visibleNamespaces[nsi];
                  if (namespace.prefix === "" && namespace.namespace === node.namespaceURI) {
                    defaultNS = namespace.namespace;
                    break;
                  }
                }
              }
              if (defaultNS !== node.namespaceURI) {
                for (var nsi = visibleNamespaces.length - 1; nsi >= 0; nsi--) {
                  var namespace = visibleNamespaces[nsi];
                  if (namespace.namespace === node.namespaceURI) {
                    if (namespace.prefix) {
                      prefixedNodeName = namespace.prefix + ":" + nodeName;
                    }
                    break;
                  }
                }
              }
            }
            buf.push("<", prefixedNodeName);
            for (var i = 0; i < len; i++) {
              var attr = attrs.item(i);
              if (attr.prefix == "xmlns") {
                visibleNamespaces.push({
                  prefix: attr.localName,
                  namespace: attr.value
                });
              } else if (attr.nodeName == "xmlns") {
                visibleNamespaces.push({ prefix: "", namespace: attr.value });
              }
            }
            for (var i = 0; i < len; i++) {
              var attr = attrs.item(i);
              if (needNamespaceDefine(attr, isHTML, visibleNamespaces)) {
                var prefix = attr.prefix || "";
                var uri = attr.namespaceURI;
                addSerializedAttribute(buf, prefix ? "xmlns:" + prefix : "xmlns", uri);
                visibleNamespaces.push({ prefix, namespace: uri });
              }
              serializeToString(attr, buf, nodeFilter, visibleNamespaces);
            }
            if (nodeName === prefixedNodeName && needNamespaceDefine(node, isHTML, visibleNamespaces)) {
              var prefix = node.prefix || "";
              var uri = node.namespaceURI;
              addSerializedAttribute(buf, prefix ? "xmlns:" + prefix : "xmlns", uri);
              visibleNamespaces.push({ prefix, namespace: uri });
            }
            var canCloseTag = !child;
            if (canCloseTag && (isHTML || node.namespaceURI === NAMESPACE.HTML)) {
              canCloseTag = isHTMLVoidElement(nodeName);
            }
            if (canCloseTag) {
              buf.push("/>");
            } else {
              buf.push(">");
              if (isHTML && isHTMLRawTextElement(nodeName)) {
                while (child) {
                  if (child.data) {
                    buf.push(child.data);
                  } else {
                    serializeToString(child, buf, nodeFilter, visibleNamespaces.slice());
                  }
                  child = child.nextSibling;
                }
              } else {
                while (child) {
                  serializeToString(child, buf, nodeFilter, visibleNamespaces.slice());
                  child = child.nextSibling;
                }
              }
              buf.push("</", prefixedNodeName, ">");
            }
            return;
          case DOCUMENT_NODE:
          case DOCUMENT_FRAGMENT_NODE:
            var child = node.firstChild;
            while (child) {
              serializeToString(child, buf, nodeFilter, visibleNamespaces.slice());
              child = child.nextSibling;
            }
            return;
          case ATTRIBUTE_NODE:
            return addSerializedAttribute(buf, node.name, node.value);
          case TEXT_NODE:
            return buf.push(node.data.replace(/[<&>]/g, _xmlEncoder));
          case CDATA_SECTION_NODE:
            return buf.push(g.CDATA_START, node.data, g.CDATA_END);
          case COMMENT_NODE:
            return buf.push(g.COMMENT_START, node.data, g.COMMENT_END);
          case DOCUMENT_TYPE_NODE:
            var pubid = node.publicId;
            var sysid = node.systemId;
            buf.push(g.DOCTYPE_DECL_START, " ", node.name);
            if (pubid) {
              buf.push(" ", g.PUBLIC, " ", pubid);
              if (sysid && sysid !== ".") {
                buf.push(" ", sysid);
              }
            } else if (sysid && sysid !== ".") {
              buf.push(" ", g.SYSTEM, " ", sysid);
            }
            if (node.internalSubset) {
              buf.push(" [", node.internalSubset, "]");
            }
            buf.push(">");
            return;
          case PROCESSING_INSTRUCTION_NODE:
            return buf.push("<?", node.target, " ", node.data, "?>");
          case ENTITY_REFERENCE_NODE:
            return buf.push("&", node.nodeName, ";");
          //case ENTITY_NODE:
          //case NOTATION_NODE:
          default:
            buf.push("??", node.nodeName);
        }
      }
      function importNode(doc, node, deep) {
        var node2;
        switch (node.nodeType) {
          case ELEMENT_NODE:
            node2 = node.cloneNode(false);
            node2.ownerDocument = doc;
          //var attrs = node2.attributes;
          //var len = attrs.length;
          //for(var i=0;i<len;i++){
          //node2.setAttributeNodeNS(importNode(doc,attrs.item(i),deep));
          //}
          case DOCUMENT_FRAGMENT_NODE:
            break;
          case ATTRIBUTE_NODE:
            deep = true;
            break;
        }
        if (!node2) {
          node2 = node.cloneNode(false);
        }
        node2.ownerDocument = doc;
        node2.parentNode = null;
        if (deep) {
          var child = node.firstChild;
          while (child) {
            node2.appendChild(importNode(doc, child, deep));
            child = child.nextSibling;
          }
        }
        return node2;
      }
      function cloneNode(doc, node, deep) {
        var node2 = new node.constructor(PDC);
        for (var n in node) {
          if (hasOwn(node, n)) {
            var v = node[n];
            if (typeof v != "object") {
              if (v != node2[n]) {
                node2[n] = v;
              }
            }
          }
        }
        if (node.childNodes) {
          node2.childNodes = new NodeList();
        }
        node2.ownerDocument = doc;
        switch (node2.nodeType) {
          case ELEMENT_NODE:
            var attrs = node.attributes;
            var attrs2 = node2.attributes = new NamedNodeMap();
            var len = attrs.length;
            attrs2._ownerElement = node2;
            for (var i = 0; i < len; i++) {
              node2.setAttributeNode(cloneNode(doc, attrs.item(i), true));
            }
            break;
          case ATTRIBUTE_NODE:
            deep = true;
        }
        if (deep) {
          var child = node.firstChild;
          while (child) {
            node2.appendChild(cloneNode(doc, child, deep));
            child = child.nextSibling;
          }
        }
        return node2;
      }
      function __set__(object, key, value) {
        object[key] = value;
      }
      try {
        if (Object.defineProperty) {
          let getTextContent = function(node) {
            switch (node.nodeType) {
              case ELEMENT_NODE:
              case DOCUMENT_FRAGMENT_NODE:
                var buf = [];
                node = node.firstChild;
                while (node) {
                  if (node.nodeType !== 7 && node.nodeType !== 8) {
                    buf.push(getTextContent(node));
                  }
                  node = node.nextSibling;
                }
                return buf.join("");
              default:
                return node.nodeValue;
            }
          };
          Object.defineProperty(LiveNodeList.prototype, "length", {
            get: function() {
              _updateLiveList(this);
              return this.$$length;
            }
          });
          Object.defineProperty(Node.prototype, "textContent", {
            get: function() {
              return getTextContent(this);
            },
            set: function(data) {
              switch (this.nodeType) {
                case ELEMENT_NODE:
                case DOCUMENT_FRAGMENT_NODE:
                  while (this.firstChild) {
                    this.removeChild(this.firstChild);
                  }
                  if (data || String(data)) {
                    this.appendChild(this.ownerDocument.createTextNode(data));
                  }
                  break;
                default:
                  this.data = data;
                  this.value = data;
                  this.nodeValue = data;
              }
            }
          });
          __set__ = function(object, key, value) {
            object["$$" + key] = value;
          };
        }
      } catch (e) {
      }
      exports._updateLiveList = _updateLiveList;
      exports.Attr = Attr;
      exports.CDATASection = CDATASection;
      exports.CharacterData = CharacterData;
      exports.Comment = Comment;
      exports.Document = Document;
      exports.DocumentFragment = DocumentFragment;
      exports.DocumentType = DocumentType;
      exports.DOMImplementation = DOMImplementation;
      exports.Element = Element;
      exports.Entity = Entity;
      exports.EntityReference = EntityReference;
      exports.LiveNodeList = LiveNodeList;
      exports.NamedNodeMap = NamedNodeMap;
      exports.Node = Node;
      exports.NodeList = NodeList;
      exports.Notation = Notation;
      exports.Text = Text;
      exports.ProcessingInstruction = ProcessingInstruction;
      exports.XMLSerializer = XMLSerializer;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/entities.js
  var require_entities = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/entities.js"(exports) {
      "use strict";
      var freeze = require_conventions().freeze;
      exports.XML_ENTITIES = freeze({
        amp: "&",
        apos: "'",
        gt: ">",
        lt: "<",
        quot: '"'
      });
      exports.HTML_ENTITIES = freeze({
        Aacute: "\xC1",
        aacute: "\xE1",
        Abreve: "\u0102",
        abreve: "\u0103",
        ac: "\u223E",
        acd: "\u223F",
        acE: "\u223E\u0333",
        Acirc: "\xC2",
        acirc: "\xE2",
        acute: "\xB4",
        Acy: "\u0410",
        acy: "\u0430",
        AElig: "\xC6",
        aelig: "\xE6",
        af: "\u2061",
        Afr: "\u{1D504}",
        afr: "\u{1D51E}",
        Agrave: "\xC0",
        agrave: "\xE0",
        alefsym: "\u2135",
        aleph: "\u2135",
        Alpha: "\u0391",
        alpha: "\u03B1",
        Amacr: "\u0100",
        amacr: "\u0101",
        amalg: "\u2A3F",
        AMP: "&",
        amp: "&",
        And: "\u2A53",
        and: "\u2227",
        andand: "\u2A55",
        andd: "\u2A5C",
        andslope: "\u2A58",
        andv: "\u2A5A",
        ang: "\u2220",
        ange: "\u29A4",
        angle: "\u2220",
        angmsd: "\u2221",
        angmsdaa: "\u29A8",
        angmsdab: "\u29A9",
        angmsdac: "\u29AA",
        angmsdad: "\u29AB",
        angmsdae: "\u29AC",
        angmsdaf: "\u29AD",
        angmsdag: "\u29AE",
        angmsdah: "\u29AF",
        angrt: "\u221F",
        angrtvb: "\u22BE",
        angrtvbd: "\u299D",
        angsph: "\u2222",
        angst: "\xC5",
        angzarr: "\u237C",
        Aogon: "\u0104",
        aogon: "\u0105",
        Aopf: "\u{1D538}",
        aopf: "\u{1D552}",
        ap: "\u2248",
        apacir: "\u2A6F",
        apE: "\u2A70",
        ape: "\u224A",
        apid: "\u224B",
        apos: "'",
        ApplyFunction: "\u2061",
        approx: "\u2248",
        approxeq: "\u224A",
        Aring: "\xC5",
        aring: "\xE5",
        Ascr: "\u{1D49C}",
        ascr: "\u{1D4B6}",
        Assign: "\u2254",
        ast: "*",
        asymp: "\u2248",
        asympeq: "\u224D",
        Atilde: "\xC3",
        atilde: "\xE3",
        Auml: "\xC4",
        auml: "\xE4",
        awconint: "\u2233",
        awint: "\u2A11",
        backcong: "\u224C",
        backepsilon: "\u03F6",
        backprime: "\u2035",
        backsim: "\u223D",
        backsimeq: "\u22CD",
        Backslash: "\u2216",
        Barv: "\u2AE7",
        barvee: "\u22BD",
        Barwed: "\u2306",
        barwed: "\u2305",
        barwedge: "\u2305",
        bbrk: "\u23B5",
        bbrktbrk: "\u23B6",
        bcong: "\u224C",
        Bcy: "\u0411",
        bcy: "\u0431",
        bdquo: "\u201E",
        becaus: "\u2235",
        Because: "\u2235",
        because: "\u2235",
        bemptyv: "\u29B0",
        bepsi: "\u03F6",
        bernou: "\u212C",
        Bernoullis: "\u212C",
        Beta: "\u0392",
        beta: "\u03B2",
        beth: "\u2136",
        between: "\u226C",
        Bfr: "\u{1D505}",
        bfr: "\u{1D51F}",
        bigcap: "\u22C2",
        bigcirc: "\u25EF",
        bigcup: "\u22C3",
        bigodot: "\u2A00",
        bigoplus: "\u2A01",
        bigotimes: "\u2A02",
        bigsqcup: "\u2A06",
        bigstar: "\u2605",
        bigtriangledown: "\u25BD",
        bigtriangleup: "\u25B3",
        biguplus: "\u2A04",
        bigvee: "\u22C1",
        bigwedge: "\u22C0",
        bkarow: "\u290D",
        blacklozenge: "\u29EB",
        blacksquare: "\u25AA",
        blacktriangle: "\u25B4",
        blacktriangledown: "\u25BE",
        blacktriangleleft: "\u25C2",
        blacktriangleright: "\u25B8",
        blank: "\u2423",
        blk12: "\u2592",
        blk14: "\u2591",
        blk34: "\u2593",
        block: "\u2588",
        bne: "=\u20E5",
        bnequiv: "\u2261\u20E5",
        bNot: "\u2AED",
        bnot: "\u2310",
        Bopf: "\u{1D539}",
        bopf: "\u{1D553}",
        bot: "\u22A5",
        bottom: "\u22A5",
        bowtie: "\u22C8",
        boxbox: "\u29C9",
        boxDL: "\u2557",
        boxDl: "\u2556",
        boxdL: "\u2555",
        boxdl: "\u2510",
        boxDR: "\u2554",
        boxDr: "\u2553",
        boxdR: "\u2552",
        boxdr: "\u250C",
        boxH: "\u2550",
        boxh: "\u2500",
        boxHD: "\u2566",
        boxHd: "\u2564",
        boxhD: "\u2565",
        boxhd: "\u252C",
        boxHU: "\u2569",
        boxHu: "\u2567",
        boxhU: "\u2568",
        boxhu: "\u2534",
        boxminus: "\u229F",
        boxplus: "\u229E",
        boxtimes: "\u22A0",
        boxUL: "\u255D",
        boxUl: "\u255C",
        boxuL: "\u255B",
        boxul: "\u2518",
        boxUR: "\u255A",
        boxUr: "\u2559",
        boxuR: "\u2558",
        boxur: "\u2514",
        boxV: "\u2551",
        boxv: "\u2502",
        boxVH: "\u256C",
        boxVh: "\u256B",
        boxvH: "\u256A",
        boxvh: "\u253C",
        boxVL: "\u2563",
        boxVl: "\u2562",
        boxvL: "\u2561",
        boxvl: "\u2524",
        boxVR: "\u2560",
        boxVr: "\u255F",
        boxvR: "\u255E",
        boxvr: "\u251C",
        bprime: "\u2035",
        Breve: "\u02D8",
        breve: "\u02D8",
        brvbar: "\xA6",
        Bscr: "\u212C",
        bscr: "\u{1D4B7}",
        bsemi: "\u204F",
        bsim: "\u223D",
        bsime: "\u22CD",
        bsol: "\\",
        bsolb: "\u29C5",
        bsolhsub: "\u27C8",
        bull: "\u2022",
        bullet: "\u2022",
        bump: "\u224E",
        bumpE: "\u2AAE",
        bumpe: "\u224F",
        Bumpeq: "\u224E",
        bumpeq: "\u224F",
        Cacute: "\u0106",
        cacute: "\u0107",
        Cap: "\u22D2",
        cap: "\u2229",
        capand: "\u2A44",
        capbrcup: "\u2A49",
        capcap: "\u2A4B",
        capcup: "\u2A47",
        capdot: "\u2A40",
        CapitalDifferentialD: "\u2145",
        caps: "\u2229\uFE00",
        caret: "\u2041",
        caron: "\u02C7",
        Cayleys: "\u212D",
        ccaps: "\u2A4D",
        Ccaron: "\u010C",
        ccaron: "\u010D",
        Ccedil: "\xC7",
        ccedil: "\xE7",
        Ccirc: "\u0108",
        ccirc: "\u0109",
        Cconint: "\u2230",
        ccups: "\u2A4C",
        ccupssm: "\u2A50",
        Cdot: "\u010A",
        cdot: "\u010B",
        cedil: "\xB8",
        Cedilla: "\xB8",
        cemptyv: "\u29B2",
        cent: "\xA2",
        CenterDot: "\xB7",
        centerdot: "\xB7",
        Cfr: "\u212D",
        cfr: "\u{1D520}",
        CHcy: "\u0427",
        chcy: "\u0447",
        check: "\u2713",
        checkmark: "\u2713",
        Chi: "\u03A7",
        chi: "\u03C7",
        cir: "\u25CB",
        circ: "\u02C6",
        circeq: "\u2257",
        circlearrowleft: "\u21BA",
        circlearrowright: "\u21BB",
        circledast: "\u229B",
        circledcirc: "\u229A",
        circleddash: "\u229D",
        CircleDot: "\u2299",
        circledR: "\xAE",
        circledS: "\u24C8",
        CircleMinus: "\u2296",
        CirclePlus: "\u2295",
        CircleTimes: "\u2297",
        cirE: "\u29C3",
        cire: "\u2257",
        cirfnint: "\u2A10",
        cirmid: "\u2AEF",
        cirscir: "\u29C2",
        ClockwiseContourIntegral: "\u2232",
        CloseCurlyDoubleQuote: "\u201D",
        CloseCurlyQuote: "\u2019",
        clubs: "\u2663",
        clubsuit: "\u2663",
        Colon: "\u2237",
        colon: ":",
        Colone: "\u2A74",
        colone: "\u2254",
        coloneq: "\u2254",
        comma: ",",
        commat: "@",
        comp: "\u2201",
        compfn: "\u2218",
        complement: "\u2201",
        complexes: "\u2102",
        cong: "\u2245",
        congdot: "\u2A6D",
        Congruent: "\u2261",
        Conint: "\u222F",
        conint: "\u222E",
        ContourIntegral: "\u222E",
        Copf: "\u2102",
        copf: "\u{1D554}",
        coprod: "\u2210",
        Coproduct: "\u2210",
        COPY: "\xA9",
        copy: "\xA9",
        copysr: "\u2117",
        CounterClockwiseContourIntegral: "\u2233",
        crarr: "\u21B5",
        Cross: "\u2A2F",
        cross: "\u2717",
        Cscr: "\u{1D49E}",
        cscr: "\u{1D4B8}",
        csub: "\u2ACF",
        csube: "\u2AD1",
        csup: "\u2AD0",
        csupe: "\u2AD2",
        ctdot: "\u22EF",
        cudarrl: "\u2938",
        cudarrr: "\u2935",
        cuepr: "\u22DE",
        cuesc: "\u22DF",
        cularr: "\u21B6",
        cularrp: "\u293D",
        Cup: "\u22D3",
        cup: "\u222A",
        cupbrcap: "\u2A48",
        CupCap: "\u224D",
        cupcap: "\u2A46",
        cupcup: "\u2A4A",
        cupdot: "\u228D",
        cupor: "\u2A45",
        cups: "\u222A\uFE00",
        curarr: "\u21B7",
        curarrm: "\u293C",
        curlyeqprec: "\u22DE",
        curlyeqsucc: "\u22DF",
        curlyvee: "\u22CE",
        curlywedge: "\u22CF",
        curren: "\xA4",
        curvearrowleft: "\u21B6",
        curvearrowright: "\u21B7",
        cuvee: "\u22CE",
        cuwed: "\u22CF",
        cwconint: "\u2232",
        cwint: "\u2231",
        cylcty: "\u232D",
        Dagger: "\u2021",
        dagger: "\u2020",
        daleth: "\u2138",
        Darr: "\u21A1",
        dArr: "\u21D3",
        darr: "\u2193",
        dash: "\u2010",
        Dashv: "\u2AE4",
        dashv: "\u22A3",
        dbkarow: "\u290F",
        dblac: "\u02DD",
        Dcaron: "\u010E",
        dcaron: "\u010F",
        Dcy: "\u0414",
        dcy: "\u0434",
        DD: "\u2145",
        dd: "\u2146",
        ddagger: "\u2021",
        ddarr: "\u21CA",
        DDotrahd: "\u2911",
        ddotseq: "\u2A77",
        deg: "\xB0",
        Del: "\u2207",
        Delta: "\u0394",
        delta: "\u03B4",
        demptyv: "\u29B1",
        dfisht: "\u297F",
        Dfr: "\u{1D507}",
        dfr: "\u{1D521}",
        dHar: "\u2965",
        dharl: "\u21C3",
        dharr: "\u21C2",
        DiacriticalAcute: "\xB4",
        DiacriticalDot: "\u02D9",
        DiacriticalDoubleAcute: "\u02DD",
        DiacriticalGrave: "`",
        DiacriticalTilde: "\u02DC",
        diam: "\u22C4",
        Diamond: "\u22C4",
        diamond: "\u22C4",
        diamondsuit: "\u2666",
        diams: "\u2666",
        die: "\xA8",
        DifferentialD: "\u2146",
        digamma: "\u03DD",
        disin: "\u22F2",
        div: "\xF7",
        divide: "\xF7",
        divideontimes: "\u22C7",
        divonx: "\u22C7",
        DJcy: "\u0402",
        djcy: "\u0452",
        dlcorn: "\u231E",
        dlcrop: "\u230D",
        dollar: "$",
        Dopf: "\u{1D53B}",
        dopf: "\u{1D555}",
        Dot: "\xA8",
        dot: "\u02D9",
        DotDot: "\u20DC",
        doteq: "\u2250",
        doteqdot: "\u2251",
        DotEqual: "\u2250",
        dotminus: "\u2238",
        dotplus: "\u2214",
        dotsquare: "\u22A1",
        doublebarwedge: "\u2306",
        DoubleContourIntegral: "\u222F",
        DoubleDot: "\xA8",
        DoubleDownArrow: "\u21D3",
        DoubleLeftArrow: "\u21D0",
        DoubleLeftRightArrow: "\u21D4",
        DoubleLeftTee: "\u2AE4",
        DoubleLongLeftArrow: "\u27F8",
        DoubleLongLeftRightArrow: "\u27FA",
        DoubleLongRightArrow: "\u27F9",
        DoubleRightArrow: "\u21D2",
        DoubleRightTee: "\u22A8",
        DoubleUpArrow: "\u21D1",
        DoubleUpDownArrow: "\u21D5",
        DoubleVerticalBar: "\u2225",
        DownArrow: "\u2193",
        Downarrow: "\u21D3",
        downarrow: "\u2193",
        DownArrowBar: "\u2913",
        DownArrowUpArrow: "\u21F5",
        DownBreve: "\u0311",
        downdownarrows: "\u21CA",
        downharpoonleft: "\u21C3",
        downharpoonright: "\u21C2",
        DownLeftRightVector: "\u2950",
        DownLeftTeeVector: "\u295E",
        DownLeftVector: "\u21BD",
        DownLeftVectorBar: "\u2956",
        DownRightTeeVector: "\u295F",
        DownRightVector: "\u21C1",
        DownRightVectorBar: "\u2957",
        DownTee: "\u22A4",
        DownTeeArrow: "\u21A7",
        drbkarow: "\u2910",
        drcorn: "\u231F",
        drcrop: "\u230C",
        Dscr: "\u{1D49F}",
        dscr: "\u{1D4B9}",
        DScy: "\u0405",
        dscy: "\u0455",
        dsol: "\u29F6",
        Dstrok: "\u0110",
        dstrok: "\u0111",
        dtdot: "\u22F1",
        dtri: "\u25BF",
        dtrif: "\u25BE",
        duarr: "\u21F5",
        duhar: "\u296F",
        dwangle: "\u29A6",
        DZcy: "\u040F",
        dzcy: "\u045F",
        dzigrarr: "\u27FF",
        Eacute: "\xC9",
        eacute: "\xE9",
        easter: "\u2A6E",
        Ecaron: "\u011A",
        ecaron: "\u011B",
        ecir: "\u2256",
        Ecirc: "\xCA",
        ecirc: "\xEA",
        ecolon: "\u2255",
        Ecy: "\u042D",
        ecy: "\u044D",
        eDDot: "\u2A77",
        Edot: "\u0116",
        eDot: "\u2251",
        edot: "\u0117",
        ee: "\u2147",
        efDot: "\u2252",
        Efr: "\u{1D508}",
        efr: "\u{1D522}",
        eg: "\u2A9A",
        Egrave: "\xC8",
        egrave: "\xE8",
        egs: "\u2A96",
        egsdot: "\u2A98",
        el: "\u2A99",
        Element: "\u2208",
        elinters: "\u23E7",
        ell: "\u2113",
        els: "\u2A95",
        elsdot: "\u2A97",
        Emacr: "\u0112",
        emacr: "\u0113",
        empty: "\u2205",
        emptyset: "\u2205",
        EmptySmallSquare: "\u25FB",
        emptyv: "\u2205",
        EmptyVerySmallSquare: "\u25AB",
        emsp: "\u2003",
        emsp13: "\u2004",
        emsp14: "\u2005",
        ENG: "\u014A",
        eng: "\u014B",
        ensp: "\u2002",
        Eogon: "\u0118",
        eogon: "\u0119",
        Eopf: "\u{1D53C}",
        eopf: "\u{1D556}",
        epar: "\u22D5",
        eparsl: "\u29E3",
        eplus: "\u2A71",
        epsi: "\u03B5",
        Epsilon: "\u0395",
        epsilon: "\u03B5",
        epsiv: "\u03F5",
        eqcirc: "\u2256",
        eqcolon: "\u2255",
        eqsim: "\u2242",
        eqslantgtr: "\u2A96",
        eqslantless: "\u2A95",
        Equal: "\u2A75",
        equals: "=",
        EqualTilde: "\u2242",
        equest: "\u225F",
        Equilibrium: "\u21CC",
        equiv: "\u2261",
        equivDD: "\u2A78",
        eqvparsl: "\u29E5",
        erarr: "\u2971",
        erDot: "\u2253",
        Escr: "\u2130",
        escr: "\u212F",
        esdot: "\u2250",
        Esim: "\u2A73",
        esim: "\u2242",
        Eta: "\u0397",
        eta: "\u03B7",
        ETH: "\xD0",
        eth: "\xF0",
        Euml: "\xCB",
        euml: "\xEB",
        euro: "\u20AC",
        excl: "!",
        exist: "\u2203",
        Exists: "\u2203",
        expectation: "\u2130",
        ExponentialE: "\u2147",
        exponentiale: "\u2147",
        fallingdotseq: "\u2252",
        Fcy: "\u0424",
        fcy: "\u0444",
        female: "\u2640",
        ffilig: "\uFB03",
        fflig: "\uFB00",
        ffllig: "\uFB04",
        Ffr: "\u{1D509}",
        ffr: "\u{1D523}",
        filig: "\uFB01",
        FilledSmallSquare: "\u25FC",
        FilledVerySmallSquare: "\u25AA",
        fjlig: "fj",
        flat: "\u266D",
        fllig: "\uFB02",
        fltns: "\u25B1",
        fnof: "\u0192",
        Fopf: "\u{1D53D}",
        fopf: "\u{1D557}",
        ForAll: "\u2200",
        forall: "\u2200",
        fork: "\u22D4",
        forkv: "\u2AD9",
        Fouriertrf: "\u2131",
        fpartint: "\u2A0D",
        frac12: "\xBD",
        frac13: "\u2153",
        frac14: "\xBC",
        frac15: "\u2155",
        frac16: "\u2159",
        frac18: "\u215B",
        frac23: "\u2154",
        frac25: "\u2156",
        frac34: "\xBE",
        frac35: "\u2157",
        frac38: "\u215C",
        frac45: "\u2158",
        frac56: "\u215A",
        frac58: "\u215D",
        frac78: "\u215E",
        frasl: "\u2044",
        frown: "\u2322",
        Fscr: "\u2131",
        fscr: "\u{1D4BB}",
        gacute: "\u01F5",
        Gamma: "\u0393",
        gamma: "\u03B3",
        Gammad: "\u03DC",
        gammad: "\u03DD",
        gap: "\u2A86",
        Gbreve: "\u011E",
        gbreve: "\u011F",
        Gcedil: "\u0122",
        Gcirc: "\u011C",
        gcirc: "\u011D",
        Gcy: "\u0413",
        gcy: "\u0433",
        Gdot: "\u0120",
        gdot: "\u0121",
        gE: "\u2267",
        ge: "\u2265",
        gEl: "\u2A8C",
        gel: "\u22DB",
        geq: "\u2265",
        geqq: "\u2267",
        geqslant: "\u2A7E",
        ges: "\u2A7E",
        gescc: "\u2AA9",
        gesdot: "\u2A80",
        gesdoto: "\u2A82",
        gesdotol: "\u2A84",
        gesl: "\u22DB\uFE00",
        gesles: "\u2A94",
        Gfr: "\u{1D50A}",
        gfr: "\u{1D524}",
        Gg: "\u22D9",
        gg: "\u226B",
        ggg: "\u22D9",
        gimel: "\u2137",
        GJcy: "\u0403",
        gjcy: "\u0453",
        gl: "\u2277",
        gla: "\u2AA5",
        glE: "\u2A92",
        glj: "\u2AA4",
        gnap: "\u2A8A",
        gnapprox: "\u2A8A",
        gnE: "\u2269",
        gne: "\u2A88",
        gneq: "\u2A88",
        gneqq: "\u2269",
        gnsim: "\u22E7",
        Gopf: "\u{1D53E}",
        gopf: "\u{1D558}",
        grave: "`",
        GreaterEqual: "\u2265",
        GreaterEqualLess: "\u22DB",
        GreaterFullEqual: "\u2267",
        GreaterGreater: "\u2AA2",
        GreaterLess: "\u2277",
        GreaterSlantEqual: "\u2A7E",
        GreaterTilde: "\u2273",
        Gscr: "\u{1D4A2}",
        gscr: "\u210A",
        gsim: "\u2273",
        gsime: "\u2A8E",
        gsiml: "\u2A90",
        Gt: "\u226B",
        GT: ">",
        gt: ">",
        gtcc: "\u2AA7",
        gtcir: "\u2A7A",
        gtdot: "\u22D7",
        gtlPar: "\u2995",
        gtquest: "\u2A7C",
        gtrapprox: "\u2A86",
        gtrarr: "\u2978",
        gtrdot: "\u22D7",
        gtreqless: "\u22DB",
        gtreqqless: "\u2A8C",
        gtrless: "\u2277",
        gtrsim: "\u2273",
        gvertneqq: "\u2269\uFE00",
        gvnE: "\u2269\uFE00",
        Hacek: "\u02C7",
        hairsp: "\u200A",
        half: "\xBD",
        hamilt: "\u210B",
        HARDcy: "\u042A",
        hardcy: "\u044A",
        hArr: "\u21D4",
        harr: "\u2194",
        harrcir: "\u2948",
        harrw: "\u21AD",
        Hat: "^",
        hbar: "\u210F",
        Hcirc: "\u0124",
        hcirc: "\u0125",
        hearts: "\u2665",
        heartsuit: "\u2665",
        hellip: "\u2026",
        hercon: "\u22B9",
        Hfr: "\u210C",
        hfr: "\u{1D525}",
        HilbertSpace: "\u210B",
        hksearow: "\u2925",
        hkswarow: "\u2926",
        hoarr: "\u21FF",
        homtht: "\u223B",
        hookleftarrow: "\u21A9",
        hookrightarrow: "\u21AA",
        Hopf: "\u210D",
        hopf: "\u{1D559}",
        horbar: "\u2015",
        HorizontalLine: "\u2500",
        Hscr: "\u210B",
        hscr: "\u{1D4BD}",
        hslash: "\u210F",
        Hstrok: "\u0126",
        hstrok: "\u0127",
        HumpDownHump: "\u224E",
        HumpEqual: "\u224F",
        hybull: "\u2043",
        hyphen: "\u2010",
        Iacute: "\xCD",
        iacute: "\xED",
        ic: "\u2063",
        Icirc: "\xCE",
        icirc: "\xEE",
        Icy: "\u0418",
        icy: "\u0438",
        Idot: "\u0130",
        IEcy: "\u0415",
        iecy: "\u0435",
        iexcl: "\xA1",
        iff: "\u21D4",
        Ifr: "\u2111",
        ifr: "\u{1D526}",
        Igrave: "\xCC",
        igrave: "\xEC",
        ii: "\u2148",
        iiiint: "\u2A0C",
        iiint: "\u222D",
        iinfin: "\u29DC",
        iiota: "\u2129",
        IJlig: "\u0132",
        ijlig: "\u0133",
        Im: "\u2111",
        Imacr: "\u012A",
        imacr: "\u012B",
        image: "\u2111",
        ImaginaryI: "\u2148",
        imagline: "\u2110",
        imagpart: "\u2111",
        imath: "\u0131",
        imof: "\u22B7",
        imped: "\u01B5",
        Implies: "\u21D2",
        in: "\u2208",
        incare: "\u2105",
        infin: "\u221E",
        infintie: "\u29DD",
        inodot: "\u0131",
        Int: "\u222C",
        int: "\u222B",
        intcal: "\u22BA",
        integers: "\u2124",
        Integral: "\u222B",
        intercal: "\u22BA",
        Intersection: "\u22C2",
        intlarhk: "\u2A17",
        intprod: "\u2A3C",
        InvisibleComma: "\u2063",
        InvisibleTimes: "\u2062",
        IOcy: "\u0401",
        iocy: "\u0451",
        Iogon: "\u012E",
        iogon: "\u012F",
        Iopf: "\u{1D540}",
        iopf: "\u{1D55A}",
        Iota: "\u0399",
        iota: "\u03B9",
        iprod: "\u2A3C",
        iquest: "\xBF",
        Iscr: "\u2110",
        iscr: "\u{1D4BE}",
        isin: "\u2208",
        isindot: "\u22F5",
        isinE: "\u22F9",
        isins: "\u22F4",
        isinsv: "\u22F3",
        isinv: "\u2208",
        it: "\u2062",
        Itilde: "\u0128",
        itilde: "\u0129",
        Iukcy: "\u0406",
        iukcy: "\u0456",
        Iuml: "\xCF",
        iuml: "\xEF",
        Jcirc: "\u0134",
        jcirc: "\u0135",
        Jcy: "\u0419",
        jcy: "\u0439",
        Jfr: "\u{1D50D}",
        jfr: "\u{1D527}",
        jmath: "\u0237",
        Jopf: "\u{1D541}",
        jopf: "\u{1D55B}",
        Jscr: "\u{1D4A5}",
        jscr: "\u{1D4BF}",
        Jsercy: "\u0408",
        jsercy: "\u0458",
        Jukcy: "\u0404",
        jukcy: "\u0454",
        Kappa: "\u039A",
        kappa: "\u03BA",
        kappav: "\u03F0",
        Kcedil: "\u0136",
        kcedil: "\u0137",
        Kcy: "\u041A",
        kcy: "\u043A",
        Kfr: "\u{1D50E}",
        kfr: "\u{1D528}",
        kgreen: "\u0138",
        KHcy: "\u0425",
        khcy: "\u0445",
        KJcy: "\u040C",
        kjcy: "\u045C",
        Kopf: "\u{1D542}",
        kopf: "\u{1D55C}",
        Kscr: "\u{1D4A6}",
        kscr: "\u{1D4C0}",
        lAarr: "\u21DA",
        Lacute: "\u0139",
        lacute: "\u013A",
        laemptyv: "\u29B4",
        lagran: "\u2112",
        Lambda: "\u039B",
        lambda: "\u03BB",
        Lang: "\u27EA",
        lang: "\u27E8",
        langd: "\u2991",
        langle: "\u27E8",
        lap: "\u2A85",
        Laplacetrf: "\u2112",
        laquo: "\xAB",
        Larr: "\u219E",
        lArr: "\u21D0",
        larr: "\u2190",
        larrb: "\u21E4",
        larrbfs: "\u291F",
        larrfs: "\u291D",
        larrhk: "\u21A9",
        larrlp: "\u21AB",
        larrpl: "\u2939",
        larrsim: "\u2973",
        larrtl: "\u21A2",
        lat: "\u2AAB",
        lAtail: "\u291B",
        latail: "\u2919",
        late: "\u2AAD",
        lates: "\u2AAD\uFE00",
        lBarr: "\u290E",
        lbarr: "\u290C",
        lbbrk: "\u2772",
        lbrace: "{",
        lbrack: "[",
        lbrke: "\u298B",
        lbrksld: "\u298F",
        lbrkslu: "\u298D",
        Lcaron: "\u013D",
        lcaron: "\u013E",
        Lcedil: "\u013B",
        lcedil: "\u013C",
        lceil: "\u2308",
        lcub: "{",
        Lcy: "\u041B",
        lcy: "\u043B",
        ldca: "\u2936",
        ldquo: "\u201C",
        ldquor: "\u201E",
        ldrdhar: "\u2967",
        ldrushar: "\u294B",
        ldsh: "\u21B2",
        lE: "\u2266",
        le: "\u2264",
        LeftAngleBracket: "\u27E8",
        LeftArrow: "\u2190",
        Leftarrow: "\u21D0",
        leftarrow: "\u2190",
        LeftArrowBar: "\u21E4",
        LeftArrowRightArrow: "\u21C6",
        leftarrowtail: "\u21A2",
        LeftCeiling: "\u2308",
        LeftDoubleBracket: "\u27E6",
        LeftDownTeeVector: "\u2961",
        LeftDownVector: "\u21C3",
        LeftDownVectorBar: "\u2959",
        LeftFloor: "\u230A",
        leftharpoondown: "\u21BD",
        leftharpoonup: "\u21BC",
        leftleftarrows: "\u21C7",
        LeftRightArrow: "\u2194",
        Leftrightarrow: "\u21D4",
        leftrightarrow: "\u2194",
        leftrightarrows: "\u21C6",
        leftrightharpoons: "\u21CB",
        leftrightsquigarrow: "\u21AD",
        LeftRightVector: "\u294E",
        LeftTee: "\u22A3",
        LeftTeeArrow: "\u21A4",
        LeftTeeVector: "\u295A",
        leftthreetimes: "\u22CB",
        LeftTriangle: "\u22B2",
        LeftTriangleBar: "\u29CF",
        LeftTriangleEqual: "\u22B4",
        LeftUpDownVector: "\u2951",
        LeftUpTeeVector: "\u2960",
        LeftUpVector: "\u21BF",
        LeftUpVectorBar: "\u2958",
        LeftVector: "\u21BC",
        LeftVectorBar: "\u2952",
        lEg: "\u2A8B",
        leg: "\u22DA",
        leq: "\u2264",
        leqq: "\u2266",
        leqslant: "\u2A7D",
        les: "\u2A7D",
        lescc: "\u2AA8",
        lesdot: "\u2A7F",
        lesdoto: "\u2A81",
        lesdotor: "\u2A83",
        lesg: "\u22DA\uFE00",
        lesges: "\u2A93",
        lessapprox: "\u2A85",
        lessdot: "\u22D6",
        lesseqgtr: "\u22DA",
        lesseqqgtr: "\u2A8B",
        LessEqualGreater: "\u22DA",
        LessFullEqual: "\u2266",
        LessGreater: "\u2276",
        lessgtr: "\u2276",
        LessLess: "\u2AA1",
        lesssim: "\u2272",
        LessSlantEqual: "\u2A7D",
        LessTilde: "\u2272",
        lfisht: "\u297C",
        lfloor: "\u230A",
        Lfr: "\u{1D50F}",
        lfr: "\u{1D529}",
        lg: "\u2276",
        lgE: "\u2A91",
        lHar: "\u2962",
        lhard: "\u21BD",
        lharu: "\u21BC",
        lharul: "\u296A",
        lhblk: "\u2584",
        LJcy: "\u0409",
        ljcy: "\u0459",
        Ll: "\u22D8",
        ll: "\u226A",
        llarr: "\u21C7",
        llcorner: "\u231E",
        Lleftarrow: "\u21DA",
        llhard: "\u296B",
        lltri: "\u25FA",
        Lmidot: "\u013F",
        lmidot: "\u0140",
        lmoust: "\u23B0",
        lmoustache: "\u23B0",
        lnap: "\u2A89",
        lnapprox: "\u2A89",
        lnE: "\u2268",
        lne: "\u2A87",
        lneq: "\u2A87",
        lneqq: "\u2268",
        lnsim: "\u22E6",
        loang: "\u27EC",
        loarr: "\u21FD",
        lobrk: "\u27E6",
        LongLeftArrow: "\u27F5",
        Longleftarrow: "\u27F8",
        longleftarrow: "\u27F5",
        LongLeftRightArrow: "\u27F7",
        Longleftrightarrow: "\u27FA",
        longleftrightarrow: "\u27F7",
        longmapsto: "\u27FC",
        LongRightArrow: "\u27F6",
        Longrightarrow: "\u27F9",
        longrightarrow: "\u27F6",
        looparrowleft: "\u21AB",
        looparrowright: "\u21AC",
        lopar: "\u2985",
        Lopf: "\u{1D543}",
        lopf: "\u{1D55D}",
        loplus: "\u2A2D",
        lotimes: "\u2A34",
        lowast: "\u2217",
        lowbar: "_",
        LowerLeftArrow: "\u2199",
        LowerRightArrow: "\u2198",
        loz: "\u25CA",
        lozenge: "\u25CA",
        lozf: "\u29EB",
        lpar: "(",
        lparlt: "\u2993",
        lrarr: "\u21C6",
        lrcorner: "\u231F",
        lrhar: "\u21CB",
        lrhard: "\u296D",
        lrm: "\u200E",
        lrtri: "\u22BF",
        lsaquo: "\u2039",
        Lscr: "\u2112",
        lscr: "\u{1D4C1}",
        Lsh: "\u21B0",
        lsh: "\u21B0",
        lsim: "\u2272",
        lsime: "\u2A8D",
        lsimg: "\u2A8F",
        lsqb: "[",
        lsquo: "\u2018",
        lsquor: "\u201A",
        Lstrok: "\u0141",
        lstrok: "\u0142",
        Lt: "\u226A",
        LT: "<",
        lt: "<",
        ltcc: "\u2AA6",
        ltcir: "\u2A79",
        ltdot: "\u22D6",
        lthree: "\u22CB",
        ltimes: "\u22C9",
        ltlarr: "\u2976",
        ltquest: "\u2A7B",
        ltri: "\u25C3",
        ltrie: "\u22B4",
        ltrif: "\u25C2",
        ltrPar: "\u2996",
        lurdshar: "\u294A",
        luruhar: "\u2966",
        lvertneqq: "\u2268\uFE00",
        lvnE: "\u2268\uFE00",
        macr: "\xAF",
        male: "\u2642",
        malt: "\u2720",
        maltese: "\u2720",
        Map: "\u2905",
        map: "\u21A6",
        mapsto: "\u21A6",
        mapstodown: "\u21A7",
        mapstoleft: "\u21A4",
        mapstoup: "\u21A5",
        marker: "\u25AE",
        mcomma: "\u2A29",
        Mcy: "\u041C",
        mcy: "\u043C",
        mdash: "\u2014",
        mDDot: "\u223A",
        measuredangle: "\u2221",
        MediumSpace: "\u205F",
        Mellintrf: "\u2133",
        Mfr: "\u{1D510}",
        mfr: "\u{1D52A}",
        mho: "\u2127",
        micro: "\xB5",
        mid: "\u2223",
        midast: "*",
        midcir: "\u2AF0",
        middot: "\xB7",
        minus: "\u2212",
        minusb: "\u229F",
        minusd: "\u2238",
        minusdu: "\u2A2A",
        MinusPlus: "\u2213",
        mlcp: "\u2ADB",
        mldr: "\u2026",
        mnplus: "\u2213",
        models: "\u22A7",
        Mopf: "\u{1D544}",
        mopf: "\u{1D55E}",
        mp: "\u2213",
        Mscr: "\u2133",
        mscr: "\u{1D4C2}",
        mstpos: "\u223E",
        Mu: "\u039C",
        mu: "\u03BC",
        multimap: "\u22B8",
        mumap: "\u22B8",
        nabla: "\u2207",
        Nacute: "\u0143",
        nacute: "\u0144",
        nang: "\u2220\u20D2",
        nap: "\u2249",
        napE: "\u2A70\u0338",
        napid: "\u224B\u0338",
        napos: "\u0149",
        napprox: "\u2249",
        natur: "\u266E",
        natural: "\u266E",
        naturals: "\u2115",
        nbsp: "\xA0",
        nbump: "\u224E\u0338",
        nbumpe: "\u224F\u0338",
        ncap: "\u2A43",
        Ncaron: "\u0147",
        ncaron: "\u0148",
        Ncedil: "\u0145",
        ncedil: "\u0146",
        ncong: "\u2247",
        ncongdot: "\u2A6D\u0338",
        ncup: "\u2A42",
        Ncy: "\u041D",
        ncy: "\u043D",
        ndash: "\u2013",
        ne: "\u2260",
        nearhk: "\u2924",
        neArr: "\u21D7",
        nearr: "\u2197",
        nearrow: "\u2197",
        nedot: "\u2250\u0338",
        NegativeMediumSpace: "\u200B",
        NegativeThickSpace: "\u200B",
        NegativeThinSpace: "\u200B",
        NegativeVeryThinSpace: "\u200B",
        nequiv: "\u2262",
        nesear: "\u2928",
        nesim: "\u2242\u0338",
        NestedGreaterGreater: "\u226B",
        NestedLessLess: "\u226A",
        NewLine: "\n",
        nexist: "\u2204",
        nexists: "\u2204",
        Nfr: "\u{1D511}",
        nfr: "\u{1D52B}",
        ngE: "\u2267\u0338",
        nge: "\u2271",
        ngeq: "\u2271",
        ngeqq: "\u2267\u0338",
        ngeqslant: "\u2A7E\u0338",
        nges: "\u2A7E\u0338",
        nGg: "\u22D9\u0338",
        ngsim: "\u2275",
        nGt: "\u226B\u20D2",
        ngt: "\u226F",
        ngtr: "\u226F",
        nGtv: "\u226B\u0338",
        nhArr: "\u21CE",
        nharr: "\u21AE",
        nhpar: "\u2AF2",
        ni: "\u220B",
        nis: "\u22FC",
        nisd: "\u22FA",
        niv: "\u220B",
        NJcy: "\u040A",
        njcy: "\u045A",
        nlArr: "\u21CD",
        nlarr: "\u219A",
        nldr: "\u2025",
        nlE: "\u2266\u0338",
        nle: "\u2270",
        nLeftarrow: "\u21CD",
        nleftarrow: "\u219A",
        nLeftrightarrow: "\u21CE",
        nleftrightarrow: "\u21AE",
        nleq: "\u2270",
        nleqq: "\u2266\u0338",
        nleqslant: "\u2A7D\u0338",
        nles: "\u2A7D\u0338",
        nless: "\u226E",
        nLl: "\u22D8\u0338",
        nlsim: "\u2274",
        nLt: "\u226A\u20D2",
        nlt: "\u226E",
        nltri: "\u22EA",
        nltrie: "\u22EC",
        nLtv: "\u226A\u0338",
        nmid: "\u2224",
        NoBreak: "\u2060",
        NonBreakingSpace: "\xA0",
        Nopf: "\u2115",
        nopf: "\u{1D55F}",
        Not: "\u2AEC",
        not: "\xAC",
        NotCongruent: "\u2262",
        NotCupCap: "\u226D",
        NotDoubleVerticalBar: "\u2226",
        NotElement: "\u2209",
        NotEqual: "\u2260",
        NotEqualTilde: "\u2242\u0338",
        NotExists: "\u2204",
        NotGreater: "\u226F",
        NotGreaterEqual: "\u2271",
        NotGreaterFullEqual: "\u2267\u0338",
        NotGreaterGreater: "\u226B\u0338",
        NotGreaterLess: "\u2279",
        NotGreaterSlantEqual: "\u2A7E\u0338",
        NotGreaterTilde: "\u2275",
        NotHumpDownHump: "\u224E\u0338",
        NotHumpEqual: "\u224F\u0338",
        notin: "\u2209",
        notindot: "\u22F5\u0338",
        notinE: "\u22F9\u0338",
        notinva: "\u2209",
        notinvb: "\u22F7",
        notinvc: "\u22F6",
        NotLeftTriangle: "\u22EA",
        NotLeftTriangleBar: "\u29CF\u0338",
        NotLeftTriangleEqual: "\u22EC",
        NotLess: "\u226E",
        NotLessEqual: "\u2270",
        NotLessGreater: "\u2278",
        NotLessLess: "\u226A\u0338",
        NotLessSlantEqual: "\u2A7D\u0338",
        NotLessTilde: "\u2274",
        NotNestedGreaterGreater: "\u2AA2\u0338",
        NotNestedLessLess: "\u2AA1\u0338",
        notni: "\u220C",
        notniva: "\u220C",
        notnivb: "\u22FE",
        notnivc: "\u22FD",
        NotPrecedes: "\u2280",
        NotPrecedesEqual: "\u2AAF\u0338",
        NotPrecedesSlantEqual: "\u22E0",
        NotReverseElement: "\u220C",
        NotRightTriangle: "\u22EB",
        NotRightTriangleBar: "\u29D0\u0338",
        NotRightTriangleEqual: "\u22ED",
        NotSquareSubset: "\u228F\u0338",
        NotSquareSubsetEqual: "\u22E2",
        NotSquareSuperset: "\u2290\u0338",
        NotSquareSupersetEqual: "\u22E3",
        NotSubset: "\u2282\u20D2",
        NotSubsetEqual: "\u2288",
        NotSucceeds: "\u2281",
        NotSucceedsEqual: "\u2AB0\u0338",
        NotSucceedsSlantEqual: "\u22E1",
        NotSucceedsTilde: "\u227F\u0338",
        NotSuperset: "\u2283\u20D2",
        NotSupersetEqual: "\u2289",
        NotTilde: "\u2241",
        NotTildeEqual: "\u2244",
        NotTildeFullEqual: "\u2247",
        NotTildeTilde: "\u2249",
        NotVerticalBar: "\u2224",
        npar: "\u2226",
        nparallel: "\u2226",
        nparsl: "\u2AFD\u20E5",
        npart: "\u2202\u0338",
        npolint: "\u2A14",
        npr: "\u2280",
        nprcue: "\u22E0",
        npre: "\u2AAF\u0338",
        nprec: "\u2280",
        npreceq: "\u2AAF\u0338",
        nrArr: "\u21CF",
        nrarr: "\u219B",
        nrarrc: "\u2933\u0338",
        nrarrw: "\u219D\u0338",
        nRightarrow: "\u21CF",
        nrightarrow: "\u219B",
        nrtri: "\u22EB",
        nrtrie: "\u22ED",
        nsc: "\u2281",
        nsccue: "\u22E1",
        nsce: "\u2AB0\u0338",
        Nscr: "\u{1D4A9}",
        nscr: "\u{1D4C3}",
        nshortmid: "\u2224",
        nshortparallel: "\u2226",
        nsim: "\u2241",
        nsime: "\u2244",
        nsimeq: "\u2244",
        nsmid: "\u2224",
        nspar: "\u2226",
        nsqsube: "\u22E2",
        nsqsupe: "\u22E3",
        nsub: "\u2284",
        nsubE: "\u2AC5\u0338",
        nsube: "\u2288",
        nsubset: "\u2282\u20D2",
        nsubseteq: "\u2288",
        nsubseteqq: "\u2AC5\u0338",
        nsucc: "\u2281",
        nsucceq: "\u2AB0\u0338",
        nsup: "\u2285",
        nsupE: "\u2AC6\u0338",
        nsupe: "\u2289",
        nsupset: "\u2283\u20D2",
        nsupseteq: "\u2289",
        nsupseteqq: "\u2AC6\u0338",
        ntgl: "\u2279",
        Ntilde: "\xD1",
        ntilde: "\xF1",
        ntlg: "\u2278",
        ntriangleleft: "\u22EA",
        ntrianglelefteq: "\u22EC",
        ntriangleright: "\u22EB",
        ntrianglerighteq: "\u22ED",
        Nu: "\u039D",
        nu: "\u03BD",
        num: "#",
        numero: "\u2116",
        numsp: "\u2007",
        nvap: "\u224D\u20D2",
        nVDash: "\u22AF",
        nVdash: "\u22AE",
        nvDash: "\u22AD",
        nvdash: "\u22AC",
        nvge: "\u2265\u20D2",
        nvgt: ">\u20D2",
        nvHarr: "\u2904",
        nvinfin: "\u29DE",
        nvlArr: "\u2902",
        nvle: "\u2264\u20D2",
        nvlt: "<\u20D2",
        nvltrie: "\u22B4\u20D2",
        nvrArr: "\u2903",
        nvrtrie: "\u22B5\u20D2",
        nvsim: "\u223C\u20D2",
        nwarhk: "\u2923",
        nwArr: "\u21D6",
        nwarr: "\u2196",
        nwarrow: "\u2196",
        nwnear: "\u2927",
        Oacute: "\xD3",
        oacute: "\xF3",
        oast: "\u229B",
        ocir: "\u229A",
        Ocirc: "\xD4",
        ocirc: "\xF4",
        Ocy: "\u041E",
        ocy: "\u043E",
        odash: "\u229D",
        Odblac: "\u0150",
        odblac: "\u0151",
        odiv: "\u2A38",
        odot: "\u2299",
        odsold: "\u29BC",
        OElig: "\u0152",
        oelig: "\u0153",
        ofcir: "\u29BF",
        Ofr: "\u{1D512}",
        ofr: "\u{1D52C}",
        ogon: "\u02DB",
        Ograve: "\xD2",
        ograve: "\xF2",
        ogt: "\u29C1",
        ohbar: "\u29B5",
        ohm: "\u03A9",
        oint: "\u222E",
        olarr: "\u21BA",
        olcir: "\u29BE",
        olcross: "\u29BB",
        oline: "\u203E",
        olt: "\u29C0",
        Omacr: "\u014C",
        omacr: "\u014D",
        Omega: "\u03A9",
        omega: "\u03C9",
        Omicron: "\u039F",
        omicron: "\u03BF",
        omid: "\u29B6",
        ominus: "\u2296",
        Oopf: "\u{1D546}",
        oopf: "\u{1D560}",
        opar: "\u29B7",
        OpenCurlyDoubleQuote: "\u201C",
        OpenCurlyQuote: "\u2018",
        operp: "\u29B9",
        oplus: "\u2295",
        Or: "\u2A54",
        or: "\u2228",
        orarr: "\u21BB",
        ord: "\u2A5D",
        order: "\u2134",
        orderof: "\u2134",
        ordf: "\xAA",
        ordm: "\xBA",
        origof: "\u22B6",
        oror: "\u2A56",
        orslope: "\u2A57",
        orv: "\u2A5B",
        oS: "\u24C8",
        Oscr: "\u{1D4AA}",
        oscr: "\u2134",
        Oslash: "\xD8",
        oslash: "\xF8",
        osol: "\u2298",
        Otilde: "\xD5",
        otilde: "\xF5",
        Otimes: "\u2A37",
        otimes: "\u2297",
        otimesas: "\u2A36",
        Ouml: "\xD6",
        ouml: "\xF6",
        ovbar: "\u233D",
        OverBar: "\u203E",
        OverBrace: "\u23DE",
        OverBracket: "\u23B4",
        OverParenthesis: "\u23DC",
        par: "\u2225",
        para: "\xB6",
        parallel: "\u2225",
        parsim: "\u2AF3",
        parsl: "\u2AFD",
        part: "\u2202",
        PartialD: "\u2202",
        Pcy: "\u041F",
        pcy: "\u043F",
        percnt: "%",
        period: ".",
        permil: "\u2030",
        perp: "\u22A5",
        pertenk: "\u2031",
        Pfr: "\u{1D513}",
        pfr: "\u{1D52D}",
        Phi: "\u03A6",
        phi: "\u03C6",
        phiv: "\u03D5",
        phmmat: "\u2133",
        phone: "\u260E",
        Pi: "\u03A0",
        pi: "\u03C0",
        pitchfork: "\u22D4",
        piv: "\u03D6",
        planck: "\u210F",
        planckh: "\u210E",
        plankv: "\u210F",
        plus: "+",
        plusacir: "\u2A23",
        plusb: "\u229E",
        pluscir: "\u2A22",
        plusdo: "\u2214",
        plusdu: "\u2A25",
        pluse: "\u2A72",
        PlusMinus: "\xB1",
        plusmn: "\xB1",
        plussim: "\u2A26",
        plustwo: "\u2A27",
        pm: "\xB1",
        Poincareplane: "\u210C",
        pointint: "\u2A15",
        Popf: "\u2119",
        popf: "\u{1D561}",
        pound: "\xA3",
        Pr: "\u2ABB",
        pr: "\u227A",
        prap: "\u2AB7",
        prcue: "\u227C",
        prE: "\u2AB3",
        pre: "\u2AAF",
        prec: "\u227A",
        precapprox: "\u2AB7",
        preccurlyeq: "\u227C",
        Precedes: "\u227A",
        PrecedesEqual: "\u2AAF",
        PrecedesSlantEqual: "\u227C",
        PrecedesTilde: "\u227E",
        preceq: "\u2AAF",
        precnapprox: "\u2AB9",
        precneqq: "\u2AB5",
        precnsim: "\u22E8",
        precsim: "\u227E",
        Prime: "\u2033",
        prime: "\u2032",
        primes: "\u2119",
        prnap: "\u2AB9",
        prnE: "\u2AB5",
        prnsim: "\u22E8",
        prod: "\u220F",
        Product: "\u220F",
        profalar: "\u232E",
        profline: "\u2312",
        profsurf: "\u2313",
        prop: "\u221D",
        Proportion: "\u2237",
        Proportional: "\u221D",
        propto: "\u221D",
        prsim: "\u227E",
        prurel: "\u22B0",
        Pscr: "\u{1D4AB}",
        pscr: "\u{1D4C5}",
        Psi: "\u03A8",
        psi: "\u03C8",
        puncsp: "\u2008",
        Qfr: "\u{1D514}",
        qfr: "\u{1D52E}",
        qint: "\u2A0C",
        Qopf: "\u211A",
        qopf: "\u{1D562}",
        qprime: "\u2057",
        Qscr: "\u{1D4AC}",
        qscr: "\u{1D4C6}",
        quaternions: "\u210D",
        quatint: "\u2A16",
        quest: "?",
        questeq: "\u225F",
        QUOT: '"',
        quot: '"',
        rAarr: "\u21DB",
        race: "\u223D\u0331",
        Racute: "\u0154",
        racute: "\u0155",
        radic: "\u221A",
        raemptyv: "\u29B3",
        Rang: "\u27EB",
        rang: "\u27E9",
        rangd: "\u2992",
        range: "\u29A5",
        rangle: "\u27E9",
        raquo: "\xBB",
        Rarr: "\u21A0",
        rArr: "\u21D2",
        rarr: "\u2192",
        rarrap: "\u2975",
        rarrb: "\u21E5",
        rarrbfs: "\u2920",
        rarrc: "\u2933",
        rarrfs: "\u291E",
        rarrhk: "\u21AA",
        rarrlp: "\u21AC",
        rarrpl: "\u2945",
        rarrsim: "\u2974",
        Rarrtl: "\u2916",
        rarrtl: "\u21A3",
        rarrw: "\u219D",
        rAtail: "\u291C",
        ratail: "\u291A",
        ratio: "\u2236",
        rationals: "\u211A",
        RBarr: "\u2910",
        rBarr: "\u290F",
        rbarr: "\u290D",
        rbbrk: "\u2773",
        rbrace: "}",
        rbrack: "]",
        rbrke: "\u298C",
        rbrksld: "\u298E",
        rbrkslu: "\u2990",
        Rcaron: "\u0158",
        rcaron: "\u0159",
        Rcedil: "\u0156",
        rcedil: "\u0157",
        rceil: "\u2309",
        rcub: "}",
        Rcy: "\u0420",
        rcy: "\u0440",
        rdca: "\u2937",
        rdldhar: "\u2969",
        rdquo: "\u201D",
        rdquor: "\u201D",
        rdsh: "\u21B3",
        Re: "\u211C",
        real: "\u211C",
        realine: "\u211B",
        realpart: "\u211C",
        reals: "\u211D",
        rect: "\u25AD",
        REG: "\xAE",
        reg: "\xAE",
        ReverseElement: "\u220B",
        ReverseEquilibrium: "\u21CB",
        ReverseUpEquilibrium: "\u296F",
        rfisht: "\u297D",
        rfloor: "\u230B",
        Rfr: "\u211C",
        rfr: "\u{1D52F}",
        rHar: "\u2964",
        rhard: "\u21C1",
        rharu: "\u21C0",
        rharul: "\u296C",
        Rho: "\u03A1",
        rho: "\u03C1",
        rhov: "\u03F1",
        RightAngleBracket: "\u27E9",
        RightArrow: "\u2192",
        Rightarrow: "\u21D2",
        rightarrow: "\u2192",
        RightArrowBar: "\u21E5",
        RightArrowLeftArrow: "\u21C4",
        rightarrowtail: "\u21A3",
        RightCeiling: "\u2309",
        RightDoubleBracket: "\u27E7",
        RightDownTeeVector: "\u295D",
        RightDownVector: "\u21C2",
        RightDownVectorBar: "\u2955",
        RightFloor: "\u230B",
        rightharpoondown: "\u21C1",
        rightharpoonup: "\u21C0",
        rightleftarrows: "\u21C4",
        rightleftharpoons: "\u21CC",
        rightrightarrows: "\u21C9",
        rightsquigarrow: "\u219D",
        RightTee: "\u22A2",
        RightTeeArrow: "\u21A6",
        RightTeeVector: "\u295B",
        rightthreetimes: "\u22CC",
        RightTriangle: "\u22B3",
        RightTriangleBar: "\u29D0",
        RightTriangleEqual: "\u22B5",
        RightUpDownVector: "\u294F",
        RightUpTeeVector: "\u295C",
        RightUpVector: "\u21BE",
        RightUpVectorBar: "\u2954",
        RightVector: "\u21C0",
        RightVectorBar: "\u2953",
        ring: "\u02DA",
        risingdotseq: "\u2253",
        rlarr: "\u21C4",
        rlhar: "\u21CC",
        rlm: "\u200F",
        rmoust: "\u23B1",
        rmoustache: "\u23B1",
        rnmid: "\u2AEE",
        roang: "\u27ED",
        roarr: "\u21FE",
        robrk: "\u27E7",
        ropar: "\u2986",
        Ropf: "\u211D",
        ropf: "\u{1D563}",
        roplus: "\u2A2E",
        rotimes: "\u2A35",
        RoundImplies: "\u2970",
        rpar: ")",
        rpargt: "\u2994",
        rppolint: "\u2A12",
        rrarr: "\u21C9",
        Rrightarrow: "\u21DB",
        rsaquo: "\u203A",
        Rscr: "\u211B",
        rscr: "\u{1D4C7}",
        Rsh: "\u21B1",
        rsh: "\u21B1",
        rsqb: "]",
        rsquo: "\u2019",
        rsquor: "\u2019",
        rthree: "\u22CC",
        rtimes: "\u22CA",
        rtri: "\u25B9",
        rtrie: "\u22B5",
        rtrif: "\u25B8",
        rtriltri: "\u29CE",
        RuleDelayed: "\u29F4",
        ruluhar: "\u2968",
        rx: "\u211E",
        Sacute: "\u015A",
        sacute: "\u015B",
        sbquo: "\u201A",
        Sc: "\u2ABC",
        sc: "\u227B",
        scap: "\u2AB8",
        Scaron: "\u0160",
        scaron: "\u0161",
        sccue: "\u227D",
        scE: "\u2AB4",
        sce: "\u2AB0",
        Scedil: "\u015E",
        scedil: "\u015F",
        Scirc: "\u015C",
        scirc: "\u015D",
        scnap: "\u2ABA",
        scnE: "\u2AB6",
        scnsim: "\u22E9",
        scpolint: "\u2A13",
        scsim: "\u227F",
        Scy: "\u0421",
        scy: "\u0441",
        sdot: "\u22C5",
        sdotb: "\u22A1",
        sdote: "\u2A66",
        searhk: "\u2925",
        seArr: "\u21D8",
        searr: "\u2198",
        searrow: "\u2198",
        sect: "\xA7",
        semi: ";",
        seswar: "\u2929",
        setminus: "\u2216",
        setmn: "\u2216",
        sext: "\u2736",
        Sfr: "\u{1D516}",
        sfr: "\u{1D530}",
        sfrown: "\u2322",
        sharp: "\u266F",
        SHCHcy: "\u0429",
        shchcy: "\u0449",
        SHcy: "\u0428",
        shcy: "\u0448",
        ShortDownArrow: "\u2193",
        ShortLeftArrow: "\u2190",
        shortmid: "\u2223",
        shortparallel: "\u2225",
        ShortRightArrow: "\u2192",
        ShortUpArrow: "\u2191",
        shy: "\xAD",
        Sigma: "\u03A3",
        sigma: "\u03C3",
        sigmaf: "\u03C2",
        sigmav: "\u03C2",
        sim: "\u223C",
        simdot: "\u2A6A",
        sime: "\u2243",
        simeq: "\u2243",
        simg: "\u2A9E",
        simgE: "\u2AA0",
        siml: "\u2A9D",
        simlE: "\u2A9F",
        simne: "\u2246",
        simplus: "\u2A24",
        simrarr: "\u2972",
        slarr: "\u2190",
        SmallCircle: "\u2218",
        smallsetminus: "\u2216",
        smashp: "\u2A33",
        smeparsl: "\u29E4",
        smid: "\u2223",
        smile: "\u2323",
        smt: "\u2AAA",
        smte: "\u2AAC",
        smtes: "\u2AAC\uFE00",
        SOFTcy: "\u042C",
        softcy: "\u044C",
        sol: "/",
        solb: "\u29C4",
        solbar: "\u233F",
        Sopf: "\u{1D54A}",
        sopf: "\u{1D564}",
        spades: "\u2660",
        spadesuit: "\u2660",
        spar: "\u2225",
        sqcap: "\u2293",
        sqcaps: "\u2293\uFE00",
        sqcup: "\u2294",
        sqcups: "\u2294\uFE00",
        Sqrt: "\u221A",
        sqsub: "\u228F",
        sqsube: "\u2291",
        sqsubset: "\u228F",
        sqsubseteq: "\u2291",
        sqsup: "\u2290",
        sqsupe: "\u2292",
        sqsupset: "\u2290",
        sqsupseteq: "\u2292",
        squ: "\u25A1",
        Square: "\u25A1",
        square: "\u25A1",
        SquareIntersection: "\u2293",
        SquareSubset: "\u228F",
        SquareSubsetEqual: "\u2291",
        SquareSuperset: "\u2290",
        SquareSupersetEqual: "\u2292",
        SquareUnion: "\u2294",
        squarf: "\u25AA",
        squf: "\u25AA",
        srarr: "\u2192",
        Sscr: "\u{1D4AE}",
        sscr: "\u{1D4C8}",
        ssetmn: "\u2216",
        ssmile: "\u2323",
        sstarf: "\u22C6",
        Star: "\u22C6",
        star: "\u2606",
        starf: "\u2605",
        straightepsilon: "\u03F5",
        straightphi: "\u03D5",
        strns: "\xAF",
        Sub: "\u22D0",
        sub: "\u2282",
        subdot: "\u2ABD",
        subE: "\u2AC5",
        sube: "\u2286",
        subedot: "\u2AC3",
        submult: "\u2AC1",
        subnE: "\u2ACB",
        subne: "\u228A",
        subplus: "\u2ABF",
        subrarr: "\u2979",
        Subset: "\u22D0",
        subset: "\u2282",
        subseteq: "\u2286",
        subseteqq: "\u2AC5",
        SubsetEqual: "\u2286",
        subsetneq: "\u228A",
        subsetneqq: "\u2ACB",
        subsim: "\u2AC7",
        subsub: "\u2AD5",
        subsup: "\u2AD3",
        succ: "\u227B",
        succapprox: "\u2AB8",
        succcurlyeq: "\u227D",
        Succeeds: "\u227B",
        SucceedsEqual: "\u2AB0",
        SucceedsSlantEqual: "\u227D",
        SucceedsTilde: "\u227F",
        succeq: "\u2AB0",
        succnapprox: "\u2ABA",
        succneqq: "\u2AB6",
        succnsim: "\u22E9",
        succsim: "\u227F",
        SuchThat: "\u220B",
        Sum: "\u2211",
        sum: "\u2211",
        sung: "\u266A",
        Sup: "\u22D1",
        sup: "\u2283",
        sup1: "\xB9",
        sup2: "\xB2",
        sup3: "\xB3",
        supdot: "\u2ABE",
        supdsub: "\u2AD8",
        supE: "\u2AC6",
        supe: "\u2287",
        supedot: "\u2AC4",
        Superset: "\u2283",
        SupersetEqual: "\u2287",
        suphsol: "\u27C9",
        suphsub: "\u2AD7",
        suplarr: "\u297B",
        supmult: "\u2AC2",
        supnE: "\u2ACC",
        supne: "\u228B",
        supplus: "\u2AC0",
        Supset: "\u22D1",
        supset: "\u2283",
        supseteq: "\u2287",
        supseteqq: "\u2AC6",
        supsetneq: "\u228B",
        supsetneqq: "\u2ACC",
        supsim: "\u2AC8",
        supsub: "\u2AD4",
        supsup: "\u2AD6",
        swarhk: "\u2926",
        swArr: "\u21D9",
        swarr: "\u2199",
        swarrow: "\u2199",
        swnwar: "\u292A",
        szlig: "\xDF",
        Tab: "	",
        target: "\u2316",
        Tau: "\u03A4",
        tau: "\u03C4",
        tbrk: "\u23B4",
        Tcaron: "\u0164",
        tcaron: "\u0165",
        Tcedil: "\u0162",
        tcedil: "\u0163",
        Tcy: "\u0422",
        tcy: "\u0442",
        tdot: "\u20DB",
        telrec: "\u2315",
        Tfr: "\u{1D517}",
        tfr: "\u{1D531}",
        there4: "\u2234",
        Therefore: "\u2234",
        therefore: "\u2234",
        Theta: "\u0398",
        theta: "\u03B8",
        thetasym: "\u03D1",
        thetav: "\u03D1",
        thickapprox: "\u2248",
        thicksim: "\u223C",
        ThickSpace: "\u205F\u200A",
        thinsp: "\u2009",
        ThinSpace: "\u2009",
        thkap: "\u2248",
        thksim: "\u223C",
        THORN: "\xDE",
        thorn: "\xFE",
        Tilde: "\u223C",
        tilde: "\u02DC",
        TildeEqual: "\u2243",
        TildeFullEqual: "\u2245",
        TildeTilde: "\u2248",
        times: "\xD7",
        timesb: "\u22A0",
        timesbar: "\u2A31",
        timesd: "\u2A30",
        tint: "\u222D",
        toea: "\u2928",
        top: "\u22A4",
        topbot: "\u2336",
        topcir: "\u2AF1",
        Topf: "\u{1D54B}",
        topf: "\u{1D565}",
        topfork: "\u2ADA",
        tosa: "\u2929",
        tprime: "\u2034",
        TRADE: "\u2122",
        trade: "\u2122",
        triangle: "\u25B5",
        triangledown: "\u25BF",
        triangleleft: "\u25C3",
        trianglelefteq: "\u22B4",
        triangleq: "\u225C",
        triangleright: "\u25B9",
        trianglerighteq: "\u22B5",
        tridot: "\u25EC",
        trie: "\u225C",
        triminus: "\u2A3A",
        TripleDot: "\u20DB",
        triplus: "\u2A39",
        trisb: "\u29CD",
        tritime: "\u2A3B",
        trpezium: "\u23E2",
        Tscr: "\u{1D4AF}",
        tscr: "\u{1D4C9}",
        TScy: "\u0426",
        tscy: "\u0446",
        TSHcy: "\u040B",
        tshcy: "\u045B",
        Tstrok: "\u0166",
        tstrok: "\u0167",
        twixt: "\u226C",
        twoheadleftarrow: "\u219E",
        twoheadrightarrow: "\u21A0",
        Uacute: "\xDA",
        uacute: "\xFA",
        Uarr: "\u219F",
        uArr: "\u21D1",
        uarr: "\u2191",
        Uarrocir: "\u2949",
        Ubrcy: "\u040E",
        ubrcy: "\u045E",
        Ubreve: "\u016C",
        ubreve: "\u016D",
        Ucirc: "\xDB",
        ucirc: "\xFB",
        Ucy: "\u0423",
        ucy: "\u0443",
        udarr: "\u21C5",
        Udblac: "\u0170",
        udblac: "\u0171",
        udhar: "\u296E",
        ufisht: "\u297E",
        Ufr: "\u{1D518}",
        ufr: "\u{1D532}",
        Ugrave: "\xD9",
        ugrave: "\xF9",
        uHar: "\u2963",
        uharl: "\u21BF",
        uharr: "\u21BE",
        uhblk: "\u2580",
        ulcorn: "\u231C",
        ulcorner: "\u231C",
        ulcrop: "\u230F",
        ultri: "\u25F8",
        Umacr: "\u016A",
        umacr: "\u016B",
        uml: "\xA8",
        UnderBar: "_",
        UnderBrace: "\u23DF",
        UnderBracket: "\u23B5",
        UnderParenthesis: "\u23DD",
        Union: "\u22C3",
        UnionPlus: "\u228E",
        Uogon: "\u0172",
        uogon: "\u0173",
        Uopf: "\u{1D54C}",
        uopf: "\u{1D566}",
        UpArrow: "\u2191",
        Uparrow: "\u21D1",
        uparrow: "\u2191",
        UpArrowBar: "\u2912",
        UpArrowDownArrow: "\u21C5",
        UpDownArrow: "\u2195",
        Updownarrow: "\u21D5",
        updownarrow: "\u2195",
        UpEquilibrium: "\u296E",
        upharpoonleft: "\u21BF",
        upharpoonright: "\u21BE",
        uplus: "\u228E",
        UpperLeftArrow: "\u2196",
        UpperRightArrow: "\u2197",
        Upsi: "\u03D2",
        upsi: "\u03C5",
        upsih: "\u03D2",
        Upsilon: "\u03A5",
        upsilon: "\u03C5",
        UpTee: "\u22A5",
        UpTeeArrow: "\u21A5",
        upuparrows: "\u21C8",
        urcorn: "\u231D",
        urcorner: "\u231D",
        urcrop: "\u230E",
        Uring: "\u016E",
        uring: "\u016F",
        urtri: "\u25F9",
        Uscr: "\u{1D4B0}",
        uscr: "\u{1D4CA}",
        utdot: "\u22F0",
        Utilde: "\u0168",
        utilde: "\u0169",
        utri: "\u25B5",
        utrif: "\u25B4",
        uuarr: "\u21C8",
        Uuml: "\xDC",
        uuml: "\xFC",
        uwangle: "\u29A7",
        vangrt: "\u299C",
        varepsilon: "\u03F5",
        varkappa: "\u03F0",
        varnothing: "\u2205",
        varphi: "\u03D5",
        varpi: "\u03D6",
        varpropto: "\u221D",
        vArr: "\u21D5",
        varr: "\u2195",
        varrho: "\u03F1",
        varsigma: "\u03C2",
        varsubsetneq: "\u228A\uFE00",
        varsubsetneqq: "\u2ACB\uFE00",
        varsupsetneq: "\u228B\uFE00",
        varsupsetneqq: "\u2ACC\uFE00",
        vartheta: "\u03D1",
        vartriangleleft: "\u22B2",
        vartriangleright: "\u22B3",
        Vbar: "\u2AEB",
        vBar: "\u2AE8",
        vBarv: "\u2AE9",
        Vcy: "\u0412",
        vcy: "\u0432",
        VDash: "\u22AB",
        Vdash: "\u22A9",
        vDash: "\u22A8",
        vdash: "\u22A2",
        Vdashl: "\u2AE6",
        Vee: "\u22C1",
        vee: "\u2228",
        veebar: "\u22BB",
        veeeq: "\u225A",
        vellip: "\u22EE",
        Verbar: "\u2016",
        verbar: "|",
        Vert: "\u2016",
        vert: "|",
        VerticalBar: "\u2223",
        VerticalLine: "|",
        VerticalSeparator: "\u2758",
        VerticalTilde: "\u2240",
        VeryThinSpace: "\u200A",
        Vfr: "\u{1D519}",
        vfr: "\u{1D533}",
        vltri: "\u22B2",
        vnsub: "\u2282\u20D2",
        vnsup: "\u2283\u20D2",
        Vopf: "\u{1D54D}",
        vopf: "\u{1D567}",
        vprop: "\u221D",
        vrtri: "\u22B3",
        Vscr: "\u{1D4B1}",
        vscr: "\u{1D4CB}",
        vsubnE: "\u2ACB\uFE00",
        vsubne: "\u228A\uFE00",
        vsupnE: "\u2ACC\uFE00",
        vsupne: "\u228B\uFE00",
        Vvdash: "\u22AA",
        vzigzag: "\u299A",
        Wcirc: "\u0174",
        wcirc: "\u0175",
        wedbar: "\u2A5F",
        Wedge: "\u22C0",
        wedge: "\u2227",
        wedgeq: "\u2259",
        weierp: "\u2118",
        Wfr: "\u{1D51A}",
        wfr: "\u{1D534}",
        Wopf: "\u{1D54E}",
        wopf: "\u{1D568}",
        wp: "\u2118",
        wr: "\u2240",
        wreath: "\u2240",
        Wscr: "\u{1D4B2}",
        wscr: "\u{1D4CC}",
        xcap: "\u22C2",
        xcirc: "\u25EF",
        xcup: "\u22C3",
        xdtri: "\u25BD",
        Xfr: "\u{1D51B}",
        xfr: "\u{1D535}",
        xhArr: "\u27FA",
        xharr: "\u27F7",
        Xi: "\u039E",
        xi: "\u03BE",
        xlArr: "\u27F8",
        xlarr: "\u27F5",
        xmap: "\u27FC",
        xnis: "\u22FB",
        xodot: "\u2A00",
        Xopf: "\u{1D54F}",
        xopf: "\u{1D569}",
        xoplus: "\u2A01",
        xotime: "\u2A02",
        xrArr: "\u27F9",
        xrarr: "\u27F6",
        Xscr: "\u{1D4B3}",
        xscr: "\u{1D4CD}",
        xsqcup: "\u2A06",
        xuplus: "\u2A04",
        xutri: "\u25B3",
        xvee: "\u22C1",
        xwedge: "\u22C0",
        Yacute: "\xDD",
        yacute: "\xFD",
        YAcy: "\u042F",
        yacy: "\u044F",
        Ycirc: "\u0176",
        ycirc: "\u0177",
        Ycy: "\u042B",
        ycy: "\u044B",
        yen: "\xA5",
        Yfr: "\u{1D51C}",
        yfr: "\u{1D536}",
        YIcy: "\u0407",
        yicy: "\u0457",
        Yopf: "\u{1D550}",
        yopf: "\u{1D56A}",
        Yscr: "\u{1D4B4}",
        yscr: "\u{1D4CE}",
        YUcy: "\u042E",
        yucy: "\u044E",
        Yuml: "\u0178",
        yuml: "\xFF",
        Zacute: "\u0179",
        zacute: "\u017A",
        Zcaron: "\u017D",
        zcaron: "\u017E",
        Zcy: "\u0417",
        zcy: "\u0437",
        Zdot: "\u017B",
        zdot: "\u017C",
        zeetrf: "\u2128",
        ZeroWidthSpace: "\u200B",
        Zeta: "\u0396",
        zeta: "\u03B6",
        Zfr: "\u2128",
        zfr: "\u{1D537}",
        ZHcy: "\u0416",
        zhcy: "\u0436",
        zigrarr: "\u21DD",
        Zopf: "\u2124",
        zopf: "\u{1D56B}",
        Zscr: "\u{1D4B5}",
        zscr: "\u{1D4CF}",
        zwj: "\u200D",
        zwnj: "\u200C"
      });
      exports.entityMap = exports.HTML_ENTITIES;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/sax.js
  var require_sax = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/sax.js"(exports) {
      "use strict";
      var conventions = require_conventions();
      var g = require_grammar();
      var errors = require_errors();
      var isHTMLEscapableRawTextElement = conventions.isHTMLEscapableRawTextElement;
      var isHTMLMimeType = conventions.isHTMLMimeType;
      var isHTMLRawTextElement = conventions.isHTMLRawTextElement;
      var hasOwn = conventions.hasOwn;
      var NAMESPACE = conventions.NAMESPACE;
      var ParseError = errors.ParseError;
      var DOMException = errors.DOMException;
      var S_TAG = 0;
      var S_ATTR = 1;
      var S_ATTR_SPACE = 2;
      var S_EQ = 3;
      var S_ATTR_NOQUOT_VALUE = 4;
      var S_ATTR_END = 5;
      var S_TAG_SPACE = 6;
      var S_TAG_CLOSE = 7;
      function XMLReader() {
      }
      XMLReader.prototype = {
        parse: function(source, defaultNSMap, entityMap) {
          var domBuilder = this.domBuilder;
          domBuilder.startDocument();
          _copy(defaultNSMap, defaultNSMap = /* @__PURE__ */ Object.create(null));
          parse(source, defaultNSMap, entityMap, domBuilder, this.errorHandler);
          domBuilder.endDocument();
        }
      };
      var ENTITY_REG = /&#?\w+;?/g;
      function parse(source, defaultNSMapCopy, entityMap, domBuilder, errorHandler) {
        var isHTML = isHTMLMimeType(domBuilder.mimeType);
        if (source.indexOf(g.UNICODE_REPLACEMENT_CHARACTER) >= 0) {
          errorHandler.warning("Unicode replacement character detected, source encoding issues?");
        }
        function fixedFromCharCode(code) {
          if (code > 65535) {
            code -= 65536;
            var surrogate1 = 55296 + (code >> 10), surrogate2 = 56320 + (code & 1023);
            return String.fromCharCode(surrogate1, surrogate2);
          } else {
            return String.fromCharCode(code);
          }
        }
        function entityReplacer(a2) {
          var complete = a2[a2.length - 1] === ";" ? a2 : a2 + ";";
          if (!isHTML && complete !== a2) {
            errorHandler.error("EntityRef: expecting ;");
            return a2;
          }
          var match = g.Reference.exec(complete);
          if (!match || match[0].length !== complete.length) {
            errorHandler.error("entity not matching Reference production: " + a2);
            return a2;
          }
          var k = complete.slice(1, -1);
          if (hasOwn(entityMap, k)) {
            return entityMap[k];
          } else if (k.charAt(0) === "#") {
            return fixedFromCharCode(parseInt(k.substring(1).replace("x", "0x")));
          } else {
            errorHandler.error("entity not found:" + a2);
            return a2;
          }
        }
        function appendText(end2) {
          if (end2 > start) {
            var xt = source.substring(start, end2).replace(ENTITY_REG, entityReplacer);
            locator && position(start);
            domBuilder.characters(xt, 0, end2 - start);
            start = end2;
          }
        }
        var lineStart = 0;
        var lineEnd = 0;
        var linePattern = /\r\n?|\n|$/g;
        var locator = domBuilder.locator;
        function position(p, m) {
          while (p >= lineEnd && (m = linePattern.exec(source))) {
            lineStart = lineEnd;
            lineEnd = m.index + m[0].length;
            locator.lineNumber++;
          }
          locator.columnNumber = p - lineStart + 1;
        }
        var parseStack = [{ currentNSMap: defaultNSMapCopy }];
        var unclosedTags = [];
        var start = 0;
        while (true) {
          try {
            var tagStart = source.indexOf("<", start);
            if (tagStart < 0) {
              if (!isHTML && unclosedTags.length > 0) {
                return errorHandler.fatalError("unclosed xml tag(s): " + unclosedTags.join(", "));
              }
              if (!source.substring(start).match(/^\s*$/)) {
                var doc = domBuilder.doc;
                var text = doc.createTextNode(source.substring(start));
                if (doc.documentElement) {
                  return errorHandler.error("Extra content at the end of the document");
                }
                doc.appendChild(text);
                domBuilder.currentElement = text;
              }
              return;
            }
            if (tagStart > start) {
              var fromSource = source.substring(start, tagStart);
              if (!isHTML && unclosedTags.length === 0) {
                fromSource = fromSource.replace(new RegExp(g.S_OPT.source, "g"), "");
                fromSource && errorHandler.error("Unexpected content outside root element: '" + fromSource + "'");
              }
              appendText(tagStart);
            }
            switch (source.charAt(tagStart + 1)) {
              case "/":
                var end = source.indexOf(">", tagStart + 2);
                var tagNameRaw = source.substring(tagStart + 2, end > 0 ? end : void 0);
                if (!tagNameRaw) {
                  return errorHandler.fatalError("end tag name missing");
                }
                var tagNameMatch = end > 0 && g.reg("^", g.QName_group, g.S_OPT, "$").exec(tagNameRaw);
                if (!tagNameMatch) {
                  return errorHandler.fatalError('end tag name contains invalid characters: "' + tagNameRaw + '"');
                }
                if (!domBuilder.currentElement && !domBuilder.doc.documentElement) {
                  return;
                }
                var currentTagName = unclosedTags[unclosedTags.length - 1] || domBuilder.currentElement.tagName || domBuilder.doc.documentElement.tagName || "";
                if (currentTagName !== tagNameMatch[1]) {
                  var tagNameLower = tagNameMatch[1].toLowerCase();
                  if (!isHTML || currentTagName.toLowerCase() !== tagNameLower) {
                    return errorHandler.fatalError('Opening and ending tag mismatch: "' + currentTagName + '" != "' + tagNameRaw + '"');
                  }
                }
                var config = parseStack.pop();
                unclosedTags.pop();
                var localNSMap = config.localNSMap;
                domBuilder.endElement(config.uri, config.localName, currentTagName);
                if (localNSMap) {
                  for (var prefix in localNSMap) {
                    if (hasOwn(localNSMap, prefix)) {
                      domBuilder.endPrefixMapping(prefix);
                    }
                  }
                }
                end++;
                break;
              // end element
              case "?":
                locator && position(tagStart);
                end = parseProcessingInstruction(source, tagStart, domBuilder, errorHandler);
                break;
              case "!":
                locator && position(tagStart);
                end = parseDoctypeCommentOrCData(source, tagStart, domBuilder, errorHandler, isHTML);
                break;
              default:
                locator && position(tagStart);
                var el = new ElementAttributes();
                var currentNSMap = parseStack[parseStack.length - 1].currentNSMap;
                var end = parseElementStartPart(source, tagStart, el, currentNSMap, entityReplacer, errorHandler, isHTML);
                var len = el.length;
                if (!el.closed) {
                  if (isHTML && conventions.isHTMLVoidElement(el.tagName)) {
                    el.closed = true;
                  } else {
                    unclosedTags.push(el.tagName);
                  }
                }
                if (locator && len) {
                  var locator2 = copyLocator(locator, {});
                  for (var i = 0; i < len; i++) {
                    var a = el[i];
                    position(a.offset);
                    a.locator = copyLocator(locator, {});
                  }
                  domBuilder.locator = locator2;
                  if (appendElement(el, domBuilder, currentNSMap)) {
                    parseStack.push(el);
                  }
                  domBuilder.locator = locator;
                } else {
                  if (appendElement(el, domBuilder, currentNSMap)) {
                    parseStack.push(el);
                  }
                }
                if (isHTML && !el.closed) {
                  end = parseHtmlSpecialContent(source, end, el.tagName, entityReplacer, domBuilder);
                } else {
                  end++;
                }
            }
          } catch (e) {
            if (e instanceof ParseError) {
              throw e;
            } else if (e instanceof DOMException) {
              throw new ParseError(e.name + ": " + e.message, domBuilder.locator, e);
            }
            errorHandler.error("element parse error: " + e);
            end = -1;
          }
          if (end > start) {
            start = end;
          } else {
            appendText(Math.max(tagStart, start) + 1);
          }
        }
      }
      function copyLocator(f, t) {
        t.lineNumber = f.lineNumber;
        t.columnNumber = f.columnNumber;
        return t;
      }
      function parseElementStartPart(source, start, el, currentNSMap, entityReplacer, errorHandler, isHTML) {
        function addAttribute(qname, value2, startIndex) {
          if (hasOwn(el.attributeNames, qname)) {
            return errorHandler.fatalError("Attribute " + qname + " redefined");
          }
          if (!isHTML && value2.indexOf("<") >= 0) {
            return errorHandler.fatalError("Unescaped '<' not allowed in attributes values");
          }
          el.addValue(
            qname,
            // @see https://www.w3.org/TR/xml/#AVNormalize
            // since the xmldom sax parser does not "interpret" DTD the following is not implemented:
            // - recursive replacement of (DTD) entity references
            // - trimming and collapsing multiple spaces into a single one for attributes that are not of type CDATA
            value2.replace(/[\t\n\r]/g, " ").replace(ENTITY_REG, entityReplacer),
            startIndex
          );
        }
        var attrName;
        var value;
        var p = ++start;
        var s = S_TAG;
        while (true) {
          var c = source.charAt(p);
          switch (c) {
            case "=":
              if (s === S_ATTR) {
                attrName = source.slice(start, p);
                s = S_EQ;
              } else if (s === S_ATTR_SPACE) {
                s = S_EQ;
              } else {
                throw new Error("attribute equal must after attrName");
              }
              break;
            case "'":
            case '"':
              if (s === S_EQ || s === S_ATTR) {
                if (s === S_ATTR) {
                  errorHandler.warning('attribute value must after "="');
                  attrName = source.slice(start, p);
                }
                start = p + 1;
                p = source.indexOf(c, start);
                if (p > 0) {
                  value = source.slice(start, p);
                  addAttribute(attrName, value, start - 1);
                  s = S_ATTR_END;
                } else {
                  throw new Error("attribute value no end '" + c + "' match");
                }
              } else if (s == S_ATTR_NOQUOT_VALUE) {
                value = source.slice(start, p);
                addAttribute(attrName, value, start);
                errorHandler.warning('attribute "' + attrName + '" missed start quot(' + c + ")!!");
                start = p + 1;
                s = S_ATTR_END;
              } else {
                throw new Error('attribute value must after "="');
              }
              break;
            case "/":
              switch (s) {
                case S_TAG:
                  el.setTagName(source.slice(start, p));
                case S_ATTR_END:
                case S_TAG_SPACE:
                case S_TAG_CLOSE:
                  s = S_TAG_CLOSE;
                  el.closed = true;
                case S_ATTR_NOQUOT_VALUE:
                case S_ATTR:
                  break;
                case S_ATTR_SPACE:
                  el.closed = true;
                  break;
                //case S_EQ:
                default:
                  throw new Error("attribute invalid close char('/')");
              }
              break;
            case "":
              errorHandler.error("unexpected end of input");
              if (s == S_TAG) {
                el.setTagName(source.slice(start, p));
              }
              return p;
            case ">":
              switch (s) {
                case S_TAG:
                  el.setTagName(source.slice(start, p));
                case S_ATTR_END:
                case S_TAG_SPACE:
                case S_TAG_CLOSE:
                  break;
                //normal
                case S_ATTR_NOQUOT_VALUE:
                //Compatible state
                case S_ATTR:
                  value = source.slice(start, p);
                  if (value.slice(-1) === "/") {
                    el.closed = true;
                    value = value.slice(0, -1);
                  }
                case S_ATTR_SPACE:
                  if (s === S_ATTR_SPACE) {
                    value = attrName;
                  }
                  if (s == S_ATTR_NOQUOT_VALUE) {
                    errorHandler.warning('attribute "' + value + '" missed quot(")!');
                    addAttribute(attrName, value, start);
                  } else {
                    if (!isHTML) {
                      errorHandler.warning('attribute "' + value + '" missed value!! "' + value + '" instead!!');
                    }
                    addAttribute(value, value, start);
                  }
                  break;
                case S_EQ:
                  if (!isHTML) {
                    return errorHandler.fatalError(`AttValue: ' or " expected`);
                  }
              }
              return p;
            /*xml space '\x20' | #x9 | #xD | #xA; */
            case "\x80":
              c = " ";
            default:
              if (c <= " ") {
                switch (s) {
                  case S_TAG:
                    el.setTagName(source.slice(start, p));
                    s = S_TAG_SPACE;
                    break;
                  case S_ATTR:
                    attrName = source.slice(start, p);
                    s = S_ATTR_SPACE;
                    break;
                  case S_ATTR_NOQUOT_VALUE:
                    var value = source.slice(start, p);
                    errorHandler.warning('attribute "' + value + '" missed quot(")!!');
                    addAttribute(attrName, value, start);
                  case S_ATTR_END:
                    s = S_TAG_SPACE;
                    break;
                }
              } else {
                switch (s) {
                  //case S_TAG:void();break;
                  //case S_ATTR:void();break;
                  //case S_ATTR_NOQUOT_VALUE:void();break;
                  case S_ATTR_SPACE:
                    if (!isHTML) {
                      errorHandler.warning('attribute "' + attrName + '" missed value!! "' + attrName + '" instead2!!');
                    }
                    addAttribute(attrName, attrName, start);
                    start = p;
                    s = S_ATTR;
                    break;
                  case S_ATTR_END:
                    errorHandler.warning('attribute space is required"' + attrName + '"!!');
                  case S_TAG_SPACE:
                    s = S_ATTR;
                    start = p;
                    break;
                  case S_EQ:
                    s = S_ATTR_NOQUOT_VALUE;
                    start = p;
                    break;
                  case S_TAG_CLOSE:
                    throw new Error("elements closed character '/' and '>' must be connected to");
                }
              }
          }
          p++;
        }
      }
      function appendElement(el, domBuilder, currentNSMap) {
        var tagName = el.tagName;
        var localNSMap = null;
        var i = el.length;
        while (i--) {
          var a = el[i];
          var qName = a.qName;
          var value = a.value;
          var nsp = qName.indexOf(":");
          if (nsp > 0) {
            var prefix = a.prefix = qName.slice(0, nsp);
            var localName = qName.slice(nsp + 1);
            var nsPrefix = prefix === "xmlns" && localName;
          } else {
            localName = qName;
            prefix = null;
            nsPrefix = qName === "xmlns" && "";
          }
          a.localName = localName;
          if (nsPrefix !== false) {
            if (localNSMap == null) {
              localNSMap = /* @__PURE__ */ Object.create(null);
              _copy(currentNSMap, currentNSMap = /* @__PURE__ */ Object.create(null));
            }
            currentNSMap[nsPrefix] = localNSMap[nsPrefix] = value;
            a.uri = NAMESPACE.XMLNS;
            domBuilder.startPrefixMapping(nsPrefix, value);
          }
        }
        var i = el.length;
        while (i--) {
          a = el[i];
          if (a.prefix) {
            if (a.prefix === "xml") {
              a.uri = NAMESPACE.XML;
            }
            if (a.prefix !== "xmlns") {
              a.uri = currentNSMap[a.prefix];
            }
          }
        }
        var nsp = tagName.indexOf(":");
        if (nsp > 0) {
          prefix = el.prefix = tagName.slice(0, nsp);
          localName = el.localName = tagName.slice(nsp + 1);
        } else {
          prefix = null;
          localName = el.localName = tagName;
        }
        var ns = el.uri = currentNSMap[prefix || ""];
        domBuilder.startElement(ns, localName, tagName, el);
        if (el.closed) {
          domBuilder.endElement(ns, localName, tagName);
          if (localNSMap) {
            for (prefix in localNSMap) {
              if (hasOwn(localNSMap, prefix)) {
                domBuilder.endPrefixMapping(prefix);
              }
            }
          }
        } else {
          el.currentNSMap = currentNSMap;
          el.localNSMap = localNSMap;
          return true;
        }
      }
      function parseHtmlSpecialContent(source, elStartEnd, tagName, entityReplacer, domBuilder) {
        var isEscapableRaw = isHTMLEscapableRawTextElement(tagName);
        if (isEscapableRaw || isHTMLRawTextElement(tagName)) {
          var elEndStart = source.indexOf("</" + tagName + ">", elStartEnd);
          var text = source.substring(elStartEnd + 1, elEndStart);
          if (isEscapableRaw) {
            text = text.replace(ENTITY_REG, entityReplacer);
          }
          domBuilder.characters(text, 0, text.length);
          return elEndStart;
        }
        return elStartEnd + 1;
      }
      function _copy(source, target) {
        for (var n in source) {
          if (hasOwn(source, n)) {
            target[n] = source[n];
          }
        }
      }
      function parseUtils(source, start) {
        var index = start;
        function char(n) {
          n = n || 0;
          return source.charAt(index + n);
        }
        function skip(n) {
          n = n || 1;
          index += n;
        }
        function skipBlanks() {
          var blanks = 0;
          while (index < source.length) {
            var c = char();
            if (c !== " " && c !== "\n" && c !== "	" && c !== "\r") {
              return blanks;
            }
            blanks++;
            skip();
          }
          return -1;
        }
        function substringFromIndex() {
          return source.substring(index);
        }
        function substringStartsWith(text) {
          return source.substring(index, index + text.length) === text;
        }
        function substringStartsWithCaseInsensitive(text) {
          return source.substring(index, index + text.length).toUpperCase() === text.toUpperCase();
        }
        function getMatch(args) {
          var expr = g.reg("^", args);
          var match = expr.exec(substringFromIndex());
          if (match) {
            skip(match[0].length);
            return match[0];
          }
          return null;
        }
        return {
          char,
          getIndex: function() {
            return index;
          },
          getMatch,
          getSource: function() {
            return source;
          },
          skip,
          skipBlanks,
          substringFromIndex,
          substringStartsWith,
          substringStartsWithCaseInsensitive
        };
      }
      function parseDoctypeInternalSubset(p, errorHandler) {
        function parsePI(p2, errorHandler2) {
          var match = g.PI.exec(p2.substringFromIndex());
          if (!match) {
            return errorHandler2.fatalError("processing instruction is not well-formed at position " + p2.getIndex());
          }
          if (match[1].toLowerCase() === "xml") {
            return errorHandler2.fatalError(
              "xml declaration is only allowed at the start of the document, but found at position " + p2.getIndex()
            );
          }
          p2.skip(match[0].length);
          return match[0];
        }
        var source = p.getSource();
        if (p.char() === "[") {
          p.skip(1);
          var intSubsetStart = p.getIndex();
          while (p.getIndex() < source.length) {
            p.skipBlanks();
            if (p.char() === "]") {
              var internalSubset = source.substring(intSubsetStart, p.getIndex());
              p.skip(1);
              return internalSubset;
            }
            var current = null;
            if (p.char() === "<" && p.char(1) === "!") {
              switch (p.char(2)) {
                case "E":
                  if (p.char(3) === "L") {
                    current = p.getMatch(g.elementdecl);
                  } else if (p.char(3) === "N") {
                    current = p.getMatch(g.EntityDecl);
                  }
                  break;
                case "A":
                  current = p.getMatch(g.AttlistDecl);
                  break;
                case "N":
                  current = p.getMatch(g.NotationDecl);
                  break;
                case "-":
                  current = p.getMatch(g.Comment);
                  break;
              }
            } else if (p.char() === "<" && p.char(1) === "?") {
              current = parsePI(p, errorHandler);
            } else if (p.char() === "%") {
              current = p.getMatch(g.PEReference);
            } else {
              return errorHandler.fatalError("Error detected in Markup declaration");
            }
            if (!current) {
              return errorHandler.fatalError("Error in internal subset at position " + p.getIndex());
            }
          }
          return errorHandler.fatalError("doctype internal subset is not well-formed, missing ]");
        }
      }
      function parseDoctypeCommentOrCData(source, start, domBuilder, errorHandler, isHTML) {
        var p = parseUtils(source, start);
        switch (isHTML ? p.char(2).toUpperCase() : p.char(2)) {
          case "-":
            var comment = p.getMatch(g.Comment);
            if (comment) {
              domBuilder.comment(comment, g.COMMENT_START.length, comment.length - g.COMMENT_START.length - g.COMMENT_END.length);
              return p.getIndex();
            } else {
              return errorHandler.fatalError("comment is not well-formed at position " + p.getIndex());
            }
          case "[":
            var cdata = p.getMatch(g.CDSect);
            if (cdata) {
              if (!isHTML && !domBuilder.currentElement) {
                return errorHandler.fatalError("CDATA outside of element");
              }
              domBuilder.startCDATA();
              domBuilder.characters(cdata, g.CDATA_START.length, cdata.length - g.CDATA_START.length - g.CDATA_END.length);
              domBuilder.endCDATA();
              return p.getIndex();
            } else {
              return errorHandler.fatalError("Invalid CDATA starting at position " + start);
            }
          case "D": {
            if (domBuilder.doc && domBuilder.doc.documentElement) {
              return errorHandler.fatalError("Doctype not allowed inside or after documentElement at position " + p.getIndex());
            }
            if (isHTML ? !p.substringStartsWithCaseInsensitive(g.DOCTYPE_DECL_START) : !p.substringStartsWith(g.DOCTYPE_DECL_START)) {
              return errorHandler.fatalError("Expected " + g.DOCTYPE_DECL_START + " at position " + p.getIndex());
            }
            p.skip(g.DOCTYPE_DECL_START.length);
            if (p.skipBlanks() < 1) {
              return errorHandler.fatalError("Expected whitespace after " + g.DOCTYPE_DECL_START + " at position " + p.getIndex());
            }
            var doctype = {
              name: void 0,
              publicId: void 0,
              systemId: void 0,
              internalSubset: void 0
            };
            doctype.name = p.getMatch(g.Name);
            if (!doctype.name)
              return errorHandler.fatalError("doctype name missing or contains unexpected characters at position " + p.getIndex());
            if (isHTML && doctype.name.toLowerCase() !== "html") {
              errorHandler.warning("Unexpected DOCTYPE in HTML document at position " + p.getIndex());
            }
            p.skipBlanks();
            if (p.substringStartsWith(g.PUBLIC) || p.substringStartsWith(g.SYSTEM)) {
              var match = g.ExternalID_match.exec(p.substringFromIndex());
              if (!match) {
                return errorHandler.fatalError("doctype external id is not well-formed at position " + p.getIndex());
              }
              if (match.groups.SystemLiteralOnly !== void 0) {
                doctype.systemId = match.groups.SystemLiteralOnly;
              } else {
                doctype.systemId = match.groups.SystemLiteral;
                doctype.publicId = match.groups.PubidLiteral;
              }
              p.skip(match[0].length);
            } else if (isHTML && p.substringStartsWithCaseInsensitive(g.SYSTEM)) {
              p.skip(g.SYSTEM.length);
              if (p.skipBlanks() < 1) {
                return errorHandler.fatalError("Expected whitespace after " + g.SYSTEM + " at position " + p.getIndex());
              }
              doctype.systemId = p.getMatch(g.ABOUT_LEGACY_COMPAT_SystemLiteral);
              if (!doctype.systemId) {
                return errorHandler.fatalError(
                  "Expected " + g.ABOUT_LEGACY_COMPAT + " in single or double quotes after " + g.SYSTEM + " at position " + p.getIndex()
                );
              }
            }
            if (isHTML && doctype.systemId && !g.ABOUT_LEGACY_COMPAT_SystemLiteral.test(doctype.systemId)) {
              errorHandler.warning("Unexpected doctype.systemId in HTML document at position " + p.getIndex());
            }
            if (!isHTML) {
              p.skipBlanks();
              doctype.internalSubset = parseDoctypeInternalSubset(p, errorHandler);
            }
            p.skipBlanks();
            if (p.char() !== ">") {
              return errorHandler.fatalError("doctype not terminated with > at position " + p.getIndex());
            }
            p.skip(1);
            domBuilder.startDTD(doctype.name, doctype.publicId, doctype.systemId, doctype.internalSubset);
            domBuilder.endDTD();
            return p.getIndex();
          }
          default:
            return errorHandler.fatalError('Not well-formed XML starting with "<!" at position ' + start);
        }
      }
      function parseProcessingInstruction(source, start, domBuilder, errorHandler) {
        var match = source.substring(start).match(g.PI);
        if (!match) {
          return errorHandler.fatalError("Invalid processing instruction starting at position " + start);
        }
        if (match[1].toLowerCase() === "xml") {
          if (start > 0) {
            return errorHandler.fatalError(
              "processing instruction at position " + start + " is an xml declaration which is only at the start of the document"
            );
          }
          if (!g.XMLDecl.test(source.substring(start))) {
            return errorHandler.fatalError("xml declaration is not well-formed");
          }
        }
        domBuilder.processingInstruction(match[1], match[2]);
        return start + match[0].length;
      }
      function ElementAttributes() {
        this.attributeNames = /* @__PURE__ */ Object.create(null);
      }
      ElementAttributes.prototype = {
        setTagName: function(tagName) {
          if (!g.QName_exact.test(tagName)) {
            throw new Error("invalid tagName:" + tagName);
          }
          this.tagName = tagName;
        },
        addValue: function(qName, value, offset) {
          if (!g.QName_exact.test(qName)) {
            throw new Error("invalid attribute:" + qName);
          }
          this.attributeNames[qName] = this.length;
          this[this.length++] = { qName, value, offset };
        },
        length: 0,
        getLocalName: function(i) {
          return this[i].localName;
        },
        getLocator: function(i) {
          return this[i].locator;
        },
        getQName: function(i) {
          return this[i].qName;
        },
        getURI: function(i) {
          return this[i].uri;
        },
        getValue: function(i) {
          return this[i].value;
        }
        //	,getIndex:function(uri, localName)){
        //		if(localName){
        //
        //		}else{
        //			var qName = uri
        //		}
        //	},
        //	getValue:function(){return this.getValue(this.getIndex.apply(this,arguments))},
        //	getType:function(uri,localName){}
        //	getType:function(i){},
      };
      exports.XMLReader = XMLReader;
      exports.parseUtils = parseUtils;
      exports.parseDoctypeCommentOrCData = parseDoctypeCommentOrCData;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/dom-parser.js
  var require_dom_parser = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/dom-parser.js"(exports) {
      "use strict";
      var conventions = require_conventions();
      var dom = require_dom();
      var errors = require_errors();
      var entities = require_entities();
      var sax = require_sax();
      var DOMImplementation = dom.DOMImplementation;
      var hasDefaultHTMLNamespace = conventions.hasDefaultHTMLNamespace;
      var isHTMLMimeType = conventions.isHTMLMimeType;
      var isValidMimeType = conventions.isValidMimeType;
      var MIME_TYPE = conventions.MIME_TYPE;
      var NAMESPACE = conventions.NAMESPACE;
      var ParseError = errors.ParseError;
      var XMLReader = sax.XMLReader;
      function normalizeLineEndings(input) {
        return input.replace(/\r[\n\u0085]/g, "\n").replace(/[\r\u0085\u2028\u2029]/g, "\n");
      }
      function DOMParser2(options) {
        options = options || {};
        if (options.locator === void 0) {
          options.locator = true;
        }
        this.assign = options.assign || conventions.assign;
        this.domHandler = options.domHandler || DOMHandler;
        this.onError = options.onError || options.errorHandler;
        if (options.errorHandler && typeof options.errorHandler !== "function") {
          throw new TypeError("errorHandler object is no longer supported, switch to onError!");
        } else if (options.errorHandler) {
          options.errorHandler("warning", "The `errorHandler` option has been deprecated, use `onError` instead!", this);
        }
        this.normalizeLineEndings = options.normalizeLineEndings || normalizeLineEndings;
        this.locator = !!options.locator;
        this.xmlns = this.assign(/* @__PURE__ */ Object.create(null), options.xmlns);
      }
      DOMParser2.prototype.parseFromString = function(source, mimeType) {
        if (!isValidMimeType(mimeType)) {
          throw new TypeError('DOMParser.parseFromString: the provided mimeType "' + mimeType + '" is not valid.');
        }
        var defaultNSMap = this.assign(/* @__PURE__ */ Object.create(null), this.xmlns);
        var entityMap = entities.XML_ENTITIES;
        var defaultNamespace = defaultNSMap[""] || null;
        if (hasDefaultHTMLNamespace(mimeType)) {
          entityMap = entities.HTML_ENTITIES;
          defaultNamespace = NAMESPACE.HTML;
        } else if (mimeType === MIME_TYPE.XML_SVG_IMAGE) {
          defaultNamespace = NAMESPACE.SVG;
        }
        defaultNSMap[""] = defaultNamespace;
        defaultNSMap.xml = defaultNSMap.xml || NAMESPACE.XML;
        var domBuilder = new this.domHandler({
          mimeType,
          defaultNamespace,
          onError: this.onError
        });
        var locator = this.locator ? {} : void 0;
        if (this.locator) {
          domBuilder.setDocumentLocator(locator);
        }
        var sax2 = new XMLReader();
        sax2.errorHandler = domBuilder;
        sax2.domBuilder = domBuilder;
        var isXml = !conventions.isHTMLMimeType(mimeType);
        if (isXml && typeof source !== "string") {
          sax2.errorHandler.fatalError("source is not a string");
        }
        sax2.parse(this.normalizeLineEndings(String(source)), defaultNSMap, entityMap);
        if (!domBuilder.doc.documentElement) {
          sax2.errorHandler.fatalError("missing root element");
        }
        return domBuilder.doc;
      };
      function DOMHandler(options) {
        var opt = options || {};
        this.mimeType = opt.mimeType || MIME_TYPE.XML_APPLICATION;
        this.defaultNamespace = opt.defaultNamespace || null;
        this.cdata = false;
        this.currentElement = void 0;
        this.doc = void 0;
        this.locator = void 0;
        this.onError = opt.onError;
      }
      function position(locator, node) {
        node.lineNumber = locator.lineNumber;
        node.columnNumber = locator.columnNumber;
      }
      DOMHandler.prototype = {
        /**
         * Either creates an XML or an HTML document and stores it under `this.doc`.
         * If it is an XML document, `this.defaultNamespace` is used to create it,
         * and it will not contain any `childNodes`.
         * If it is an HTML document, it will be created without any `childNodes`.
         *
         * @see http://www.saxproject.org/apidoc/org/xml/sax/ContentHandler.html
         */
        startDocument: function() {
          var impl = new DOMImplementation();
          this.doc = isHTMLMimeType(this.mimeType) ? impl.createHTMLDocument(false) : impl.createDocument(this.defaultNamespace, "");
        },
        startElement: function(namespaceURI, localName, qName, attrs) {
          var doc = this.doc;
          var el = doc.createElementNS(namespaceURI, qName || localName);
          var len = attrs.length;
          appendElement(this, el);
          this.currentElement = el;
          this.locator && position(this.locator, el);
          for (var i = 0; i < len; i++) {
            var namespaceURI = attrs.getURI(i);
            var value = attrs.getValue(i);
            var qName = attrs.getQName(i);
            var attr = doc.createAttributeNS(namespaceURI, qName);
            this.locator && position(attrs.getLocator(i), attr);
            attr.value = attr.nodeValue = value;
            el.setAttributeNode(attr);
          }
        },
        endElement: function(namespaceURI, localName, qName) {
          this.currentElement = this.currentElement.parentNode;
        },
        startPrefixMapping: function(prefix, uri) {
        },
        endPrefixMapping: function(prefix) {
        },
        processingInstruction: function(target, data) {
          var ins = this.doc.createProcessingInstruction(target, data);
          this.locator && position(this.locator, ins);
          appendElement(this, ins);
        },
        ignorableWhitespace: function(ch, start, length) {
        },
        characters: function(chars, start, length) {
          chars = _toString.apply(this, arguments);
          if (chars) {
            if (this.cdata) {
              var charNode = this.doc.createCDATASection(chars);
            } else {
              var charNode = this.doc.createTextNode(chars);
            }
            if (this.currentElement) {
              this.currentElement.appendChild(charNode);
            } else if (/^\s*$/.test(chars)) {
              this.doc.appendChild(charNode);
            }
            this.locator && position(this.locator, charNode);
          }
        },
        skippedEntity: function(name2) {
        },
        endDocument: function() {
          this.doc.normalize();
        },
        /**
         * Stores the locator to be able to set the `columnNumber` and `lineNumber`
         * on the created DOM nodes.
         *
         * @param {Locator} locator
         */
        setDocumentLocator: function(locator) {
          if (locator) {
            locator.lineNumber = 0;
          }
          this.locator = locator;
        },
        //LexicalHandler
        comment: function(chars, start, length) {
          chars = _toString.apply(this, arguments);
          var comm = this.doc.createComment(chars);
          this.locator && position(this.locator, comm);
          appendElement(this, comm);
        },
        startCDATA: function() {
          this.cdata = true;
        },
        endCDATA: function() {
          this.cdata = false;
        },
        startDTD: function(name2, publicId, systemId, internalSubset) {
          var impl = this.doc.implementation;
          if (impl && impl.createDocumentType) {
            var dt = impl.createDocumentType(name2, publicId, systemId, internalSubset);
            this.locator && position(this.locator, dt);
            appendElement(this, dt);
            this.doc.doctype = dt;
          }
        },
        reportError: function(level, message) {
          if (typeof this.onError === "function") {
            try {
              this.onError(level, message, this);
            } catch (e) {
              throw new ParseError("Reporting " + level + ' "' + message + '" caused ' + e, this.locator);
            }
          } else {
            console.error("[xmldom " + level + "]	" + message, _locator(this.locator));
          }
        },
        /**
         * @see http://www.saxproject.org/apidoc/org/xml/sax/ErrorHandler.html
         */
        warning: function(message) {
          this.reportError("warning", message);
        },
        error: function(message) {
          this.reportError("error", message);
        },
        /**
         * This function reports a fatal error and throws a ParseError.
         *
         * @param {string} message
         * - The message to be used for reporting and throwing the error.
         * @returns {never}
         * This function always throws an error and never returns a value.
         * @throws {ParseError}
         * Always throws a ParseError with the provided message.
         */
        fatalError: function(message) {
          this.reportError("fatalError", message);
          throw new ParseError(message, this.locator);
        }
      };
      function _locator(l) {
        if (l) {
          return "\n@#[line:" + l.lineNumber + ",col:" + l.columnNumber + "]";
        }
      }
      function _toString(chars, start, length) {
        if (typeof chars == "string") {
          return chars.substr(start, length);
        } else {
          if (chars.length >= start + length || start) {
            return new java.lang.String(chars, start, length) + "";
          }
          return chars;
        }
      }
      "endDTD,startEntity,endEntity,attributeDecl,elementDecl,externalEntityDecl,internalEntityDecl,resolveEntity,getExternalSubset,notationDecl,unparsedEntityDecl".replace(
        /\w+/g,
        function(key) {
          DOMHandler.prototype[key] = function() {
            return null;
          };
        }
      );
      function appendElement(handler, node) {
        if (!handler.currentElement) {
          handler.doc.appendChild(node);
        } else {
          handler.currentElement.appendChild(node);
        }
      }
      function onErrorStopParsing(level) {
        if (level === "error") throw "onErrorStopParsing";
      }
      function onWarningStopParsing() {
        throw "onWarningStopParsing";
      }
      exports.__DOMHandler = DOMHandler;
      exports.DOMParser = DOMParser2;
      exports.normalizeLineEndings = normalizeLineEndings;
      exports.onErrorStopParsing = onErrorStopParsing;
      exports.onWarningStopParsing = onWarningStopParsing;
    }
  });

  // node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/index.js
  var require_lib = __commonJS({
    "node_modules/.pnpm/@xmldom+xmldom@0.9.8/node_modules/@xmldom/xmldom/lib/index.js"(exports) {
      "use strict";
      var conventions = require_conventions();
      exports.assign = conventions.assign;
      exports.hasDefaultHTMLNamespace = conventions.hasDefaultHTMLNamespace;
      exports.isHTMLMimeType = conventions.isHTMLMimeType;
      exports.isValidMimeType = conventions.isValidMimeType;
      exports.MIME_TYPE = conventions.MIME_TYPE;
      exports.NAMESPACE = conventions.NAMESPACE;
      var errors = require_errors();
      exports.DOMException = errors.DOMException;
      exports.DOMExceptionName = errors.DOMExceptionName;
      exports.ExceptionCode = errors.ExceptionCode;
      exports.ParseError = errors.ParseError;
      var dom = require_dom();
      exports.Attr = dom.Attr;
      exports.CDATASection = dom.CDATASection;
      exports.CharacterData = dom.CharacterData;
      exports.Comment = dom.Comment;
      exports.Document = dom.Document;
      exports.DocumentFragment = dom.DocumentFragment;
      exports.DocumentType = dom.DocumentType;
      exports.DOMImplementation = dom.DOMImplementation;
      exports.Element = dom.Element;
      exports.Entity = dom.Entity;
      exports.EntityReference = dom.EntityReference;
      exports.LiveNodeList = dom.LiveNodeList;
      exports.NamedNodeMap = dom.NamedNodeMap;
      exports.Node = dom.Node;
      exports.NodeList = dom.NodeList;
      exports.Notation = dom.Notation;
      exports.ProcessingInstruction = dom.ProcessingInstruction;
      exports.Text = dom.Text;
      exports.XMLSerializer = dom.XMLSerializer;
      var domParser = require_dom_parser();
      exports.DOMParser = domParser.DOMParser;
      exports.normalizeLineEndings = domParser.normalizeLineEndings;
      exports.onErrorStopParsing = domParser.onErrorStopParsing;
      exports.onWarningStopParsing = domParser.onWarningStopParsing;
    }
  });

  // node_modules/.pnpm/xpath@0.0.34/node_modules/xpath/xpath.js
  var require_xpath = __commonJS({
    "node_modules/.pnpm/xpath@0.0.34/node_modules/xpath/xpath.js"(exports) {
      "use strict";
      var xpath2 = typeof exports === "undefined" ? {} : exports;
      (function(exports2) {
        "use strict";
        var NAMESPACE_NODE_NODETYPE = "__namespace";
        var isNil = function(x) {
          return x === null || x === void 0;
        };
        var isValidNodeType = function(nodeType) {
          return nodeType === NAMESPACE_NODE_NODETYPE || Number.isInteger(nodeType) && nodeType >= 1 && nodeType <= 11;
        };
        var isNodeLike = function(value) {
          return value && isValidNodeType(value.nodeType) && typeof value.nodeName === "string";
        };
        function curry(func) {
          var slice = Array.prototype.slice, totalargs = func.length, partial = function(args, fn3) {
            return function() {
              return fn3.apply(this, args.concat(slice.call(arguments)));
            };
          }, fn2 = function() {
            var args = slice.call(arguments);
            return args.length < totalargs ? partial(args, fn2) : func.apply(this, slice.apply(arguments, [0, totalargs]));
          };
          return fn2;
        }
        var forEach = function(f, xs) {
          for (var i = 0; i < xs.length; i += 1) {
            f(xs[i], i, xs);
          }
        };
        var reduce = function(f, seed, xs) {
          var acc = seed;
          forEach(function(x, i) {
            acc = f(acc, x, i);
          }, xs);
          return acc;
        };
        var map = function(f, xs) {
          var mapped = new Array(xs.length);
          forEach(function(x, i) {
            mapped[i] = f(x);
          }, xs);
          return mapped;
        };
        var filter = function(f, xs) {
          var filtered = [];
          forEach(function(x, i) {
            if (f(x, i)) {
              filtered.push(x);
            }
          }, xs);
          return filtered;
        };
        var includes = function(values, value) {
          for (var i = 0; i < values.length; i += 1) {
            if (values[i] === value) {
              return true;
            }
          }
          return false;
        };
        function always(value) {
          return function() {
            return value;
          };
        }
        function toString(x) {
          return x.toString();
        }
        var join = function(s, xs) {
          return xs.join(s);
        };
        var wrap = function(pref, suf, str) {
          return pref + str + suf;
        };
        var prototypeConcat = Array.prototype.concat;
        var sortNodes = function(nodes, reverse) {
          var ns = new XNodeSet();
          ns.addArray(nodes);
          var sorted = ns.toArray();
          return reverse ? sorted.reverse() : sorted;
        };
        var MAX_ARGUMENT_LENGTH = 32767;
        function flatten(arr) {
          var result = [];
          for (var start = 0; start < arr.length; start += MAX_ARGUMENT_LENGTH) {
            var chunk = arr.slice(start, start + MAX_ARGUMENT_LENGTH);
            result = prototypeConcat.apply(result, chunk);
          }
          return result;
        }
        function assign(target, varArgs) {
          var to = Object(target);
          for (var index = 1; index < arguments.length; index++) {
            var nextSource = arguments[index];
            if (nextSource != null) {
              for (var nextKey in nextSource) {
                if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
                  to[nextKey] = nextSource[nextKey];
                }
              }
            }
          }
          return to;
        }
        var NodeTypes = {
          ELEMENT_NODE: 1,
          ATTRIBUTE_NODE: 2,
          TEXT_NODE: 3,
          CDATA_SECTION_NODE: 4,
          PROCESSING_INSTRUCTION_NODE: 7,
          COMMENT_NODE: 8,
          DOCUMENT_NODE: 9,
          DOCUMENT_TYPE_NODE: 10,
          DOCUMENT_FRAGMENT_NODE: 11,
          NAMESPACE_NODE: NAMESPACE_NODE_NODETYPE
        };
        XPathParser.prototype = new Object();
        XPathParser.prototype.constructor = XPathParser;
        XPathParser.superclass = Object.prototype;
        function XPathParser() {
          this.init();
        }
        XPathParser.prototype.init = function() {
          this.reduceActions = [];
          this.reduceActions[3] = function(rhs) {
            return new OrOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[5] = function(rhs) {
            return new AndOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[7] = function(rhs) {
            return new EqualsOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[8] = function(rhs) {
            return new NotEqualOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[10] = function(rhs) {
            return new LessThanOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[11] = function(rhs) {
            return new GreaterThanOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[12] = function(rhs) {
            return new LessThanOrEqualOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[13] = function(rhs) {
            return new GreaterThanOrEqualOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[15] = function(rhs) {
            return new PlusOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[16] = function(rhs) {
            return new MinusOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[18] = function(rhs) {
            return new MultiplyOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[19] = function(rhs) {
            return new DivOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[20] = function(rhs) {
            return new ModOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[22] = function(rhs) {
            return new UnaryMinusOperation(rhs[1]);
          };
          this.reduceActions[24] = function(rhs) {
            return new BarOperation(rhs[0], rhs[2]);
          };
          this.reduceActions[25] = function(rhs) {
            return new PathExpr(void 0, void 0, rhs[0]);
          };
          this.reduceActions[27] = function(rhs) {
            rhs[0].locationPath = rhs[2];
            return rhs[0];
          };
          this.reduceActions[28] = function(rhs) {
            rhs[0].locationPath = rhs[2];
            rhs[0].locationPath.steps.unshift(new Step(Step.DESCENDANTORSELF, NodeTest.nodeTest, []));
            return rhs[0];
          };
          this.reduceActions[29] = function(rhs) {
            return new PathExpr(rhs[0], [], void 0);
          };
          this.reduceActions[30] = function(rhs) {
            if (Utilities.instance_of(rhs[0], PathExpr)) {
              if (rhs[0].filterPredicates == void 0) {
                rhs[0].filterPredicates = [];
              }
              rhs[0].filterPredicates.push(rhs[1]);
              return rhs[0];
            } else {
              return new PathExpr(rhs[0], [rhs[1]], void 0);
            }
          };
          this.reduceActions[32] = function(rhs) {
            return rhs[1];
          };
          this.reduceActions[33] = function(rhs) {
            return new XString(rhs[0]);
          };
          this.reduceActions[34] = function(rhs) {
            return new XNumber(rhs[0]);
          };
          this.reduceActions[36] = function(rhs) {
            return new FunctionCall(rhs[0], []);
          };
          this.reduceActions[37] = function(rhs) {
            return new FunctionCall(rhs[0], rhs[2]);
          };
          this.reduceActions[38] = function(rhs) {
            return [rhs[0]];
          };
          this.reduceActions[39] = function(rhs) {
            rhs[2].unshift(rhs[0]);
            return rhs[2];
          };
          this.reduceActions[43] = function(rhs) {
            return new LocationPath(true, []);
          };
          this.reduceActions[44] = function(rhs) {
            rhs[1].absolute = true;
            return rhs[1];
          };
          this.reduceActions[46] = function(rhs) {
            return new LocationPath(false, [rhs[0]]);
          };
          this.reduceActions[47] = function(rhs) {
            rhs[0].steps.push(rhs[2]);
            return rhs[0];
          };
          this.reduceActions[49] = function(rhs) {
            return new Step(rhs[0], rhs[1], []);
          };
          this.reduceActions[50] = function(rhs) {
            return new Step(Step.CHILD, rhs[0], []);
          };
          this.reduceActions[51] = function(rhs) {
            return new Step(rhs[0], rhs[1], rhs[2]);
          };
          this.reduceActions[52] = function(rhs) {
            return new Step(Step.CHILD, rhs[0], rhs[1]);
          };
          this.reduceActions[54] = function(rhs) {
            return [rhs[0]];
          };
          this.reduceActions[55] = function(rhs) {
            rhs[1].unshift(rhs[0]);
            return rhs[1];
          };
          this.reduceActions[56] = function(rhs) {
            if (rhs[0] == "ancestor") {
              return Step.ANCESTOR;
            } else if (rhs[0] == "ancestor-or-self") {
              return Step.ANCESTORORSELF;
            } else if (rhs[0] == "attribute") {
              return Step.ATTRIBUTE;
            } else if (rhs[0] == "child") {
              return Step.CHILD;
            } else if (rhs[0] == "descendant") {
              return Step.DESCENDANT;
            } else if (rhs[0] == "descendant-or-self") {
              return Step.DESCENDANTORSELF;
            } else if (rhs[0] == "following") {
              return Step.FOLLOWING;
            } else if (rhs[0] == "following-sibling") {
              return Step.FOLLOWINGSIBLING;
            } else if (rhs[0] == "namespace") {
              return Step.NAMESPACE;
            } else if (rhs[0] == "parent") {
              return Step.PARENT;
            } else if (rhs[0] == "preceding") {
              return Step.PRECEDING;
            } else if (rhs[0] == "preceding-sibling") {
              return Step.PRECEDINGSIBLING;
            } else if (rhs[0] == "self") {
              return Step.SELF;
            }
            return -1;
          };
          this.reduceActions[57] = function(rhs) {
            return Step.ATTRIBUTE;
          };
          this.reduceActions[59] = function(rhs) {
            if (rhs[0] == "comment") {
              return NodeTest.commentTest;
            } else if (rhs[0] == "text") {
              return NodeTest.textTest;
            } else if (rhs[0] == "processing-instruction") {
              return NodeTest.anyPiTest;
            } else if (rhs[0] == "node") {
              return NodeTest.nodeTest;
            }
            return new NodeTest(-1, void 0);
          };
          this.reduceActions[60] = function(rhs) {
            return new NodeTest.PITest(rhs[2]);
          };
          this.reduceActions[61] = function(rhs) {
            return rhs[1];
          };
          this.reduceActions[63] = function(rhs) {
            rhs[1].absolute = true;
            rhs[1].steps.unshift(new Step(Step.DESCENDANTORSELF, NodeTest.nodeTest, []));
            return rhs[1];
          };
          this.reduceActions[64] = function(rhs) {
            rhs[0].steps.push(new Step(Step.DESCENDANTORSELF, NodeTest.nodeTest, []));
            rhs[0].steps.push(rhs[2]);
            return rhs[0];
          };
          this.reduceActions[65] = function(rhs) {
            return new Step(Step.SELF, NodeTest.nodeTest, []);
          };
          this.reduceActions[66] = function(rhs) {
            return new Step(Step.PARENT, NodeTest.nodeTest, []);
          };
          this.reduceActions[67] = function(rhs) {
            return new VariableReference(rhs[1]);
          };
          this.reduceActions[68] = function(rhs) {
            return NodeTest.nameTestAny;
          };
          this.reduceActions[69] = function(rhs) {
            return new NodeTest.NameTestPrefixAny(rhs[0].split(":")[0]);
          };
          this.reduceActions[70] = function(rhs) {
            return new NodeTest.NameTestQName(rhs[0]);
          };
        };
        XPathParser.actionTable = [
          " s s        sssssssss    s ss  s  ss",
          "                 s                  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "                rrrrr               ",
          " s s        sssssssss    s ss  s  ss",
          "rs  rrrrrrrr s  sssssrrrrrr  rrs rs ",
          " s s        sssssssss    s ss  s  ss",
          "                            s       ",
          "                            s       ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "  s                                 ",
          "                            s       ",
          " s           s  sssss          s  s ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "a                                   ",
          "r       s                    rr  r  ",
          "r      sr                    rr  r  ",
          "r   s  rr            s       rr  r  ",
          "r   rssrr            rss     rr  r  ",
          "r   rrrrr            rrrss   rr  r  ",
          "r   rrrrrsss         rrrrr   rr  r  ",
          "r   rrrrrrrr         rrrrr   rr  r  ",
          "r   rrrrrrrr         rrrrrs  rr  r  ",
          "r   rrrrrrrr         rrrrrr  rr  r  ",
          "r   rrrrrrrr         rrrrrr  rr  r  ",
          "r  srrrrrrrr         rrrrrrs rr sr  ",
          "r  srrrrrrrr         rrrrrrs rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r   rrrrrrrr         rrrrrr  rr  r  ",
          "r   rrrrrrrr         rrrrrr  rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "                sssss               ",
          "r  rrrrrrrrr         rrrrrrr rr sr  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "                             s      ",
          "r  srrrrrrrr         rrrrrrs rr  r  ",
          "r   rrrrrrrr         rrrrr   rr  r  ",
          "              s                     ",
          "                             s      ",
          "                rrrrr               ",
          " s s        sssssssss    s sss s  ss",
          "r  srrrrrrrr         rrrrrrs rr  r  ",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s s        sssssssss      ss  s  ss",
          " s s        sssssssss    s ss  s  ss",
          " s           s  sssss          s  s ",
          " s           s  sssss          s  s ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          " s           s  sssss          s  s ",
          " s           s  sssss          s  s ",
          "r  rrrrrrrrr         rrrrrrr rr sr  ",
          "r  rrrrrrrrr         rrrrrrr rr sr  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "                             s      ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "                             rr     ",
          "                             s      ",
          "                             rs     ",
          "r      sr                    rr  r  ",
          "r   s  rr            s       rr  r  ",
          "r   rssrr            rss     rr  r  ",
          "r   rssrr            rss     rr  r  ",
          "r   rrrrr            rrrss   rr  r  ",
          "r   rrrrr            rrrss   rr  r  ",
          "r   rrrrr            rrrss   rr  r  ",
          "r   rrrrr            rrrss   rr  r  ",
          "r   rrrrrsss         rrrrr   rr  r  ",
          "r   rrrrrsss         rrrrr   rr  r  ",
          "r   rrrrrrrr         rrrrr   rr  r  ",
          "r   rrrrrrrr         rrrrr   rr  r  ",
          "r   rrrrrrrr         rrrrr   rr  r  ",
          "r   rrrrrrrr         rrrrrr  rr  r  ",
          "                                 r  ",
          "                                 s  ",
          "r  srrrrrrrr         rrrrrrs rr  r  ",
          "r  srrrrrrrr         rrrrrrs rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr  r  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          " s s        sssssssss    s ss  s  ss",
          "r  rrrrrrrrr         rrrrrrr rr rr  ",
          "                             r      "
        ];
        XPathParser.actionTableNumber = [
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          "                 J                  ",
          "a  aaaaaaaaa         aaaaaaa aa  a  ",
          "                YYYYY               ",
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          `K1  KKKKKKKK .  +*)('KKKKKK  KK# K" `,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          "                            N       ",
          "                            O       ",
          "e  eeeeeeeee         eeeeeee ee ee  ",
          "f  fffffffff         fffffff ff ff  ",
          "d  ddddddddd         ddddddd dd dd  ",
          "B  BBBBBBBBB         BBBBBBB BB BB  ",
          "A  AAAAAAAAA         AAAAAAA AA AA  ",
          "  P                                 ",
          "                            Q       ",
          ` 1           .  +*)('          #  " `,
          "b  bbbbbbbbb         bbbbbbb bb  b  ",
          "                                    ",
          "!       S                    !!  !  ",
          '"      T"                    ""  "  ',
          "$   V  $$            U       $$  $  ",
          "&   &ZY&&            &XW     &&  &  ",
          ")   )))))            )))\\[   ))  )  ",
          ".   ....._^]         .....   ..  .  ",
          "1   11111111         11111   11  1  ",
          "5   55555555         55555`  55  5  ",
          "7   77777777         777777  77  7  ",
          "9   99999999         999999  99  9  ",
          ":  c::::::::         ::::::b :: a:  ",
          "I  fIIIIIIII         IIIIIIe II  I  ",
          "=  =========         ======= == ==  ",
          "?  ?????????         ??????? ?? ??  ",
          "C  CCCCCCCCC         CCCCCCC CC CC  ",
          "J   JJJJJJJJ         JJJJJJ  JJ  J  ",
          "M   MMMMMMMM         MMMMMM  MM  M  ",
          "N  NNNNNNNNN         NNNNNNN NN  N  ",
          "P  PPPPPPPPP         PPPPPPP PP  P  ",
          "                +*)('               ",
          "R  RRRRRRRRR         RRRRRRR RR aR  ",
          "U  UUUUUUUUU         UUUUUUU UU  U  ",
          "Z  ZZZZZZZZZ         ZZZZZZZ ZZ ZZ  ",
          "c  ccccccccc         ccccccc cc cc  ",
          "                             j      ",
          "L  fLLLLLLLL         LLLLLLe LL  L  ",
          "6   66666666         66666   66  6  ",
          "              k                     ",
          "                             l      ",
          "                XXXXX               ",
          ` 1 0        /.-,+*)('    & %$m #  "!`,
          "_  f________         ______e __  _  ",
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1 0        /.-,+*)('      %$  #  "!`,
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          ` 1           .  +*)('          #  " `,
          ` 1           .  +*)('          #  " `,
          ">  >>>>>>>>>         >>>>>>> >> >>  ",
          ` 1           .  +*)('          #  " `,
          ` 1           .  +*)('          #  " `,
          "Q  QQQQQQQQQ         QQQQQQQ QQ aQ  ",
          "V  VVVVVVVVV         VVVVVVV VV aV  ",
          "T  TTTTTTTTT         TTTTTTT TT  T  ",
          "@  @@@@@@@@@         @@@@@@@ @@ @@  ",
          "                             \x87      ",
          "[  [[[[[[[[[         [[[[[[[ [[ [[  ",
          "D  DDDDDDDDD         DDDDDDD DD DD  ",
          "                             HH     ",
          "                             \x88      ",
          "                             F\x89     ",
          "#      T#                    ##  #  ",
          "%   V  %%            U       %%  %  ",
          "'   'ZY''            'XW     ''  '  ",
          "(   (ZY((            (XW     ((  (  ",
          "+   +++++            +++\\[   ++  +  ",
          "*   *****            ***\\[   **  *  ",
          "-   -----            ---\\[   --  -  ",
          ",   ,,,,,            ,,,\\[   ,,  ,  ",
          "0   00000_^]         00000   00  0  ",
          "/   /////_^]         /////   //  /  ",
          "2   22222222         22222   22  2  ",
          "3   33333333         33333   33  3  ",
          "4   44444444         44444   44  4  ",
          "8   88888888         888888  88  8  ",
          "                                 ^  ",
          "                                 \x8A  ",
          ";  f;;;;;;;;         ;;;;;;e ;;  ;  ",
          "<  f<<<<<<<<         <<<<<<e <<  <  ",
          "O  OOOOOOOOO         OOOOOOO OO  O  ",
          "`  `````````         ``````` ``  `  ",
          "S  SSSSSSSSS         SSSSSSS SS  S  ",
          "W  WWWWWWWWW         WWWWWWW WW  W  ",
          "\\  \\\\\\\\\\\\\\\\\\         \\\\\\\\\\\\\\ \\\\ \\\\  ",
          "E  EEEEEEEEE         EEEEEEE EE EE  ",
          ` 1 0        /.-,+*)('    & %$  #  "!`,
          "]  ]]]]]]]]]         ]]]]]]] ]] ]]  ",
          "                             G      "
        ];
        XPathParser.gotoTable = [
          "3456789:;<=>?@ AB  CDEFGH IJ ",
          "                             ",
          "                             ",
          "                             ",
          "L456789:;<=>?@ AB  CDEFGH IJ ",
          "            M        EFGH IJ ",
          "       N;<=>?@ AB  CDEFGH IJ ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "            S        EFGH IJ ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "              e              ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                        h  J ",
          "              i          j   ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "o456789:;<=>?@ ABpqCDEFGH IJ ",
          "                             ",
          "  r6789:;<=>?@ AB  CDEFGH IJ ",
          "   s789:;<=>?@ AB  CDEFGH IJ ",
          "    t89:;<=>?@ AB  CDEFGH IJ ",
          "    u89:;<=>?@ AB  CDEFGH IJ ",
          "     v9:;<=>?@ AB  CDEFGH IJ ",
          "     w9:;<=>?@ AB  CDEFGH IJ ",
          "     x9:;<=>?@ AB  CDEFGH IJ ",
          "     y9:;<=>?@ AB  CDEFGH IJ ",
          "      z:;<=>?@ AB  CDEFGH IJ ",
          "      {:;<=>?@ AB  CDEFGH IJ ",
          "       |;<=>?@ AB  CDEFGH IJ ",
          "       };<=>?@ AB  CDEFGH IJ ",
          "       ~;<=>?@ AB  CDEFGH IJ ",
          "         \x7F=>?@ AB  CDEFGH IJ ",
          "\x80456789:;<=>?@ AB  CDEFGH IJ\x81",
          "            \x82        EFGH IJ ",
          "            \x83        EFGH IJ ",
          "                             ",
          "                     \x84 GH IJ ",
          "                     \x85 GH IJ ",
          "              i          \x86   ",
          "              i          \x87   ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "                             ",
          "o456789:;<=>?@ AB\x8CqCDEFGH IJ ",
          "                             ",
          "                             "
        ];
        XPathParser.productions = [
          [1, 1, 2],
          [2, 1, 3],
          [3, 1, 4],
          [3, 3, 3, -9, 4],
          [4, 1, 5],
          [4, 3, 4, -8, 5],
          [5, 1, 6],
          [5, 3, 5, -22, 6],
          [5, 3, 5, -5, 6],
          [6, 1, 7],
          [6, 3, 6, -23, 7],
          [6, 3, 6, -24, 7],
          [6, 3, 6, -6, 7],
          [6, 3, 6, -7, 7],
          [7, 1, 8],
          [7, 3, 7, -25, 8],
          [7, 3, 7, -26, 8],
          [8, 1, 9],
          [8, 3, 8, -12, 9],
          [8, 3, 8, -11, 9],
          [8, 3, 8, -10, 9],
          [9, 1, 10],
          [9, 2, -26, 9],
          [10, 1, 11],
          [10, 3, 10, -27, 11],
          [11, 1, 12],
          [11, 1, 13],
          [11, 3, 13, -28, 14],
          [11, 3, 13, -4, 14],
          [13, 1, 15],
          [13, 2, 13, 16],
          [15, 1, 17],
          [15, 3, -29, 2, -30],
          [15, 1, -15],
          [15, 1, -16],
          [15, 1, 18],
          [18, 3, -13, -29, -30],
          [18, 4, -13, -29, 19, -30],
          [19, 1, 20],
          [19, 3, 20, -31, 19],
          [20, 1, 2],
          [12, 1, 14],
          [12, 1, 21],
          [21, 1, -28],
          [21, 2, -28, 14],
          [21, 1, 22],
          [14, 1, 23],
          [14, 3, 14, -28, 23],
          [14, 1, 24],
          [23, 2, 25, 26],
          [23, 1, 26],
          [23, 3, 25, 26, 27],
          [23, 2, 26, 27],
          [23, 1, 28],
          [27, 1, 16],
          [27, 2, 16, 27],
          [25, 2, -14, -3],
          [25, 1, -32],
          [26, 1, 29],
          [26, 3, -20, -29, -30],
          [26, 4, -21, -29, -15, -30],
          [16, 3, -33, 30, -34],
          [30, 1, 2],
          [22, 2, -4, 14],
          [24, 3, 14, -4, 23],
          [28, 1, -35],
          [28, 1, -2],
          [17, 2, -36, -18],
          [29, 1, -17],
          [29, 1, -19],
          [29, 1, -18]
        ];
        XPathParser.DOUBLEDOT = 2;
        XPathParser.DOUBLECOLON = 3;
        XPathParser.DOUBLESLASH = 4;
        XPathParser.NOTEQUAL = 5;
        XPathParser.LESSTHANOREQUAL = 6;
        XPathParser.GREATERTHANOREQUAL = 7;
        XPathParser.AND = 8;
        XPathParser.OR = 9;
        XPathParser.MOD = 10;
        XPathParser.DIV = 11;
        XPathParser.MULTIPLYOPERATOR = 12;
        XPathParser.FUNCTIONNAME = 13;
        XPathParser.AXISNAME = 14;
        XPathParser.LITERAL = 15;
        XPathParser.NUMBER = 16;
        XPathParser.ASTERISKNAMETEST = 17;
        XPathParser.QNAME = 18;
        XPathParser.NCNAMECOLONASTERISK = 19;
        XPathParser.NODETYPE = 20;
        XPathParser.PROCESSINGINSTRUCTIONWITHLITERAL = 21;
        XPathParser.EQUALS = 22;
        XPathParser.LESSTHAN = 23;
        XPathParser.GREATERTHAN = 24;
        XPathParser.PLUS = 25;
        XPathParser.MINUS = 26;
        XPathParser.BAR = 27;
        XPathParser.SLASH = 28;
        XPathParser.LEFTPARENTHESIS = 29;
        XPathParser.RIGHTPARENTHESIS = 30;
        XPathParser.COMMA = 31;
        XPathParser.AT = 32;
        XPathParser.LEFTBRACKET = 33;
        XPathParser.RIGHTBRACKET = 34;
        XPathParser.DOT = 35;
        XPathParser.DOLLAR = 36;
        XPathParser.prototype.tokenize = function(s1) {
          var types = [];
          var values = [];
          var s = s1 + "\0";
          var pos = 0;
          var c = s.charAt(pos++);
          while (1) {
            while (c == " " || c == "	" || c == "\r" || c == "\n") {
              c = s.charAt(pos++);
            }
            if (c == "\0" || pos >= s.length) {
              break;
            }
            if (c == "(") {
              types.push(XPathParser.LEFTPARENTHESIS);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == ")") {
              types.push(XPathParser.RIGHTPARENTHESIS);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "[") {
              types.push(XPathParser.LEFTBRACKET);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "]") {
              types.push(XPathParser.RIGHTBRACKET);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "@") {
              types.push(XPathParser.AT);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == ",") {
              types.push(XPathParser.COMMA);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "|") {
              types.push(XPathParser.BAR);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "+") {
              types.push(XPathParser.PLUS);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "-") {
              types.push(XPathParser.MINUS);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "=") {
              types.push(XPathParser.EQUALS);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == "$") {
              types.push(XPathParser.DOLLAR);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == ".") {
              c = s.charAt(pos++);
              if (c == ".") {
                types.push(XPathParser.DOUBLEDOT);
                values.push("..");
                c = s.charAt(pos++);
                continue;
              }
              if (c >= "0" && c <= "9") {
                var number = "." + c;
                c = s.charAt(pos++);
                while (c >= "0" && c <= "9") {
                  number += c;
                  c = s.charAt(pos++);
                }
                types.push(XPathParser.NUMBER);
                values.push(number);
                continue;
              }
              types.push(XPathParser.DOT);
              values.push(".");
              continue;
            }
            if (c == "'" || c == '"') {
              var delimiter = c;
              var literal = "";
              while (pos < s.length && (c = s.charAt(pos)) !== delimiter) {
                literal += c;
                pos += 1;
              }
              if (c !== delimiter) {
                throw XPathException.fromMessage("Unterminated string literal: " + delimiter + literal);
              }
              pos += 1;
              types.push(XPathParser.LITERAL);
              values.push(literal);
              c = s.charAt(pos++);
              continue;
            }
            if (c >= "0" && c <= "9") {
              var number = c;
              c = s.charAt(pos++);
              while (c >= "0" && c <= "9") {
                number += c;
                c = s.charAt(pos++);
              }
              if (c == ".") {
                if (s.charAt(pos) >= "0" && s.charAt(pos) <= "9") {
                  number += c;
                  number += s.charAt(pos++);
                  c = s.charAt(pos++);
                  while (c >= "0" && c <= "9") {
                    number += c;
                    c = s.charAt(pos++);
                  }
                }
              }
              types.push(XPathParser.NUMBER);
              values.push(number);
              continue;
            }
            if (c == "*") {
              if (types.length > 0) {
                var last = types[types.length - 1];
                if (last != XPathParser.AT && last != XPathParser.DOUBLECOLON && last != XPathParser.LEFTPARENTHESIS && last != XPathParser.LEFTBRACKET && last != XPathParser.AND && last != XPathParser.OR && last != XPathParser.MOD && last != XPathParser.DIV && last != XPathParser.MULTIPLYOPERATOR && last != XPathParser.SLASH && last != XPathParser.DOUBLESLASH && last != XPathParser.BAR && last != XPathParser.PLUS && last != XPathParser.MINUS && last != XPathParser.EQUALS && last != XPathParser.NOTEQUAL && last != XPathParser.LESSTHAN && last != XPathParser.LESSTHANOREQUAL && last != XPathParser.GREATERTHAN && last != XPathParser.GREATERTHANOREQUAL) {
                  types.push(XPathParser.MULTIPLYOPERATOR);
                  values.push(c);
                  c = s.charAt(pos++);
                  continue;
                }
              }
              types.push(XPathParser.ASTERISKNAMETEST);
              values.push(c);
              c = s.charAt(pos++);
              continue;
            }
            if (c == ":") {
              if (s.charAt(pos) == ":") {
                types.push(XPathParser.DOUBLECOLON);
                values.push("::");
                pos++;
                c = s.charAt(pos++);
                continue;
              }
            }
            if (c == "/") {
              c = s.charAt(pos++);
              if (c == "/") {
                types.push(XPathParser.DOUBLESLASH);
                values.push("//");
                c = s.charAt(pos++);
                continue;
              }
              types.push(XPathParser.SLASH);
              values.push("/");
              continue;
            }
            if (c == "!") {
              if (s.charAt(pos) == "=") {
                types.push(XPathParser.NOTEQUAL);
                values.push("!=");
                pos++;
                c = s.charAt(pos++);
                continue;
              }
            }
            if (c == "<") {
              if (s.charAt(pos) == "=") {
                types.push(XPathParser.LESSTHANOREQUAL);
                values.push("<=");
                pos++;
                c = s.charAt(pos++);
                continue;
              }
              types.push(XPathParser.LESSTHAN);
              values.push("<");
              c = s.charAt(pos++);
              continue;
            }
            if (c == ">") {
              if (s.charAt(pos) == "=") {
                types.push(XPathParser.GREATERTHANOREQUAL);
                values.push(">=");
                pos++;
                c = s.charAt(pos++);
                continue;
              }
              types.push(XPathParser.GREATERTHAN);
              values.push(">");
              c = s.charAt(pos++);
              continue;
            }
            if (c == "_" || Utilities.isLetter(c.charCodeAt(0))) {
              var name2 = c;
              c = s.charAt(pos++);
              while (Utilities.isNCNameChar(c.charCodeAt(0))) {
                name2 += c;
                c = s.charAt(pos++);
              }
              if (types.length > 0) {
                var last = types[types.length - 1];
                if (last != XPathParser.AT && last != XPathParser.DOUBLECOLON && last != XPathParser.LEFTPARENTHESIS && last != XPathParser.LEFTBRACKET && last != XPathParser.AND && last != XPathParser.OR && last != XPathParser.MOD && last != XPathParser.DIV && last != XPathParser.MULTIPLYOPERATOR && last != XPathParser.SLASH && last != XPathParser.DOUBLESLASH && last != XPathParser.BAR && last != XPathParser.PLUS && last != XPathParser.MINUS && last != XPathParser.EQUALS && last != XPathParser.NOTEQUAL && last != XPathParser.LESSTHAN && last != XPathParser.LESSTHANOREQUAL && last != XPathParser.GREATERTHAN && last != XPathParser.GREATERTHANOREQUAL) {
                  if (name2 == "and") {
                    types.push(XPathParser.AND);
                    values.push(name2);
                    continue;
                  }
                  if (name2 == "or") {
                    types.push(XPathParser.OR);
                    values.push(name2);
                    continue;
                  }
                  if (name2 == "mod") {
                    types.push(XPathParser.MOD);
                    values.push(name2);
                    continue;
                  }
                  if (name2 == "div") {
                    types.push(XPathParser.DIV);
                    values.push(name2);
                    continue;
                  }
                }
              }
              if (c == ":") {
                if (s.charAt(pos) == "*") {
                  types.push(XPathParser.NCNAMECOLONASTERISK);
                  values.push(name2 + ":*");
                  pos++;
                  c = s.charAt(pos++);
                  continue;
                }
                if (s.charAt(pos) == "_" || Utilities.isLetter(s.charCodeAt(pos))) {
                  name2 += ":";
                  c = s.charAt(pos++);
                  while (Utilities.isNCNameChar(c.charCodeAt(0))) {
                    name2 += c;
                    c = s.charAt(pos++);
                  }
                  if (c == "(") {
                    types.push(XPathParser.FUNCTIONNAME);
                    values.push(name2);
                    continue;
                  }
                  types.push(XPathParser.QNAME);
                  values.push(name2);
                  continue;
                }
                if (s.charAt(pos) == ":") {
                  types.push(XPathParser.AXISNAME);
                  values.push(name2);
                  continue;
                }
              }
              if (c == "(") {
                if (name2 == "comment" || name2 == "text" || name2 == "node") {
                  types.push(XPathParser.NODETYPE);
                  values.push(name2);
                  continue;
                }
                if (name2 == "processing-instruction") {
                  if (s.charAt(pos) == ")") {
                    types.push(XPathParser.NODETYPE);
                  } else {
                    types.push(XPathParser.PROCESSINGINSTRUCTIONWITHLITERAL);
                  }
                  values.push(name2);
                  continue;
                }
                types.push(XPathParser.FUNCTIONNAME);
                values.push(name2);
                continue;
              }
              types.push(XPathParser.QNAME);
              values.push(name2);
              continue;
            }
            throw new Error("Unexpected character " + c);
          }
          types.push(1);
          values.push("[EOF]");
          return [types, values];
        };
        XPathParser.SHIFT = "s";
        XPathParser.REDUCE = "r";
        XPathParser.ACCEPT = "a";
        XPathParser.prototype.parse = function(s) {
          if (!s) {
            throw new Error("XPath expression unspecified.");
          }
          if (typeof s !== "string") {
            throw new Error("XPath expression must be a string.");
          }
          var types;
          var values;
          var res = this.tokenize(s);
          if (res == void 0) {
            return void 0;
          }
          types = res[0];
          values = res[1];
          var tokenPos = 0;
          var state = [];
          var tokenType = [];
          var tokenValue = [];
          var s;
          var a;
          var t;
          state.push(0);
          tokenType.push(1);
          tokenValue.push("_S");
          a = types[tokenPos];
          t = values[tokenPos++];
          while (1) {
            s = state[state.length - 1];
            switch (XPathParser.actionTable[s].charAt(a - 1)) {
              case XPathParser.SHIFT:
                tokenType.push(-a);
                tokenValue.push(t);
                state.push(XPathParser.actionTableNumber[s].charCodeAt(a - 1) - 32);
                a = types[tokenPos];
                t = values[tokenPos++];
                break;
              case XPathParser.REDUCE:
                var num = XPathParser.productions[XPathParser.actionTableNumber[s].charCodeAt(a - 1) - 32][1];
                var rhs = [];
                for (var i = 0; i < num; i++) {
                  tokenType.pop();
                  rhs.unshift(tokenValue.pop());
                  state.pop();
                }
                var s_ = state[state.length - 1];
                tokenType.push(XPathParser.productions[XPathParser.actionTableNumber[s].charCodeAt(a - 1) - 32][0]);
                if (this.reduceActions[XPathParser.actionTableNumber[s].charCodeAt(a - 1) - 32] == void 0) {
                  tokenValue.push(rhs[0]);
                } else {
                  tokenValue.push(this.reduceActions[XPathParser.actionTableNumber[s].charCodeAt(a - 1) - 32](rhs));
                }
                state.push(XPathParser.gotoTable[s_].charCodeAt(XPathParser.productions[XPathParser.actionTableNumber[s].charCodeAt(a - 1) - 32][0] - 2) - 33);
                break;
              case XPathParser.ACCEPT:
                return new XPath(tokenValue.pop());
              default:
                throw new Error("XPath parse error");
            }
          }
        };
        XPath.prototype = new Object();
        XPath.prototype.constructor = XPath;
        XPath.superclass = Object.prototype;
        function XPath(e) {
          this.expression = e;
        }
        XPath.prototype.toString = function() {
          return this.expression.toString();
        };
        function setIfUnset(obj, prop, value) {
          if (!(prop in obj)) {
            obj[prop] = value;
          }
        }
        XPath.prototype.evaluate = function(c) {
          var node = c.expressionContextNode;
          if (!(isNil(node) || isNodeLike(node))) {
            throw new Error("Context node does not appear to be a valid DOM node.");
          }
          c.contextNode = c.expressionContextNode;
          c.contextSize = 1;
          c.contextPosition = 1;
          if (c.isHtml) {
            setIfUnset(c, "caseInsensitive", true);
            setIfUnset(c, "allowAnyNamespaceForNoPrefix", true);
          }
          setIfUnset(c, "caseInsensitive", false);
          return this.expression.evaluate(c);
        };
        XPath.XML_NAMESPACE_URI = "http://www.w3.org/XML/1998/namespace";
        XPath.XMLNS_NAMESPACE_URI = "http://www.w3.org/2000/xmlns/";
        Expression.prototype = new Object();
        Expression.prototype.constructor = Expression;
        Expression.superclass = Object.prototype;
        function Expression() {
        }
        Expression.prototype.init = function() {
        };
        Expression.prototype.toString = function() {
          return "<Expression>";
        };
        Expression.prototype.evaluate = function(c) {
          throw new Error("Could not evaluate expression.");
        };
        UnaryOperation.prototype = new Expression();
        UnaryOperation.prototype.constructor = UnaryOperation;
        UnaryOperation.superclass = Expression.prototype;
        function UnaryOperation(rhs) {
          if (arguments.length > 0) {
            this.init(rhs);
          }
        }
        UnaryOperation.prototype.init = function(rhs) {
          this.rhs = rhs;
        };
        UnaryMinusOperation.prototype = new UnaryOperation();
        UnaryMinusOperation.prototype.constructor = UnaryMinusOperation;
        UnaryMinusOperation.superclass = UnaryOperation.prototype;
        function UnaryMinusOperation(rhs) {
          if (arguments.length > 0) {
            this.init(rhs);
          }
        }
        UnaryMinusOperation.prototype.init = function(rhs) {
          UnaryMinusOperation.superclass.init.call(this, rhs);
        };
        UnaryMinusOperation.prototype.evaluate = function(c) {
          return this.rhs.evaluate(c).number().negate();
        };
        UnaryMinusOperation.prototype.toString = function() {
          return "-" + this.rhs.toString();
        };
        BinaryOperation.prototype = new Expression();
        BinaryOperation.prototype.constructor = BinaryOperation;
        BinaryOperation.superclass = Expression.prototype;
        function BinaryOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        BinaryOperation.prototype.init = function(lhs, rhs) {
          this.lhs = lhs;
          this.rhs = rhs;
        };
        OrOperation.prototype = new BinaryOperation();
        OrOperation.prototype.constructor = OrOperation;
        OrOperation.superclass = BinaryOperation.prototype;
        function OrOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        OrOperation.prototype.init = function(lhs, rhs) {
          OrOperation.superclass.init.call(this, lhs, rhs);
        };
        OrOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " or " + this.rhs.toString() + ")";
        };
        OrOperation.prototype.evaluate = function(c) {
          var b = this.lhs.evaluate(c).bool();
          if (b.booleanValue()) {
            return b;
          }
          return this.rhs.evaluate(c).bool();
        };
        AndOperation.prototype = new BinaryOperation();
        AndOperation.prototype.constructor = AndOperation;
        AndOperation.superclass = BinaryOperation.prototype;
        function AndOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        AndOperation.prototype.init = function(lhs, rhs) {
          AndOperation.superclass.init.call(this, lhs, rhs);
        };
        AndOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " and " + this.rhs.toString() + ")";
        };
        AndOperation.prototype.evaluate = function(c) {
          var b = this.lhs.evaluate(c).bool();
          if (!b.booleanValue()) {
            return b;
          }
          return this.rhs.evaluate(c).bool();
        };
        EqualsOperation.prototype = new BinaryOperation();
        EqualsOperation.prototype.constructor = EqualsOperation;
        EqualsOperation.superclass = BinaryOperation.prototype;
        function EqualsOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        EqualsOperation.prototype.init = function(lhs, rhs) {
          EqualsOperation.superclass.init.call(this, lhs, rhs);
        };
        EqualsOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " = " + this.rhs.toString() + ")";
        };
        EqualsOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).equals(this.rhs.evaluate(c));
        };
        NotEqualOperation.prototype = new BinaryOperation();
        NotEqualOperation.prototype.constructor = NotEqualOperation;
        NotEqualOperation.superclass = BinaryOperation.prototype;
        function NotEqualOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        NotEqualOperation.prototype.init = function(lhs, rhs) {
          NotEqualOperation.superclass.init.call(this, lhs, rhs);
        };
        NotEqualOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " != " + this.rhs.toString() + ")";
        };
        NotEqualOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).notequal(this.rhs.evaluate(c));
        };
        LessThanOperation.prototype = new BinaryOperation();
        LessThanOperation.prototype.constructor = LessThanOperation;
        LessThanOperation.superclass = BinaryOperation.prototype;
        function LessThanOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        LessThanOperation.prototype.init = function(lhs, rhs) {
          LessThanOperation.superclass.init.call(this, lhs, rhs);
        };
        LessThanOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).lessthan(this.rhs.evaluate(c));
        };
        LessThanOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " < " + this.rhs.toString() + ")";
        };
        GreaterThanOperation.prototype = new BinaryOperation();
        GreaterThanOperation.prototype.constructor = GreaterThanOperation;
        GreaterThanOperation.superclass = BinaryOperation.prototype;
        function GreaterThanOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        GreaterThanOperation.prototype.init = function(lhs, rhs) {
          GreaterThanOperation.superclass.init.call(this, lhs, rhs);
        };
        GreaterThanOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).greaterthan(this.rhs.evaluate(c));
        };
        GreaterThanOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " > " + this.rhs.toString() + ")";
        };
        LessThanOrEqualOperation.prototype = new BinaryOperation();
        LessThanOrEqualOperation.prototype.constructor = LessThanOrEqualOperation;
        LessThanOrEqualOperation.superclass = BinaryOperation.prototype;
        function LessThanOrEqualOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        LessThanOrEqualOperation.prototype.init = function(lhs, rhs) {
          LessThanOrEqualOperation.superclass.init.call(this, lhs, rhs);
        };
        LessThanOrEqualOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).lessthanorequal(this.rhs.evaluate(c));
        };
        LessThanOrEqualOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " <= " + this.rhs.toString() + ")";
        };
        GreaterThanOrEqualOperation.prototype = new BinaryOperation();
        GreaterThanOrEqualOperation.prototype.constructor = GreaterThanOrEqualOperation;
        GreaterThanOrEqualOperation.superclass = BinaryOperation.prototype;
        function GreaterThanOrEqualOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        GreaterThanOrEqualOperation.prototype.init = function(lhs, rhs) {
          GreaterThanOrEqualOperation.superclass.init.call(this, lhs, rhs);
        };
        GreaterThanOrEqualOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).greaterthanorequal(this.rhs.evaluate(c));
        };
        GreaterThanOrEqualOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " >= " + this.rhs.toString() + ")";
        };
        PlusOperation.prototype = new BinaryOperation();
        PlusOperation.prototype.constructor = PlusOperation;
        PlusOperation.superclass = BinaryOperation.prototype;
        function PlusOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        PlusOperation.prototype.init = function(lhs, rhs) {
          PlusOperation.superclass.init.call(this, lhs, rhs);
        };
        PlusOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).number().plus(this.rhs.evaluate(c).number());
        };
        PlusOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " + " + this.rhs.toString() + ")";
        };
        MinusOperation.prototype = new BinaryOperation();
        MinusOperation.prototype.constructor = MinusOperation;
        MinusOperation.superclass = BinaryOperation.prototype;
        function MinusOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        MinusOperation.prototype.init = function(lhs, rhs) {
          MinusOperation.superclass.init.call(this, lhs, rhs);
        };
        MinusOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).number().minus(this.rhs.evaluate(c).number());
        };
        MinusOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " - " + this.rhs.toString() + ")";
        };
        MultiplyOperation.prototype = new BinaryOperation();
        MultiplyOperation.prototype.constructor = MultiplyOperation;
        MultiplyOperation.superclass = BinaryOperation.prototype;
        function MultiplyOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        MultiplyOperation.prototype.init = function(lhs, rhs) {
          MultiplyOperation.superclass.init.call(this, lhs, rhs);
        };
        MultiplyOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).number().multiply(this.rhs.evaluate(c).number());
        };
        MultiplyOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " * " + this.rhs.toString() + ")";
        };
        DivOperation.prototype = new BinaryOperation();
        DivOperation.prototype.constructor = DivOperation;
        DivOperation.superclass = BinaryOperation.prototype;
        function DivOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        DivOperation.prototype.init = function(lhs, rhs) {
          DivOperation.superclass.init.call(this, lhs, rhs);
        };
        DivOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).number().div(this.rhs.evaluate(c).number());
        };
        DivOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " div " + this.rhs.toString() + ")";
        };
        ModOperation.prototype = new BinaryOperation();
        ModOperation.prototype.constructor = ModOperation;
        ModOperation.superclass = BinaryOperation.prototype;
        function ModOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        ModOperation.prototype.init = function(lhs, rhs) {
          ModOperation.superclass.init.call(this, lhs, rhs);
        };
        ModOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).number().mod(this.rhs.evaluate(c).number());
        };
        ModOperation.prototype.toString = function() {
          return "(" + this.lhs.toString() + " mod " + this.rhs.toString() + ")";
        };
        BarOperation.prototype = new BinaryOperation();
        BarOperation.prototype.constructor = BarOperation;
        BarOperation.superclass = BinaryOperation.prototype;
        function BarOperation(lhs, rhs) {
          if (arguments.length > 0) {
            this.init(lhs, rhs);
          }
        }
        BarOperation.prototype.init = function(lhs, rhs) {
          BarOperation.superclass.init.call(this, lhs, rhs);
        };
        BarOperation.prototype.evaluate = function(c) {
          return this.lhs.evaluate(c).nodeset().union(this.rhs.evaluate(c).nodeset());
        };
        BarOperation.prototype.toString = function() {
          return map(toString, [this.lhs, this.rhs]).join(" | ");
        };
        PathExpr.prototype = new Expression();
        PathExpr.prototype.constructor = PathExpr;
        PathExpr.superclass = Expression.prototype;
        function PathExpr(filter2, filterPreds, locpath) {
          if (arguments.length > 0) {
            this.init(filter2, filterPreds, locpath);
          }
        }
        PathExpr.prototype.init = function(filter2, filterPreds, locpath) {
          PathExpr.superclass.init.call(this);
          this.filter = filter2;
          this.filterPredicates = filterPreds;
          this.locationPath = locpath;
        };
        function findRoot(node) {
          while (node && node.parentNode) {
            node = node.parentNode;
          }
          return node;
        }
        var applyPredicates = function(predicates, c, nodes, reverse) {
          if (predicates.length === 0) {
            return nodes;
          }
          var ctx = c.extend({});
          return reduce(
            function(inNodes, pred) {
              ctx.contextSize = inNodes.length;
              return filter(
                function(node, i) {
                  ctx.contextNode = node;
                  ctx.contextPosition = i + 1;
                  return PathExpr.predicateMatches(pred, ctx);
                },
                inNodes
              );
            },
            sortNodes(nodes, reverse),
            predicates
          );
        };
        PathExpr.getRoot = function(xpc, nodes) {
          var firstNode = nodes[0];
          if (firstNode && firstNode.nodeType === NodeTypes.DOCUMENT_NODE) {
            return firstNode;
          }
          if (xpc.virtualRoot) {
            return xpc.virtualRoot;
          }
          if (!firstNode) {
            throw new Error("Context node not found when determining document root.");
          }
          var ownerDoc = firstNode.ownerDocument;
          if (ownerDoc) {
            return ownerDoc;
          }
          var n = firstNode;
          while (n.parentNode != null) {
            n = n.parentNode;
          }
          return n;
        };
        var getPrefixForNamespaceNode = function(attrNode) {
          var nm = String(attrNode.name);
          if (nm === "xmlns") {
            return "";
          }
          if (nm.substring(0, 6) === "xmlns:") {
            return nm.substring(6, nm.length);
          }
          return null;
        };
        PathExpr.applyStep = function(step, xpc, node) {
          if (!node) {
            throw new Error("Context node not found when evaluating XPath step: " + step);
          }
          var newNodes = [];
          xpc.contextNode = node;
          switch (step.axis) {
            case Step.ANCESTOR:
              if (xpc.contextNode === xpc.virtualRoot) {
                break;
              }
              var m;
              if (xpc.contextNode.nodeType == NodeTypes.ATTRIBUTE_NODE) {
                m = PathExpr.getOwnerElement(xpc.contextNode);
              } else {
                m = xpc.contextNode.parentNode;
              }
              while (m != null) {
                if (step.nodeTest.matches(m, xpc)) {
                  newNodes.push(m);
                }
                if (m === xpc.virtualRoot) {
                  break;
                }
                m = m.parentNode;
              }
              break;
            case Step.ANCESTORORSELF:
              for (var m = xpc.contextNode; m != null; m = m.nodeType == NodeTypes.ATTRIBUTE_NODE ? PathExpr.getOwnerElement(m) : m.parentNode) {
                if (step.nodeTest.matches(m, xpc)) {
                  newNodes.push(m);
                }
                if (m === xpc.virtualRoot) {
                  break;
                }
              }
              break;
            case Step.ATTRIBUTE:
              var nnm = xpc.contextNode.attributes;
              if (nnm != null) {
                for (var k = 0; k < nnm.length; k++) {
                  var m = nnm.item(k);
                  if (step.nodeTest.matches(m, xpc)) {
                    newNodes.push(m);
                  }
                }
              }
              break;
            case Step.CHILD:
              for (var m = xpc.contextNode.firstChild; m != null; m = m.nextSibling) {
                if (step.nodeTest.matches(m, xpc)) {
                  newNodes.push(m);
                }
              }
              break;
            case Step.DESCENDANT:
              var st = [xpc.contextNode.firstChild];
              while (st.length > 0) {
                for (var m = st.pop(); m != null; ) {
                  if (step.nodeTest.matches(m, xpc)) {
                    newNodes.push(m);
                  }
                  if (m.firstChild != null) {
                    st.push(m.nextSibling);
                    m = m.firstChild;
                  } else {
                    m = m.nextSibling;
                  }
                }
              }
              break;
            case Step.DESCENDANTORSELF:
              if (step.nodeTest.matches(xpc.contextNode, xpc)) {
                newNodes.push(xpc.contextNode);
              }
              var st = [xpc.contextNode.firstChild];
              while (st.length > 0) {
                for (var m = st.pop(); m != null; ) {
                  if (step.nodeTest.matches(m, xpc)) {
                    newNodes.push(m);
                  }
                  if (m.firstChild != null) {
                    st.push(m.nextSibling);
                    m = m.firstChild;
                  } else {
                    m = m.nextSibling;
                  }
                }
              }
              break;
            case Step.FOLLOWING:
              if (xpc.contextNode === xpc.virtualRoot) {
                break;
              }
              var st = [];
              if (xpc.contextNode.firstChild != null) {
                st.unshift(xpc.contextNode.firstChild);
              } else {
                st.unshift(xpc.contextNode.nextSibling);
              }
              for (var m = xpc.contextNode.parentNode; m != null && m.nodeType != NodeTypes.DOCUMENT_NODE && m !== xpc.virtualRoot; m = m.parentNode) {
                st.unshift(m.nextSibling);
              }
              do {
                for (var m = st.pop(); m != null; ) {
                  if (step.nodeTest.matches(m, xpc)) {
                    newNodes.push(m);
                  }
                  if (m.firstChild != null) {
                    st.push(m.nextSibling);
                    m = m.firstChild;
                  } else {
                    m = m.nextSibling;
                  }
                }
              } while (st.length > 0);
              break;
            case Step.FOLLOWINGSIBLING:
              if (xpc.contextNode === xpc.virtualRoot) {
                break;
              }
              for (var m = xpc.contextNode.nextSibling; m != null; m = m.nextSibling) {
                if (step.nodeTest.matches(m, xpc)) {
                  newNodes.push(m);
                }
              }
              break;
            case Step.NAMESPACE:
              var nodes = {};
              if (xpc.contextNode.nodeType == NodeTypes.ELEMENT_NODE) {
                nodes["xml"] = new XPathNamespace("xml", null, XPath.XML_NAMESPACE_URI, xpc.contextNode);
                for (var m = xpc.contextNode; m != null && m.nodeType == NodeTypes.ELEMENT_NODE; m = m.parentNode) {
                  for (var k = 0; k < m.attributes.length; k++) {
                    var attr = m.attributes.item(k);
                    var pre = getPrefixForNamespaceNode(attr);
                    if (pre != null && nodes[pre] == void 0) {
                      nodes[pre] = new XPathNamespace(pre, attr, attr.value, xpc.contextNode);
                    }
                  }
                }
                for (var pre in nodes) {
                  var node = nodes[pre];
                  if (step.nodeTest.matches(node, xpc)) {
                    newNodes.push(node);
                  }
                }
              }
              break;
            case Step.PARENT:
              m = null;
              if (xpc.contextNode !== xpc.virtualRoot) {
                if (xpc.contextNode.nodeType == NodeTypes.ATTRIBUTE_NODE) {
                  m = PathExpr.getOwnerElement(xpc.contextNode);
                } else {
                  m = xpc.contextNode.parentNode;
                }
              }
              if (m != null && step.nodeTest.matches(m, xpc)) {
                newNodes.push(m);
              }
              break;
            case Step.PRECEDING:
              var st;
              if (xpc.virtualRoot != null) {
                st = [xpc.virtualRoot];
              } else {
                st = [findRoot(xpc.contextNode)];
              }
              outer: while (st.length > 0) {
                for (var m = st.pop(); m != null; ) {
                  if (m == xpc.contextNode) {
                    break outer;
                  }
                  if (step.nodeTest.matches(m, xpc)) {
                    newNodes.unshift(m);
                  }
                  if (m.firstChild != null) {
                    st.push(m.nextSibling);
                    m = m.firstChild;
                  } else {
                    m = m.nextSibling;
                  }
                }
              }
              break;
            case Step.PRECEDINGSIBLING:
              if (xpc.contextNode === xpc.virtualRoot) {
                break;
              }
              for (var m = xpc.contextNode.previousSibling; m != null; m = m.previousSibling) {
                if (step.nodeTest.matches(m, xpc)) {
                  newNodes.push(m);
                }
              }
              break;
            case Step.SELF:
              if (step.nodeTest.matches(xpc.contextNode, xpc)) {
                newNodes.push(xpc.contextNode);
              }
              break;
            default:
          }
          return newNodes;
        };
        function applyStepWithPredicates(step, xpc, node) {
          return applyPredicates(
            step.predicates,
            xpc,
            PathExpr.applyStep(step, xpc, node),
            includes(REVERSE_AXES, step.axis)
          );
        }
        function applyStepToNodes(context, nodes, step) {
          return flatten(
            map(
              applyStepWithPredicates.bind(null, step, context),
              nodes
            )
          );
        }
        PathExpr.applySteps = function(steps, xpc, nodes) {
          return reduce(
            applyStepToNodes.bind(null, xpc),
            nodes,
            steps
          );
        };
        PathExpr.prototype.applyFilter = function(c, xpc) {
          if (!this.filter) {
            return { nodes: [c.contextNode] };
          }
          var ns = this.filter.evaluate(c);
          if (!Utilities.instance_of(ns, XNodeSet)) {
            if (this.filterPredicates != null && this.filterPredicates.length > 0 || this.locationPath != null) {
              throw new Error("Path expression filter must evaluate to a nodeset if predicates or location path are used");
            }
            return { nonNodes: ns };
          }
          return {
            nodes: applyPredicates(
              this.filterPredicates || [],
              xpc,
              ns.toUnsortedArray(),
              false
              // reverse
            )
          };
        };
        PathExpr.applyLocationPath = function(locationPath, xpc, nodes) {
          if (!locationPath) {
            return nodes;
          }
          var startNodes = locationPath.absolute ? [PathExpr.getRoot(xpc, nodes)] : nodes;
          return PathExpr.applySteps(locationPath.steps, xpc, startNodes);
        };
        PathExpr.prototype.evaluate = function(c) {
          var xpc = assign(new XPathContext(), c);
          var filterResult = this.applyFilter(c, xpc);
          if ("nonNodes" in filterResult) {
            return filterResult.nonNodes;
          }
          var ns = new XNodeSet();
          ns.addArray(PathExpr.applyLocationPath(this.locationPath, xpc, filterResult.nodes));
          return ns;
        };
        PathExpr.predicateMatches = function(pred, c) {
          var res = pred.evaluate(c);
          return Utilities.instance_of(res, XNumber) ? c.contextPosition === res.numberValue() : res.booleanValue();
        };
        PathExpr.predicateString = function(predicate) {
          return wrap("[", "]", predicate.toString());
        };
        PathExpr.predicatesString = function(predicates) {
          return join(
            "",
            map(PathExpr.predicateString, predicates)
          );
        };
        PathExpr.prototype.toString = function() {
          if (this.filter != void 0) {
            var filterStr = toString(this.filter);
            if (Utilities.instance_of(this.filter, XString)) {
              return wrap("'", "'", filterStr);
            }
            if (this.filterPredicates != void 0 && this.filterPredicates.length) {
              return wrap("(", ")", filterStr) + PathExpr.predicatesString(this.filterPredicates);
            }
            if (this.locationPath != void 0) {
              return filterStr + (this.locationPath.absolute ? "" : "/") + toString(this.locationPath);
            }
            return filterStr;
          }
          return toString(this.locationPath);
        };
        PathExpr.getOwnerElement = function(n) {
          if (n.ownerElement) {
            return n.ownerElement;
          }
          try {
            if (n.selectSingleNode) {
              return n.selectSingleNode("..");
            }
          } catch (e) {
          }
          var doc = n.nodeType == NodeTypes.DOCUMENT_NODE ? n : n.ownerDocument;
          var elts = doc.getElementsByTagName("*");
          for (var i = 0; i < elts.length; i++) {
            var elt = elts.item(i);
            var nnm = elt.attributes;
            for (var j = 0; j < nnm.length; j++) {
              var an = nnm.item(j);
              if (an === n) {
                return elt;
              }
            }
          }
          return null;
        };
        LocationPath.prototype = new Object();
        LocationPath.prototype.constructor = LocationPath;
        LocationPath.superclass = Object.prototype;
        function LocationPath(abs, steps) {
          if (arguments.length > 0) {
            this.init(abs, steps);
          }
        }
        LocationPath.prototype.init = function(abs, steps) {
          this.absolute = abs;
          this.steps = steps;
        };
        LocationPath.prototype.toString = function() {
          return (this.absolute ? "/" : "") + map(toString, this.steps).join("/");
        };
        Step.prototype = new Object();
        Step.prototype.constructor = Step;
        Step.superclass = Object.prototype;
        function Step(axis, nodetest, preds) {
          if (arguments.length > 0) {
            this.init(axis, nodetest, preds);
          }
        }
        Step.prototype.init = function(axis, nodetest, preds) {
          this.axis = axis;
          this.nodeTest = nodetest;
          this.predicates = preds;
        };
        Step.prototype.toString = function() {
          return Step.STEPNAMES[this.axis] + "::" + this.nodeTest.toString() + PathExpr.predicatesString(this.predicates);
        };
        Step.ANCESTOR = 0;
        Step.ANCESTORORSELF = 1;
        Step.ATTRIBUTE = 2;
        Step.CHILD = 3;
        Step.DESCENDANT = 4;
        Step.DESCENDANTORSELF = 5;
        Step.FOLLOWING = 6;
        Step.FOLLOWINGSIBLING = 7;
        Step.NAMESPACE = 8;
        Step.PARENT = 9;
        Step.PRECEDING = 10;
        Step.PRECEDINGSIBLING = 11;
        Step.SELF = 12;
        Step.STEPNAMES = reduce(function(acc, x) {
          return acc[x[0]] = x[1], acc;
        }, {}, [
          [Step.ANCESTOR, "ancestor"],
          [Step.ANCESTORORSELF, "ancestor-or-self"],
          [Step.ATTRIBUTE, "attribute"],
          [Step.CHILD, "child"],
          [Step.DESCENDANT, "descendant"],
          [Step.DESCENDANTORSELF, "descendant-or-self"],
          [Step.FOLLOWING, "following"],
          [Step.FOLLOWINGSIBLING, "following-sibling"],
          [Step.NAMESPACE, "namespace"],
          [Step.PARENT, "parent"],
          [Step.PRECEDING, "preceding"],
          [Step.PRECEDINGSIBLING, "preceding-sibling"],
          [Step.SELF, "self"]
        ]);
        var REVERSE_AXES = [
          Step.ANCESTOR,
          Step.ANCESTORORSELF,
          Step.PARENT,
          Step.PRECEDING,
          Step.PRECEDINGSIBLING
        ];
        NodeTest.prototype = new Object();
        NodeTest.prototype.constructor = NodeTest;
        NodeTest.superclass = Object.prototype;
        function NodeTest(type, value) {
          if (arguments.length > 0) {
            this.init(type, value);
          }
        }
        NodeTest.prototype.init = function(type, value) {
          this.type = type;
          this.value = value;
        };
        NodeTest.prototype.toString = function() {
          return "<unknown nodetest type>";
        };
        NodeTest.prototype.matches = function(n, xpc) {
          console.warn("unknown node test type");
        };
        NodeTest.NAMETESTANY = 0;
        NodeTest.NAMETESTPREFIXANY = 1;
        NodeTest.NAMETESTQNAME = 2;
        NodeTest.COMMENT = 3;
        NodeTest.TEXT = 4;
        NodeTest.PI = 5;
        NodeTest.NODE = 6;
        NodeTest.isNodeType = function(types) {
          return function(node) {
            return includes(types, node.nodeType);
          };
        };
        NodeTest.makeNodeTestType = function(type, members, ctor) {
          var newType = ctor || function() {
          };
          newType.prototype = new NodeTest(type);
          newType.prototype.constructor = newType;
          assign(newType.prototype, members);
          return newType;
        };
        NodeTest.makeNodeTypeTest = function(type, nodeTypes, stringVal) {
          return new (NodeTest.makeNodeTestType(type, {
            matches: NodeTest.isNodeType(nodeTypes),
            toString: always(stringVal)
          }))();
        };
        NodeTest.hasPrefix = function(node) {
          return node.prefix || (node.nodeName || node.tagName).indexOf(":") !== -1;
        };
        NodeTest.isElementOrAttribute = NodeTest.isNodeType([1, 2]);
        NodeTest.nameSpaceMatches = function(prefix, xpc, n) {
          var nNamespace = n.namespaceURI || "";
          if (!prefix) {
            return !nNamespace || xpc.allowAnyNamespaceForNoPrefix && !NodeTest.hasPrefix(n);
          }
          var ns = xpc.namespaceResolver.getNamespace(prefix, xpc.expressionContextNode);
          if (ns == null) {
            throw new Error("Cannot resolve QName " + prefix);
          }
          return ns === nNamespace;
        };
        NodeTest.localNameMatches = function(localName, xpc, n) {
          var nLocalName = n.localName || n.nodeName;
          return xpc.caseInsensitive ? localName.toLowerCase() === nLocalName.toLowerCase() : localName === nLocalName;
        };
        NodeTest.NameTestPrefixAny = NodeTest.makeNodeTestType(
          NodeTest.NAMETESTPREFIXANY,
          {
            matches: function(n, xpc) {
              return NodeTest.isElementOrAttribute(n) && NodeTest.nameSpaceMatches(this.prefix, xpc, n);
            },
            toString: function() {
              return this.prefix + ":*";
            }
          },
          function NameTestPrefixAny(prefix) {
            this.prefix = prefix;
          }
        );
        NodeTest.NameTestQName = NodeTest.makeNodeTestType(
          NodeTest.NAMETESTQNAME,
          {
            matches: function(n, xpc) {
              return NodeTest.isNodeType(
                [
                  NodeTypes.ELEMENT_NODE,
                  NodeTypes.ATTRIBUTE_NODE,
                  NodeTypes.NAMESPACE_NODE
                ]
              )(n) && NodeTest.nameSpaceMatches(this.prefix, xpc, n) && NodeTest.localNameMatches(this.localName, xpc, n);
            },
            toString: function() {
              return this.name;
            }
          },
          function NameTestQName(name2) {
            var nameParts = name2.split(":");
            this.name = name2;
            this.prefix = nameParts.length > 1 ? nameParts[0] : null;
            this.localName = nameParts[nameParts.length > 1 ? 1 : 0];
          }
        );
        NodeTest.PITest = NodeTest.makeNodeTestType(NodeTest.PI, {
          matches: function(n, xpc) {
            return NodeTest.isNodeType(
              [NodeTypes.PROCESSING_INSTRUCTION_NODE]
            )(n) && (n.target || n.nodeName) === this.name;
          },
          toString: function() {
            return wrap('processing-instruction("', '")', this.name);
          }
        }, function(name2) {
          this.name = name2;
        });
        NodeTest.nameTestAny = NodeTest.makeNodeTypeTest(
          NodeTest.NAMETESTANY,
          [
            NodeTypes.ELEMENT_NODE,
            NodeTypes.ATTRIBUTE_NODE,
            NodeTypes.NAMESPACE_NODE
          ],
          "*"
        );
        NodeTest.textTest = NodeTest.makeNodeTypeTest(
          NodeTest.TEXT,
          [
            NodeTypes.TEXT_NODE,
            NodeTypes.CDATA_SECTION_NODE
          ],
          "text()"
        );
        NodeTest.commentTest = NodeTest.makeNodeTypeTest(
          NodeTest.COMMENT,
          [NodeTypes.COMMENT_NODE],
          "comment()"
        );
        NodeTest.nodeTest = NodeTest.makeNodeTypeTest(
          NodeTest.NODE,
          [
            NodeTypes.ELEMENT_NODE,
            NodeTypes.ATTRIBUTE_NODE,
            NodeTypes.TEXT_NODE,
            NodeTypes.CDATA_SECTION_NODE,
            NodeTypes.PROCESSING_INSTRUCTION_NODE,
            NodeTypes.COMMENT_NODE,
            NodeTypes.DOCUMENT_NODE
          ],
          "node()"
        );
        NodeTest.anyPiTest = NodeTest.makeNodeTypeTest(
          NodeTest.PI,
          [NodeTypes.PROCESSING_INSTRUCTION_NODE],
          "processing-instruction()"
        );
        VariableReference.prototype = new Expression();
        VariableReference.prototype.constructor = VariableReference;
        VariableReference.superclass = Expression.prototype;
        function VariableReference(v) {
          if (arguments.length > 0) {
            this.init(v);
          }
        }
        VariableReference.prototype.init = function(v) {
          this.variable = v;
        };
        VariableReference.prototype.toString = function() {
          return "$" + this.variable;
        };
        VariableReference.prototype.evaluate = function(c) {
          var parts = Utilities.resolveQName(this.variable, c.namespaceResolver, c.contextNode, false);
          if (parts[0] == null) {
            throw new Error("Cannot resolve QName " + fn);
          }
          var result = c.variableResolver.getVariable(parts[1], parts[0]);
          if (!result) {
            throw XPathException.fromMessage("Undeclared variable: " + this.toString());
          }
          return result;
        };
        FunctionCall.prototype = new Expression();
        FunctionCall.prototype.constructor = FunctionCall;
        FunctionCall.superclass = Expression.prototype;
        function FunctionCall(fn2, args) {
          if (arguments.length > 0) {
            this.init(fn2, args);
          }
        }
        FunctionCall.prototype.init = function(fn2, args) {
          this.functionName = fn2;
          this.arguments = args;
        };
        FunctionCall.prototype.toString = function() {
          var s = this.functionName + "(";
          for (var i = 0; i < this.arguments.length; i++) {
            if (i > 0) {
              s += ", ";
            }
            s += this.arguments[i].toString();
          }
          return s + ")";
        };
        FunctionCall.prototype.evaluate = function(c) {
          var f = FunctionResolver.getFunctionFromContext(this.functionName, c);
          if (!f) {
            throw new Error("Unknown function " + this.functionName);
          }
          var a = [c].concat(this.arguments);
          return f.apply(c.functionResolver.thisArg, a);
        };
        var Operators = new Object();
        Operators.equals = function(l, r) {
          return l.equals(r);
        };
        Operators.notequal = function(l, r) {
          return l.notequal(r);
        };
        Operators.lessthan = function(l, r) {
          return l.lessthan(r);
        };
        Operators.greaterthan = function(l, r) {
          return l.greaterthan(r);
        };
        Operators.lessthanorequal = function(l, r) {
          return l.lessthanorequal(r);
        };
        Operators.greaterthanorequal = function(l, r) {
          return l.greaterthanorequal(r);
        };
        XString.prototype = new Expression();
        XString.prototype.constructor = XString;
        XString.superclass = Expression.prototype;
        function XString(s) {
          if (arguments.length > 0) {
            this.init(s);
          }
        }
        XString.prototype.init = function(s) {
          this.str = String(s);
        };
        XString.prototype.toString = function() {
          return this.str;
        };
        XString.prototype.evaluate = function(c) {
          return this;
        };
        XString.prototype.string = function() {
          return this;
        };
        XString.prototype.number = function() {
          return new XNumber(this.str);
        };
        XString.prototype.bool = function() {
          return new XBoolean(this.str);
        };
        XString.prototype.nodeset = function() {
          throw new Error("Cannot convert string to nodeset");
        };
        XString.prototype.stringValue = function() {
          return this.str;
        };
        XString.prototype.numberValue = function() {
          return this.number().numberValue();
        };
        XString.prototype.booleanValue = function() {
          return this.bool().booleanValue();
        };
        XString.prototype.equals = function(r) {
          if (Utilities.instance_of(r, XBoolean)) {
            return this.bool().equals(r);
          }
          if (Utilities.instance_of(r, XNumber)) {
            return this.number().equals(r);
          }
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithString(this, Operators.equals);
          }
          return new XBoolean(this.str == r.str);
        };
        XString.prototype.notequal = function(r) {
          if (Utilities.instance_of(r, XBoolean)) {
            return this.bool().notequal(r);
          }
          if (Utilities.instance_of(r, XNumber)) {
            return this.number().notequal(r);
          }
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithString(this, Operators.notequal);
          }
          return new XBoolean(this.str != r.str);
        };
        XString.prototype.lessthan = function(r) {
          return this.number().lessthan(r);
        };
        XString.prototype.greaterthan = function(r) {
          return this.number().greaterthan(r);
        };
        XString.prototype.lessthanorequal = function(r) {
          return this.number().lessthanorequal(r);
        };
        XString.prototype.greaterthanorequal = function(r) {
          return this.number().greaterthanorequal(r);
        };
        XNumber.prototype = new Expression();
        XNumber.prototype.constructor = XNumber;
        XNumber.superclass = Expression.prototype;
        function XNumber(n) {
          if (arguments.length > 0) {
            this.init(n);
          }
        }
        XNumber.prototype.init = function(n) {
          this.num = typeof n === "string" ? this.parse(n) : Number(n);
        };
        XNumber.prototype.numberFormat = /^\s*-?[0-9]*\.?[0-9]+\s*$/;
        XNumber.prototype.parse = function(s) {
          return this.numberFormat.test(s) ? parseFloat(s) : Number.NaN;
        };
        function padSmallNumber(numberStr) {
          var parts = numberStr.split("e-");
          var base = parts[0].replace(".", "");
          var exponent = Number(parts[1]);
          for (var i = 0; i < exponent - 1; i += 1) {
            base = "0" + base;
          }
          return "0." + base;
        }
        function padLargeNumber(numberStr) {
          var parts = numberStr.split("e");
          var base = parts[0].replace(".", "");
          var exponent = Number(parts[1]);
          var zerosToAppend = exponent + 1 - base.length;
          for (var i = 0; i < zerosToAppend; i += 1) {
            base += "0";
          }
          return base;
        }
        XNumber.prototype.toString = function() {
          var strValue = this.num.toString();
          if (strValue.indexOf("e-") !== -1) {
            return padSmallNumber(strValue);
          }
          if (strValue.indexOf("e") !== -1) {
            return padLargeNumber(strValue);
          }
          return strValue;
        };
        XNumber.prototype.evaluate = function(c) {
          return this;
        };
        XNumber.prototype.string = function() {
          return new XString(this.toString());
        };
        XNumber.prototype.number = function() {
          return this;
        };
        XNumber.prototype.bool = function() {
          return new XBoolean(this.num);
        };
        XNumber.prototype.nodeset = function() {
          throw new Error("Cannot convert number to nodeset");
        };
        XNumber.prototype.stringValue = function() {
          return this.string().stringValue();
        };
        XNumber.prototype.numberValue = function() {
          return this.num;
        };
        XNumber.prototype.booleanValue = function() {
          return this.bool().booleanValue();
        };
        XNumber.prototype.negate = function() {
          return new XNumber(-this.num);
        };
        XNumber.prototype.equals = function(r) {
          if (Utilities.instance_of(r, XBoolean)) {
            return this.bool().equals(r);
          }
          if (Utilities.instance_of(r, XString)) {
            return this.equals(r.number());
          }
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithNumber(this, Operators.equals);
          }
          return new XBoolean(this.num == r.num);
        };
        XNumber.prototype.notequal = function(r) {
          if (Utilities.instance_of(r, XBoolean)) {
            return this.bool().notequal(r);
          }
          if (Utilities.instance_of(r, XString)) {
            return this.notequal(r.number());
          }
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithNumber(this, Operators.notequal);
          }
          return new XBoolean(this.num != r.num);
        };
        XNumber.prototype.lessthan = function(r) {
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithNumber(this, Operators.greaterthan);
          }
          if (Utilities.instance_of(r, XBoolean) || Utilities.instance_of(r, XString)) {
            return this.lessthan(r.number());
          }
          return new XBoolean(this.num < r.num);
        };
        XNumber.prototype.greaterthan = function(r) {
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithNumber(this, Operators.lessthan);
          }
          if (Utilities.instance_of(r, XBoolean) || Utilities.instance_of(r, XString)) {
            return this.greaterthan(r.number());
          }
          return new XBoolean(this.num > r.num);
        };
        XNumber.prototype.lessthanorequal = function(r) {
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithNumber(this, Operators.greaterthanorequal);
          }
          if (Utilities.instance_of(r, XBoolean) || Utilities.instance_of(r, XString)) {
            return this.lessthanorequal(r.number());
          }
          return new XBoolean(this.num <= r.num);
        };
        XNumber.prototype.greaterthanorequal = function(r) {
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithNumber(this, Operators.lessthanorequal);
          }
          if (Utilities.instance_of(r, XBoolean) || Utilities.instance_of(r, XString)) {
            return this.greaterthanorequal(r.number());
          }
          return new XBoolean(this.num >= r.num);
        };
        XNumber.prototype.plus = function(r) {
          return new XNumber(this.num + r.num);
        };
        XNumber.prototype.minus = function(r) {
          return new XNumber(this.num - r.num);
        };
        XNumber.prototype.multiply = function(r) {
          return new XNumber(this.num * r.num);
        };
        XNumber.prototype.div = function(r) {
          return new XNumber(this.num / r.num);
        };
        XNumber.prototype.mod = function(r) {
          return new XNumber(this.num % r.num);
        };
        XBoolean.prototype = new Expression();
        XBoolean.prototype.constructor = XBoolean;
        XBoolean.superclass = Expression.prototype;
        function XBoolean(b) {
          if (arguments.length > 0) {
            this.init(b);
          }
        }
        XBoolean.prototype.init = function(b) {
          this.b = Boolean(b);
        };
        XBoolean.prototype.toString = function() {
          return this.b.toString();
        };
        XBoolean.prototype.evaluate = function(c) {
          return this;
        };
        XBoolean.prototype.string = function() {
          return new XString(this.b);
        };
        XBoolean.prototype.number = function() {
          return new XNumber(this.b);
        };
        XBoolean.prototype.bool = function() {
          return this;
        };
        XBoolean.prototype.nodeset = function() {
          throw new Error("Cannot convert boolean to nodeset");
        };
        XBoolean.prototype.stringValue = function() {
          return this.string().stringValue();
        };
        XBoolean.prototype.numberValue = function() {
          return this.number().numberValue();
        };
        XBoolean.prototype.booleanValue = function() {
          return this.b;
        };
        XBoolean.prototype.not = function() {
          return new XBoolean(!this.b);
        };
        XBoolean.prototype.equals = function(r) {
          if (Utilities.instance_of(r, XString) || Utilities.instance_of(r, XNumber)) {
            return this.equals(r.bool());
          }
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithBoolean(this, Operators.equals);
          }
          return new XBoolean(this.b == r.b);
        };
        XBoolean.prototype.notequal = function(r) {
          if (Utilities.instance_of(r, XString) || Utilities.instance_of(r, XNumber)) {
            return this.notequal(r.bool());
          }
          if (Utilities.instance_of(r, XNodeSet)) {
            return r.compareWithBoolean(this, Operators.notequal);
          }
          return new XBoolean(this.b != r.b);
        };
        XBoolean.prototype.lessthan = function(r) {
          return this.number().lessthan(r);
        };
        XBoolean.prototype.greaterthan = function(r) {
          return this.number().greaterthan(r);
        };
        XBoolean.prototype.lessthanorequal = function(r) {
          return this.number().lessthanorequal(r);
        };
        XBoolean.prototype.greaterthanorequal = function(r) {
          return this.number().greaterthanorequal(r);
        };
        XBoolean.true_ = new XBoolean(true);
        XBoolean.false_ = new XBoolean(false);
        AVLTree.prototype = new Object();
        AVLTree.prototype.constructor = AVLTree;
        AVLTree.superclass = Object.prototype;
        function AVLTree(n) {
          this.init(n);
        }
        AVLTree.prototype.init = function(n) {
          this.left = null;
          this.right = null;
          this.node = n;
          this.depth = 1;
        };
        AVLTree.prototype.balance = function() {
          var ldepth = this.left == null ? 0 : this.left.depth;
          var rdepth = this.right == null ? 0 : this.right.depth;
          if (ldepth > rdepth + 1) {
            var lldepth = this.left.left == null ? 0 : this.left.left.depth;
            var lrdepth = this.left.right == null ? 0 : this.left.right.depth;
            if (lldepth < lrdepth) {
              this.left.rotateRR();
            }
            this.rotateLL();
          } else if (ldepth + 1 < rdepth) {
            var rrdepth = this.right.right == null ? 0 : this.right.right.depth;
            var rldepth = this.right.left == null ? 0 : this.right.left.depth;
            if (rldepth > rrdepth) {
              this.right.rotateLL();
            }
            this.rotateRR();
          }
        };
        AVLTree.prototype.rotateLL = function() {
          var nodeBefore = this.node;
          var rightBefore = this.right;
          this.node = this.left.node;
          this.right = this.left;
          this.left = this.left.left;
          this.right.left = this.right.right;
          this.right.right = rightBefore;
          this.right.node = nodeBefore;
          this.right.updateInNewLocation();
          this.updateInNewLocation();
        };
        AVLTree.prototype.rotateRR = function() {
          var nodeBefore = this.node;
          var leftBefore = this.left;
          this.node = this.right.node;
          this.left = this.right;
          this.right = this.right.right;
          this.left.right = this.left.left;
          this.left.left = leftBefore;
          this.left.node = nodeBefore;
          this.left.updateInNewLocation();
          this.updateInNewLocation();
        };
        AVLTree.prototype.updateInNewLocation = function() {
          this.getDepthFromChildren();
        };
        AVLTree.prototype.getDepthFromChildren = function() {
          this.depth = this.node == null ? 0 : 1;
          if (this.left != null) {
            this.depth = this.left.depth + 1;
          }
          if (this.right != null && this.depth <= this.right.depth) {
            this.depth = this.right.depth + 1;
          }
        };
        function nodeOrder(n1, n2) {
          if (n1 === n2) {
            return 0;
          }
          if (n1.compareDocumentPosition) {
            var cpos = n1.compareDocumentPosition(n2);
            if (cpos & 1) {
              return 1;
            }
            if (cpos & 10) {
              return 1;
            }
            if (cpos & 20) {
              return -1;
            }
            return 0;
          }
          var d1 = 0, d2 = 0;
          for (var m1 = n1; m1 != null; m1 = m1.parentNode || m1.ownerElement) {
            d1++;
          }
          for (var m2 = n2; m2 != null; m2 = m2.parentNode || m2.ownerElement) {
            d2++;
          }
          if (d1 > d2) {
            while (d1 > d2) {
              n1 = n1.parentNode || n1.ownerElement;
              d1--;
            }
            if (n1 === n2) {
              return 1;
            }
          } else if (d2 > d1) {
            while (d2 > d1) {
              n2 = n2.parentNode || n2.ownerElement;
              d2--;
            }
            if (n1 === n2) {
              return -1;
            }
          }
          var n1Par = n1.parentNode || n1.ownerElement, n2Par = n2.parentNode || n2.ownerElement;
          while (n1Par !== n2Par) {
            n1 = n1Par;
            n2 = n2Par;
            n1Par = n1.parentNode || n1.ownerElement;
            n2Par = n2.parentNode || n2.ownerElement;
          }
          var n1isAttr = isAttributeLike(n1);
          var n2isAttr = isAttributeLike(n2);
          if (n1isAttr && !n2isAttr) {
            return -1;
          }
          if (!n1isAttr && n2isAttr) {
            return 1;
          }
          if (n1.isXPathNamespace) {
            if (n1.nodeValue === XPath.XML_NAMESPACE_URI) {
              return -1;
            }
            if (!n2.isXPathNamespace) {
              return -1;
            }
            if (n2.nodeValue === XPath.XML_NAMESPACE_URI) {
              return 1;
            }
          } else if (n2.isXPathNamespace) {
            return 1;
          }
          if (n1Par) {
            var cn = n1isAttr ? n1Par.attributes : n1Par.childNodes;
            var len = cn.length;
            var n1Compare = n1.baseNode || n1;
            var n2Compare = n2.baseNode || n2;
            for (var i = 0; i < len; i += 1) {
              var n = cn[i];
              if (n === n1Compare) {
                return -1;
              }
              if (n === n2Compare) {
                return 1;
              }
            }
          }
          throw new Error("Unexpected: could not determine node order");
        }
        AVLTree.prototype.add = function(n) {
          if (n === this.node) {
            return false;
          }
          var o = nodeOrder(n, this.node);
          var ret = false;
          if (o == -1) {
            if (this.left == null) {
              this.left = new AVLTree(n);
              ret = true;
            } else {
              ret = this.left.add(n);
              if (ret) {
                this.balance();
              }
            }
          } else if (o == 1) {
            if (this.right == null) {
              this.right = new AVLTree(n);
              ret = true;
            } else {
              ret = this.right.add(n);
              if (ret) {
                this.balance();
              }
            }
          }
          if (ret) {
            this.getDepthFromChildren();
          }
          return ret;
        };
        XNodeSet.prototype = new Expression();
        XNodeSet.prototype.constructor = XNodeSet;
        XNodeSet.superclass = Expression.prototype;
        function XNodeSet() {
          this.init();
        }
        XNodeSet.prototype.init = function() {
          this.tree = null;
          this.nodes = [];
          this.size = 0;
        };
        XNodeSet.prototype.toString = function() {
          var p = this.first();
          if (p == null) {
            return "";
          }
          return this.stringForNode(p);
        };
        XNodeSet.prototype.evaluate = function(c) {
          return this;
        };
        XNodeSet.prototype.string = function() {
          return new XString(this.toString());
        };
        XNodeSet.prototype.stringValue = function() {
          return this.toString();
        };
        XNodeSet.prototype.number = function() {
          return new XNumber(this.string());
        };
        XNodeSet.prototype.numberValue = function() {
          return Number(this.string());
        };
        XNodeSet.prototype.bool = function() {
          return new XBoolean(this.booleanValue());
        };
        XNodeSet.prototype.booleanValue = function() {
          return !!this.size;
        };
        XNodeSet.prototype.nodeset = function() {
          return this;
        };
        XNodeSet.prototype.stringForNode = function(n) {
          if (n.nodeType == NodeTypes.DOCUMENT_NODE || n.nodeType == NodeTypes.ELEMENT_NODE || n.nodeType === NodeTypes.DOCUMENT_FRAGMENT_NODE) {
            return this.stringForContainerNode(n);
          }
          if (n.nodeType === NodeTypes.ATTRIBUTE_NODE) {
            return n.value || n.nodeValue;
          }
          if (n.isNamespaceNode) {
            return n.namespace;
          }
          return n.nodeValue;
        };
        XNodeSet.prototype.stringForContainerNode = function(n) {
          var s = "";
          for (var n2 = n.firstChild; n2 != null; n2 = n2.nextSibling) {
            var nt = n2.nodeType;
            if (nt === 1 || nt === 3 || nt === 4 || nt === 9 || nt === 11) {
              s += this.stringForNode(n2);
            }
          }
          return s;
        };
        XNodeSet.prototype.buildTree = function() {
          if (!this.tree && this.nodes.length) {
            this.tree = new AVLTree(this.nodes[0]);
            for (var i = 1; i < this.nodes.length; i += 1) {
              this.tree.add(this.nodes[i]);
            }
          }
          return this.tree;
        };
        XNodeSet.prototype.first = function() {
          var p = this.buildTree();
          if (p == null) {
            return null;
          }
          while (p.left != null) {
            p = p.left;
          }
          return p.node;
        };
        XNodeSet.prototype.add = function(n) {
          for (var i = 0; i < this.nodes.length; i += 1) {
            if (n === this.nodes[i]) {
              return;
            }
          }
          this.tree = null;
          this.nodes.push(n);
          this.size += 1;
        };
        XNodeSet.prototype.addArray = function(ns) {
          var self = this;
          forEach(function(x) {
            self.add(x);
          }, ns);
        };
        XNodeSet.prototype.toArray = function() {
          var a = [];
          this.toArrayRec(this.buildTree(), a);
          return a;
        };
        XNodeSet.prototype.toArrayRec = function(t, a) {
          if (t != null) {
            this.toArrayRec(t.left, a);
            a.push(t.node);
            this.toArrayRec(t.right, a);
          }
        };
        XNodeSet.prototype.toUnsortedArray = function() {
          return this.nodes.slice();
        };
        XNodeSet.prototype.compareWithString = function(r, o) {
          var a = this.toUnsortedArray();
          for (var i = 0; i < a.length; i++) {
            var n = a[i];
            var l = new XString(this.stringForNode(n));
            var res = o(l, r);
            if (res.booleanValue()) {
              return res;
            }
          }
          return new XBoolean(false);
        };
        XNodeSet.prototype.compareWithNumber = function(r, o) {
          var a = this.toUnsortedArray();
          for (var i = 0; i < a.length; i++) {
            var n = a[i];
            var l = new XNumber(this.stringForNode(n));
            var res = o(l, r);
            if (res.booleanValue()) {
              return res;
            }
          }
          return new XBoolean(false);
        };
        XNodeSet.prototype.compareWithBoolean = function(r, o) {
          return o(this.bool(), r);
        };
        XNodeSet.prototype.compareWithNodeSet = function(r, o) {
          var arr = this.toUnsortedArray();
          var oInvert = function(lop, rop) {
            return o(rop, lop);
          };
          for (var i = 0; i < arr.length; i++) {
            var l = new XString(this.stringForNode(arr[i]));
            var res = r.compareWithString(l, oInvert);
            if (res.booleanValue()) {
              return res;
            }
          }
          return new XBoolean(false);
        };
        XNodeSet.compareWith = curry(function(o, r) {
          if (Utilities.instance_of(r, XString)) {
            return this.compareWithString(r, o);
          }
          if (Utilities.instance_of(r, XNumber)) {
            return this.compareWithNumber(r, o);
          }
          if (Utilities.instance_of(r, XBoolean)) {
            return this.compareWithBoolean(r, o);
          }
          return this.compareWithNodeSet(r, o);
        });
        XNodeSet.prototype.equals = XNodeSet.compareWith(Operators.equals);
        XNodeSet.prototype.notequal = XNodeSet.compareWith(Operators.notequal);
        XNodeSet.prototype.lessthan = XNodeSet.compareWith(Operators.lessthan);
        XNodeSet.prototype.greaterthan = XNodeSet.compareWith(Operators.greaterthan);
        XNodeSet.prototype.lessthanorequal = XNodeSet.compareWith(Operators.lessthanorequal);
        XNodeSet.prototype.greaterthanorequal = XNodeSet.compareWith(Operators.greaterthanorequal);
        XNodeSet.prototype.union = function(r) {
          var ns = new XNodeSet();
          ns.addArray(this.toUnsortedArray());
          ns.addArray(r.toUnsortedArray());
          return ns;
        };
        XPathNamespace.prototype = new Object();
        XPathNamespace.prototype.constructor = XPathNamespace;
        XPathNamespace.superclass = Object.prototype;
        function XPathNamespace(pre, node, uri, p) {
          this.isXPathNamespace = true;
          this.baseNode = node;
          this.ownerDocument = p.ownerDocument;
          this.nodeName = pre;
          this.prefix = pre;
          this.localName = pre;
          this.namespaceURI = null;
          this.nodeValue = uri;
          this.ownerElement = p;
          this.nodeType = NodeTypes.NAMESPACE_NODE;
        }
        XPathNamespace.prototype.toString = function() {
          return '{ "' + this.prefix + '", "' + this.namespaceURI + '" }';
        };
        XPathContext.prototype = new Object();
        XPathContext.prototype.constructor = XPathContext;
        XPathContext.superclass = Object.prototype;
        function XPathContext(vr, nr, fr) {
          this.variableResolver = vr != null ? vr : new VariableResolver();
          this.namespaceResolver = nr != null ? nr : new NamespaceResolver();
          this.functionResolver = fr != null ? fr : new FunctionResolver();
        }
        XPathContext.prototype.extend = function(newProps) {
          return assign(new XPathContext(), this, newProps);
        };
        VariableResolver.prototype = new Object();
        VariableResolver.prototype.constructor = VariableResolver;
        VariableResolver.superclass = Object.prototype;
        function VariableResolver() {
        }
        VariableResolver.prototype.getVariable = function(ln, ns) {
          return null;
        };
        FunctionResolver.prototype = new Object();
        FunctionResolver.prototype.constructor = FunctionResolver;
        FunctionResolver.superclass = Object.prototype;
        function FunctionResolver(thisArg) {
          this.thisArg = thisArg != null ? thisArg : Functions;
          this.functions = new Object();
          this.addStandardFunctions();
        }
        FunctionResolver.prototype.addStandardFunctions = function() {
          this.functions["{}last"] = Functions.last;
          this.functions["{}position"] = Functions.position;
          this.functions["{}count"] = Functions.count;
          this.functions["{}id"] = Functions.id;
          this.functions["{}local-name"] = Functions.localName;
          this.functions["{}namespace-uri"] = Functions.namespaceURI;
          this.functions["{}name"] = Functions.name;
          this.functions["{}string"] = Functions.string;
          this.functions["{}concat"] = Functions.concat;
          this.functions["{}starts-with"] = Functions.startsWith;
          this.functions["{}contains"] = Functions.contains;
          this.functions["{}substring-before"] = Functions.substringBefore;
          this.functions["{}substring-after"] = Functions.substringAfter;
          this.functions["{}substring"] = Functions.substring;
          this.functions["{}string-length"] = Functions.stringLength;
          this.functions["{}normalize-space"] = Functions.normalizeSpace;
          this.functions["{}translate"] = Functions.translate;
          this.functions["{}boolean"] = Functions.boolean_;
          this.functions["{}not"] = Functions.not;
          this.functions["{}true"] = Functions.true_;
          this.functions["{}false"] = Functions.false_;
          this.functions["{}lang"] = Functions.lang;
          this.functions["{}number"] = Functions.number;
          this.functions["{}sum"] = Functions.sum;
          this.functions["{}floor"] = Functions.floor;
          this.functions["{}ceiling"] = Functions.ceiling;
          this.functions["{}round"] = Functions.round;
        };
        FunctionResolver.prototype.addFunction = function(ns, ln, f) {
          this.functions["{" + ns + "}" + ln] = f;
        };
        FunctionResolver.getFunctionFromContext = function(qName, context) {
          var parts = Utilities.resolveQName(qName, context.namespaceResolver, context.contextNode, false);
          if (parts[0] === null) {
            throw new Error("Cannot resolve QName " + name);
          }
          return context.functionResolver.getFunction(parts[1], parts[0]);
        };
        FunctionResolver.prototype.getFunction = function(localName, namespace) {
          return this.functions["{" + namespace + "}" + localName];
        };
        NamespaceResolver.prototype = new Object();
        NamespaceResolver.prototype.constructor = NamespaceResolver;
        NamespaceResolver.superclass = Object.prototype;
        function NamespaceResolver() {
        }
        NamespaceResolver.prototype.getNamespace = function(prefix, n) {
          if (prefix == "xml") {
            return XPath.XML_NAMESPACE_URI;
          } else if (prefix == "xmlns") {
            return XPath.XMLNS_NAMESPACE_URI;
          }
          if (n.nodeType == NodeTypes.DOCUMENT_NODE) {
            n = n.documentElement;
          } else if (n.nodeType == NodeTypes.ATTRIBUTE_NODE) {
            n = PathExpr.getOwnerElement(n);
          } else if (n.nodeType != NodeTypes.ELEMENT_NODE) {
            n = n.parentNode;
          }
          while (n != null && n.nodeType == NodeTypes.ELEMENT_NODE) {
            var nnm = n.attributes;
            for (var i = 0; i < nnm.length; i++) {
              var a = nnm.item(i);
              var aname = a.name || a.nodeName;
              if (aname === "xmlns" && prefix === "" || aname === "xmlns:" + prefix) {
                return String(a.value || a.nodeValue);
              }
            }
            n = n.parentNode;
          }
          return null;
        };
        var Functions = new Object();
        Functions.last = function(c) {
          if (arguments.length != 1) {
            throw new Error("Function last expects ()");
          }
          return new XNumber(c.contextSize);
        };
        Functions.position = function(c) {
          if (arguments.length != 1) {
            throw new Error("Function position expects ()");
          }
          return new XNumber(c.contextPosition);
        };
        Functions.count = function() {
          var c = arguments[0];
          var ns;
          if (arguments.length != 2 || !Utilities.instance_of(ns = arguments[1].evaluate(c), XNodeSet)) {
            throw new Error("Function count expects (node-set)");
          }
          return new XNumber(ns.size);
        };
        Functions.id = function() {
          var c = arguments[0];
          var id;
          if (arguments.length != 2) {
            throw new Error("Function id expects (object)");
          }
          id = arguments[1].evaluate(c);
          if (Utilities.instance_of(id, XNodeSet)) {
            id = id.toArray().join(" ");
          } else {
            id = id.stringValue();
          }
          var ids = id.split(/[\x0d\x0a\x09\x20]+/);
          var count = 0;
          var ns = new XNodeSet();
          var doc = c.contextNode.nodeType == NodeTypes.DOCUMENT_NODE ? c.contextNode : c.contextNode.ownerDocument;
          for (var i = 0; i < ids.length; i++) {
            var n;
            if (doc.getElementById) {
              n = doc.getElementById(ids[i]);
            } else {
              n = Utilities.getElementById(doc, ids[i]);
            }
            if (n != null) {
              ns.add(n);
              count++;
            }
          }
          return ns;
        };
        Functions.localName = function(c, eNode) {
          var n;
          if (arguments.length == 1) {
            n = c.contextNode;
          } else if (arguments.length == 2) {
            n = eNode.evaluate(c).first();
          } else {
            throw new Error("Function local-name expects (node-set?)");
          }
          if (n == null) {
            return new XString("");
          }
          return new XString(
            n.localName || //  standard elements and attributes
            n.baseName || //  IE
            n.target || //  processing instructions
            n.nodeName || //  DOM1 elements
            ""
            //  fallback
          );
        };
        Functions.namespaceURI = function() {
          var c = arguments[0];
          var n;
          if (arguments.length == 1) {
            n = c.contextNode;
          } else if (arguments.length == 2) {
            n = arguments[1].evaluate(c).first();
          } else {
            throw new Error("Function namespace-uri expects (node-set?)");
          }
          if (n == null) {
            return new XString("");
          }
          return new XString(n.namespaceURI || "");
        };
        Functions.name = function() {
          var c = arguments[0];
          var n;
          if (arguments.length == 1) {
            n = c.contextNode;
          } else if (arguments.length == 2) {
            n = arguments[1].evaluate(c).first();
          } else {
            throw new Error("Function name expects (node-set?)");
          }
          if (n == null) {
            return new XString("");
          }
          if (n.nodeType == NodeTypes.ELEMENT_NODE) {
            return new XString(n.nodeName);
          } else if (n.nodeType == NodeTypes.ATTRIBUTE_NODE) {
            return new XString(n.name || n.nodeName);
          } else if (n.nodeType === NodeTypes.PROCESSING_INSTRUCTION_NODE) {
            return new XString(n.target || n.nodeName);
          } else if (n.localName == null) {
            return new XString("");
          } else {
            return new XString(n.localName);
          }
        };
        Functions.string = function() {
          var c = arguments[0];
          if (arguments.length == 1) {
            return new XString(XNodeSet.prototype.stringForNode(c.contextNode));
          } else if (arguments.length == 2) {
            return arguments[1].evaluate(c).string();
          }
          throw new Error("Function string expects (object?)");
        };
        Functions.concat = function(c) {
          if (arguments.length < 3) {
            throw new Error("Function concat expects (string, string[, string]*)");
          }
          var s = "";
          for (var i = 1; i < arguments.length; i++) {
            s += arguments[i].evaluate(c).stringValue();
          }
          return new XString(s);
        };
        Functions.startsWith = function() {
          var c = arguments[0];
          if (arguments.length != 3) {
            throw new Error("Function startsWith expects (string, string)");
          }
          var s1 = arguments[1].evaluate(c).stringValue();
          var s2 = arguments[2].evaluate(c).stringValue();
          return new XBoolean(s1.substring(0, s2.length) == s2);
        };
        Functions.contains = function() {
          var c = arguments[0];
          if (arguments.length != 3) {
            throw new Error("Function contains expects (string, string)");
          }
          var s1 = arguments[1].evaluate(c).stringValue();
          var s2 = arguments[2].evaluate(c).stringValue();
          return new XBoolean(s1.indexOf(s2) !== -1);
        };
        Functions.substringBefore = function() {
          var c = arguments[0];
          if (arguments.length != 3) {
            throw new Error("Function substring-before expects (string, string)");
          }
          var s1 = arguments[1].evaluate(c).stringValue();
          var s2 = arguments[2].evaluate(c).stringValue();
          return new XString(s1.substring(0, s1.indexOf(s2)));
        };
        Functions.substringAfter = function() {
          var c = arguments[0];
          if (arguments.length != 3) {
            throw new Error("Function substring-after expects (string, string)");
          }
          var s1 = arguments[1].evaluate(c).stringValue();
          var s2 = arguments[2].evaluate(c).stringValue();
          if (s2.length == 0) {
            return new XString(s1);
          }
          var i = s1.indexOf(s2);
          if (i == -1) {
            return new XString("");
          }
          return new XString(s1.substring(i + s2.length));
        };
        Functions.substring = function() {
          var c = arguments[0];
          if (!(arguments.length == 3 || arguments.length == 4)) {
            throw new Error("Function substring expects (string, number, number?)");
          }
          var s = arguments[1].evaluate(c).stringValue();
          var n1 = Math.round(arguments[2].evaluate(c).numberValue()) - 1;
          var n2 = arguments.length == 4 ? n1 + Math.round(arguments[3].evaluate(c).numberValue()) : void 0;
          return new XString(s.substring(n1, n2));
        };
        Functions.stringLength = function() {
          var c = arguments[0];
          var s;
          if (arguments.length == 1) {
            s = XNodeSet.prototype.stringForNode(c.contextNode);
          } else if (arguments.length == 2) {
            s = arguments[1].evaluate(c).stringValue();
          } else {
            throw new Error("Function string-length expects (string?)");
          }
          return new XNumber(s.length);
        };
        Functions.normalizeSpace = function() {
          var c = arguments[0];
          var s;
          if (arguments.length == 1) {
            s = XNodeSet.prototype.stringForNode(c.contextNode);
          } else if (arguments.length == 2) {
            s = arguments[1].evaluate(c).stringValue();
          } else {
            throw new Error("Function normalize-space expects (string?)");
          }
          var i = 0;
          var j = s.length - 1;
          while (Utilities.isSpace(s.charCodeAt(j))) {
            j--;
          }
          var t = "";
          while (i <= j && Utilities.isSpace(s.charCodeAt(i))) {
            i++;
          }
          while (i <= j) {
            if (Utilities.isSpace(s.charCodeAt(i))) {
              t += " ";
              while (i <= j && Utilities.isSpace(s.charCodeAt(i))) {
                i++;
              }
            } else {
              t += s.charAt(i);
              i++;
            }
          }
          return new XString(t);
        };
        Functions.translate = function(c, eValue, eFrom, eTo) {
          if (arguments.length != 4) {
            throw new Error("Function translate expects (string, string, string)");
          }
          var value = eValue.evaluate(c).stringValue();
          var from = eFrom.evaluate(c).stringValue();
          var to = eTo.evaluate(c).stringValue();
          var cMap = reduce(function(acc, ch, i) {
            if (!(ch in acc)) {
              acc[ch] = i > to.length ? "" : to[i];
            }
            return acc;
          }, {}, from);
          var t = join(
            "",
            map(function(ch) {
              return ch in cMap ? cMap[ch] : ch;
            }, value)
          );
          return new XString(t);
        };
        Functions.boolean_ = function() {
          var c = arguments[0];
          if (arguments.length != 2) {
            throw new Error("Function boolean expects (object)");
          }
          return arguments[1].evaluate(c).bool();
        };
        Functions.not = function(c, eValue) {
          if (arguments.length != 2) {
            throw new Error("Function not expects (object)");
          }
          return eValue.evaluate(c).bool().not();
        };
        Functions.true_ = function() {
          if (arguments.length != 1) {
            throw new Error("Function true expects ()");
          }
          return XBoolean.true_;
        };
        Functions.false_ = function() {
          if (arguments.length != 1) {
            throw new Error("Function false expects ()");
          }
          return XBoolean.false_;
        };
        Functions.lang = function() {
          var c = arguments[0];
          if (arguments.length != 2) {
            throw new Error("Function lang expects (string)");
          }
          var lang;
          for (var n = c.contextNode; n != null && n.nodeType != NodeTypes.DOCUMENT_NODE; n = n.parentNode) {
            var a = n.getAttributeNS(XPath.XML_NAMESPACE_URI, "lang");
            if (a != null) {
              lang = String(a);
              break;
            }
          }
          if (lang == null) {
            return XBoolean.false_;
          }
          var s = arguments[1].evaluate(c).stringValue();
          return new XBoolean(lang.substring(0, s.length) == s && (lang.length == s.length || lang.charAt(s.length) == "-"));
        };
        Functions.number = function() {
          var c = arguments[0];
          if (!(arguments.length == 1 || arguments.length == 2)) {
            throw new Error("Function number expects (object?)");
          }
          if (arguments.length == 1) {
            return new XNumber(XNodeSet.prototype.stringForNode(c.contextNode));
          }
          return arguments[1].evaluate(c).number();
        };
        Functions.sum = function() {
          var c = arguments[0];
          var ns;
          if (arguments.length != 2 || !Utilities.instance_of(ns = arguments[1].evaluate(c), XNodeSet)) {
            throw new Error("Function sum expects (node-set)");
          }
          ns = ns.toUnsortedArray();
          var n = 0;
          for (var i = 0; i < ns.length; i++) {
            n += new XNumber(XNodeSet.prototype.stringForNode(ns[i])).numberValue();
          }
          return new XNumber(n);
        };
        Functions.floor = function() {
          var c = arguments[0];
          if (arguments.length != 2) {
            throw new Error("Function floor expects (number)");
          }
          return new XNumber(Math.floor(arguments[1].evaluate(c).numberValue()));
        };
        Functions.ceiling = function() {
          var c = arguments[0];
          if (arguments.length != 2) {
            throw new Error("Function ceiling expects (number)");
          }
          return new XNumber(Math.ceil(arguments[1].evaluate(c).numberValue()));
        };
        Functions.round = function() {
          var c = arguments[0];
          if (arguments.length != 2) {
            throw new Error("Function round expects (number)");
          }
          return new XNumber(Math.round(arguments[1].evaluate(c).numberValue()));
        };
        var Utilities = new Object();
        var isAttributeLike = function(val) {
          return val && (val.nodeType === NodeTypes.ATTRIBUTE_NODE || val.ownerElement || val.isXPathNamespace);
        };
        Utilities.splitQName = function(qn) {
          var i = qn.indexOf(":");
          if (i == -1) {
            return [null, qn];
          }
          return [qn.substring(0, i), qn.substring(i + 1)];
        };
        Utilities.resolveQName = function(qn, nr, n, useDefault) {
          var parts = Utilities.splitQName(qn);
          if (parts[0] != null) {
            parts[0] = nr.getNamespace(parts[0], n);
          } else {
            if (useDefault) {
              parts[0] = nr.getNamespace("", n);
              if (parts[0] == null) {
                parts[0] = "";
              }
            } else {
              parts[0] = "";
            }
          }
          return parts;
        };
        Utilities.isSpace = function(c) {
          return c == 9 || c == 13 || c == 10 || c == 32;
        };
        Utilities.isLetter = function(c) {
          return c >= 65 && c <= 90 || c >= 97 && c <= 122 || c >= 192 && c <= 214 || c >= 216 && c <= 246 || c >= 248 && c <= 255 || c >= 256 && c <= 305 || c >= 308 && c <= 318 || c >= 321 && c <= 328 || c >= 330 && c <= 382 || c >= 384 && c <= 451 || c >= 461 && c <= 496 || c >= 500 && c <= 501 || c >= 506 && c <= 535 || c >= 592 && c <= 680 || c >= 699 && c <= 705 || c == 902 || c >= 904 && c <= 906 || c == 908 || c >= 910 && c <= 929 || c >= 931 && c <= 974 || c >= 976 && c <= 982 || c == 986 || c == 988 || c == 990 || c == 992 || c >= 994 && c <= 1011 || c >= 1025 && c <= 1036 || c >= 1038 && c <= 1103 || c >= 1105 && c <= 1116 || c >= 1118 && c <= 1153 || c >= 1168 && c <= 1220 || c >= 1223 && c <= 1224 || c >= 1227 && c <= 1228 || c >= 1232 && c <= 1259 || c >= 1262 && c <= 1269 || c >= 1272 && c <= 1273 || c >= 1329 && c <= 1366 || c == 1369 || c >= 1377 && c <= 1414 || c >= 1488 && c <= 1514 || c >= 1520 && c <= 1522 || c >= 1569 && c <= 1594 || c >= 1601 && c <= 1610 || c >= 1649 && c <= 1719 || c >= 1722 && c <= 1726 || c >= 1728 && c <= 1742 || c >= 1744 && c <= 1747 || c == 1749 || c >= 1765 && c <= 1766 || c >= 2309 && c <= 2361 || c == 2365 || c >= 2392 && c <= 2401 || c >= 2437 && c <= 2444 || c >= 2447 && c <= 2448 || c >= 2451 && c <= 2472 || c >= 2474 && c <= 2480 || c == 2482 || c >= 2486 && c <= 2489 || c >= 2524 && c <= 2525 || c >= 2527 && c <= 2529 || c >= 2544 && c <= 2545 || c >= 2565 && c <= 2570 || c >= 2575 && c <= 2576 || c >= 2579 && c <= 2600 || c >= 2602 && c <= 2608 || c >= 2610 && c <= 2611 || c >= 2613 && c <= 2614 || c >= 2616 && c <= 2617 || c >= 2649 && c <= 2652 || c == 2654 || c >= 2674 && c <= 2676 || c >= 2693 && c <= 2699 || c == 2701 || c >= 2703 && c <= 2705 || c >= 2707 && c <= 2728 || c >= 2730 && c <= 2736 || c >= 2738 && c <= 2739 || c >= 2741 && c <= 2745 || c == 2749 || c == 2784 || c >= 2821 && c <= 2828 || c >= 2831 && c <= 2832 || c >= 2835 && c <= 2856 || c >= 2858 && c <= 2864 || c >= 2866 && c <= 2867 || c >= 2870 && c <= 2873 || c == 2877 || c >= 2908 && c <= 2909 || c >= 2911 && c <= 2913 || c >= 2949 && c <= 2954 || c >= 2958 && c <= 2960 || c >= 2962 && c <= 2965 || c >= 2969 && c <= 2970 || c == 2972 || c >= 2974 && c <= 2975 || c >= 2979 && c <= 2980 || c >= 2984 && c <= 2986 || c >= 2990 && c <= 2997 || c >= 2999 && c <= 3001 || c >= 3077 && c <= 3084 || c >= 3086 && c <= 3088 || c >= 3090 && c <= 3112 || c >= 3114 && c <= 3123 || c >= 3125 && c <= 3129 || c >= 3168 && c <= 3169 || c >= 3205 && c <= 3212 || c >= 3214 && c <= 3216 || c >= 3218 && c <= 3240 || c >= 3242 && c <= 3251 || c >= 3253 && c <= 3257 || c == 3294 || c >= 3296 && c <= 3297 || c >= 3333 && c <= 3340 || c >= 3342 && c <= 3344 || c >= 3346 && c <= 3368 || c >= 3370 && c <= 3385 || c >= 3424 && c <= 3425 || c >= 3585 && c <= 3630 || c == 3632 || c >= 3634 && c <= 3635 || c >= 3648 && c <= 3653 || c >= 3713 && c <= 3714 || c == 3716 || c >= 3719 && c <= 3720 || c == 3722 || c == 3725 || c >= 3732 && c <= 3735 || c >= 3737 && c <= 3743 || c >= 3745 && c <= 3747 || c == 3749 || c == 3751 || c >= 3754 && c <= 3755 || c >= 3757 && c <= 3758 || c == 3760 || c >= 3762 && c <= 3763 || c == 3773 || c >= 3776 && c <= 3780 || c >= 3904 && c <= 3911 || c >= 3913 && c <= 3945 || c >= 4256 && c <= 4293 || c >= 4304 && c <= 4342 || c == 4352 || c >= 4354 && c <= 4355 || c >= 4357 && c <= 4359 || c == 4361 || c >= 4363 && c <= 4364 || c >= 4366 && c <= 4370 || c == 4412 || c == 4414 || c == 4416 || c == 4428 || c == 4430 || c == 4432 || c >= 4436 && c <= 4437 || c == 4441 || c >= 4447 && c <= 4449 || c == 4451 || c == 4453 || c == 4455 || c == 4457 || c >= 4461 && c <= 4462 || c >= 4466 && c <= 4467 || c == 4469 || c == 4510 || c == 4520 || c == 4523 || c >= 4526 && c <= 4527 || c >= 4535 && c <= 4536 || c == 4538 || c >= 4540 && c <= 4546 || c == 4587 || c == 4592 || c == 4601 || c >= 7680 && c <= 7835 || c >= 7840 && c <= 7929 || c >= 7936 && c <= 7957 || c >= 7960 && c <= 7965 || c >= 7968 && c <= 8005 || c >= 8008 && c <= 8013 || c >= 8016 && c <= 8023 || c == 8025 || c == 8027 || c == 8029 || c >= 8031 && c <= 8061 || c >= 8064 && c <= 8116 || c >= 8118 && c <= 8124 || c == 8126 || c >= 8130 && c <= 8132 || c >= 8134 && c <= 8140 || c >= 8144 && c <= 8147 || c >= 8150 && c <= 8155 || c >= 8160 && c <= 8172 || c >= 8178 && c <= 8180 || c >= 8182 && c <= 8188 || c == 8486 || c >= 8490 && c <= 8491 || c == 8494 || c >= 8576 && c <= 8578 || c >= 12353 && c <= 12436 || c >= 12449 && c <= 12538 || c >= 12549 && c <= 12588 || c >= 44032 && c <= 55203 || c >= 19968 && c <= 40869 || c == 12295 || c >= 12321 && c <= 12329;
        };
        Utilities.isNCNameChar = function(c) {
          return c >= 48 && c <= 57 || c >= 1632 && c <= 1641 || c >= 1776 && c <= 1785 || c >= 2406 && c <= 2415 || c >= 2534 && c <= 2543 || c >= 2662 && c <= 2671 || c >= 2790 && c <= 2799 || c >= 2918 && c <= 2927 || c >= 3047 && c <= 3055 || c >= 3174 && c <= 3183 || c >= 3302 && c <= 3311 || c >= 3430 && c <= 3439 || c >= 3664 && c <= 3673 || c >= 3792 && c <= 3801 || c >= 3872 && c <= 3881 || c == 46 || c == 45 || c == 95 || Utilities.isLetter(c) || c >= 768 && c <= 837 || c >= 864 && c <= 865 || c >= 1155 && c <= 1158 || c >= 1425 && c <= 1441 || c >= 1443 && c <= 1465 || c >= 1467 && c <= 1469 || c == 1471 || c >= 1473 && c <= 1474 || c == 1476 || c >= 1611 && c <= 1618 || c == 1648 || c >= 1750 && c <= 1756 || c >= 1757 && c <= 1759 || c >= 1760 && c <= 1764 || c >= 1767 && c <= 1768 || c >= 1770 && c <= 1773 || c >= 2305 && c <= 2307 || c == 2364 || c >= 2366 && c <= 2380 || c == 2381 || c >= 2385 && c <= 2388 || c >= 2402 && c <= 2403 || c >= 2433 && c <= 2435 || c == 2492 || c == 2494 || c == 2495 || c >= 2496 && c <= 2500 || c >= 2503 && c <= 2504 || c >= 2507 && c <= 2509 || c == 2519 || c >= 2530 && c <= 2531 || c == 2562 || c == 2620 || c == 2622 || c == 2623 || c >= 2624 && c <= 2626 || c >= 2631 && c <= 2632 || c >= 2635 && c <= 2637 || c >= 2672 && c <= 2673 || c >= 2689 && c <= 2691 || c == 2748 || c >= 2750 && c <= 2757 || c >= 2759 && c <= 2761 || c >= 2763 && c <= 2765 || c >= 2817 && c <= 2819 || c == 2876 || c >= 2878 && c <= 2883 || c >= 2887 && c <= 2888 || c >= 2891 && c <= 2893 || c >= 2902 && c <= 2903 || c >= 2946 && c <= 2947 || c >= 3006 && c <= 3010 || c >= 3014 && c <= 3016 || c >= 3018 && c <= 3021 || c == 3031 || c >= 3073 && c <= 3075 || c >= 3134 && c <= 3140 || c >= 3142 && c <= 3144 || c >= 3146 && c <= 3149 || c >= 3157 && c <= 3158 || c >= 3202 && c <= 3203 || c >= 3262 && c <= 3268 || c >= 3270 && c <= 3272 || c >= 3274 && c <= 3277 || c >= 3285 && c <= 3286 || c >= 3330 && c <= 3331 || c >= 3390 && c <= 3395 || c >= 3398 && c <= 3400 || c >= 3402 && c <= 3405 || c == 3415 || c == 3633 || c >= 3636 && c <= 3642 || c >= 3655 && c <= 3662 || c == 3761 || c >= 3764 && c <= 3769 || c >= 3771 && c <= 3772 || c >= 3784 && c <= 3789 || c >= 3864 && c <= 3865 || c == 3893 || c == 3895 || c == 3897 || c == 3902 || c == 3903 || c >= 3953 && c <= 3972 || c >= 3974 && c <= 3979 || c >= 3984 && c <= 3989 || c == 3991 || c >= 3993 && c <= 4013 || c >= 4017 && c <= 4023 || c == 4025 || c >= 8400 && c <= 8412 || c == 8417 || c >= 12330 && c <= 12335 || c == 12441 || c == 12442 || c == 183 || c == 720 || c == 721 || c == 903 || c == 1600 || c == 3654 || c == 3782 || c == 12293 || c >= 12337 && c <= 12341 || c >= 12445 && c <= 12446 || c >= 12540 && c <= 12542;
        };
        Utilities.coalesceText = function(n) {
          for (var m = n.firstChild; m != null; m = m.nextSibling) {
            if (m.nodeType == NodeTypes.TEXT_NODE || m.nodeType == NodeTypes.CDATA_SECTION_NODE) {
              var s = m.nodeValue;
              var first = m;
              m = m.nextSibling;
              while (m != null && (m.nodeType == NodeTypes.TEXT_NODE || m.nodeType == NodeTypes.CDATA_SECTION_NODE)) {
                s += m.nodeValue;
                var del = m;
                m = m.nextSibling;
                del.parentNode.removeChild(del);
              }
              if (first.nodeType == NodeTypes.CDATA_SECTION_NODE) {
                var p = first.parentNode;
                if (first.nextSibling == null) {
                  p.removeChild(first);
                  p.appendChild(p.ownerDocument.createTextNode(s));
                } else {
                  var next = first.nextSibling;
                  p.removeChild(first);
                  p.insertBefore(p.ownerDocument.createTextNode(s), next);
                }
              } else {
                first.nodeValue = s;
              }
              if (m == null) {
                break;
              }
            } else if (m.nodeType == NodeTypes.ELEMENT_NODE) {
              Utilities.coalesceText(m);
            }
          }
        };
        Utilities.instance_of = function(o, c) {
          while (o != null) {
            if (o.constructor === c) {
              return true;
            }
            if (o === Object) {
              return false;
            }
            o = o.constructor.superclass;
          }
          return false;
        };
        Utilities.getElementById = function(n, id) {
          if (n.nodeType == NodeTypes.ELEMENT_NODE) {
            if (n.getAttribute("id") == id || n.getAttributeNS(null, "id") == id) {
              return n;
            }
          }
          for (var m = n.firstChild; m != null; m = m.nextSibling) {
            var res = Utilities.getElementById(m, id);
            if (res != null) {
              return res;
            }
          }
          return null;
        };
        var XPathException = function() {
          function getMessage(code, exception) {
            var msg = exception ? ": " + exception.toString() : "";
            switch (code) {
              case XPathException2.INVALID_EXPRESSION_ERR:
                return "Invalid expression" + msg;
              case XPathException2.TYPE_ERR:
                return "Type error" + msg;
            }
            return null;
          }
          function XPathException2(code, error, message) {
            var err = Error.call(this, getMessage(code, error) || message);
            err.code = code;
            err.exception = error;
            return err;
          }
          XPathException2.prototype = Object.create(Error.prototype);
          XPathException2.prototype.constructor = XPathException2;
          XPathException2.superclass = Error;
          XPathException2.prototype.toString = function() {
            return this.message;
          };
          XPathException2.fromMessage = function(message, error) {
            return new XPathException2(null, error, message);
          };
          XPathException2.INVALID_EXPRESSION_ERR = 51;
          XPathException2.TYPE_ERR = 52;
          return XPathException2;
        }();
        XPathExpression.prototype = {};
        XPathExpression.prototype.constructor = XPathExpression;
        XPathExpression.superclass = Object.prototype;
        function XPathExpression(e, r, p) {
          this.xpath = p.parse(e);
          this.context = new XPathContext();
          this.context.namespaceResolver = new XPathNSResolverWrapper(r);
        }
        XPathExpression.getOwnerDocument = function(n) {
          return n.nodeType === NodeTypes.DOCUMENT_NODE ? n : n.ownerDocument;
        };
        XPathExpression.detectHtmlDom = function(n) {
          if (!n) {
            return false;
          }
          var doc = XPathExpression.getOwnerDocument(n);
          try {
            return doc.implementation.hasFeature("HTML", "2.0");
          } catch (e) {
            return true;
          }
        };
        XPathExpression.prototype.evaluate = function(n, t, res) {
          this.context.expressionContextNode = n;
          this.context.caseInsensitive = XPathExpression.detectHtmlDom(n);
          var result = this.xpath.evaluate(this.context);
          return new XPathResult(result, t);
        };
        XPathNSResolverWrapper.prototype = {};
        XPathNSResolverWrapper.prototype.constructor = XPathNSResolverWrapper;
        XPathNSResolverWrapper.superclass = Object.prototype;
        function XPathNSResolverWrapper(r) {
          this.xpathNSResolver = r;
        }
        XPathNSResolverWrapper.prototype.getNamespace = function(prefix, n) {
          if (this.xpathNSResolver == null) {
            return null;
          }
          return this.xpathNSResolver.lookupNamespaceURI(prefix);
        };
        NodeXPathNSResolver.prototype = {};
        NodeXPathNSResolver.prototype.constructor = NodeXPathNSResolver;
        NodeXPathNSResolver.superclass = Object.prototype;
        function NodeXPathNSResolver(n) {
          this.node = n;
          this.namespaceResolver = new NamespaceResolver();
        }
        NodeXPathNSResolver.prototype.lookupNamespaceURI = function(prefix) {
          return this.namespaceResolver.getNamespace(prefix, this.node);
        };
        XPathResult.prototype = {};
        XPathResult.prototype.constructor = XPathResult;
        XPathResult.superclass = Object.prototype;
        function XPathResult(v, t) {
          if (t == XPathResult.ANY_TYPE) {
            if (v.constructor === XString) {
              t = XPathResult.STRING_TYPE;
            } else if (v.constructor === XNumber) {
              t = XPathResult.NUMBER_TYPE;
            } else if (v.constructor === XBoolean) {
              t = XPathResult.BOOLEAN_TYPE;
            } else if (v.constructor === XNodeSet) {
              t = XPathResult.UNORDERED_NODE_ITERATOR_TYPE;
            }
          }
          this.resultType = t;
          switch (t) {
            case XPathResult.NUMBER_TYPE:
              this.numberValue = v.numberValue();
              return;
            case XPathResult.STRING_TYPE:
              this.stringValue = v.stringValue();
              return;
            case XPathResult.BOOLEAN_TYPE:
              this.booleanValue = v.booleanValue();
              return;
            case XPathResult.ANY_UNORDERED_NODE_TYPE:
            case XPathResult.FIRST_ORDERED_NODE_TYPE:
              if (v.constructor === XNodeSet) {
                this.singleNodeValue = v.first();
                return;
              }
              break;
            case XPathResult.UNORDERED_NODE_ITERATOR_TYPE:
            case XPathResult.ORDERED_NODE_ITERATOR_TYPE:
              if (v.constructor === XNodeSet) {
                this.invalidIteratorState = false;
                this.nodes = v.toArray();
                this.iteratorIndex = 0;
                return;
              }
              break;
            case XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE:
            case XPathResult.ORDERED_NODE_SNAPSHOT_TYPE:
              if (v.constructor === XNodeSet) {
                this.nodes = v.toArray();
                this.snapshotLength = this.nodes.length;
                return;
              }
              break;
          }
          throw new XPathException(XPathException.TYPE_ERR);
        }
        ;
        XPathResult.prototype.iterateNext = function() {
          if (this.resultType != XPathResult.UNORDERED_NODE_ITERATOR_TYPE && this.resultType != XPathResult.ORDERED_NODE_ITERATOR_TYPE) {
            throw new XPathException(XPathException.TYPE_ERR);
          }
          return this.nodes[this.iteratorIndex++];
        };
        XPathResult.prototype.snapshotItem = function(i) {
          if (this.resultType != XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE && this.resultType != XPathResult.ORDERED_NODE_SNAPSHOT_TYPE) {
            throw new XPathException(XPathException.TYPE_ERR);
          }
          return this.nodes[i];
        };
        XPathResult.ANY_TYPE = 0;
        XPathResult.NUMBER_TYPE = 1;
        XPathResult.STRING_TYPE = 2;
        XPathResult.BOOLEAN_TYPE = 3;
        XPathResult.UNORDERED_NODE_ITERATOR_TYPE = 4;
        XPathResult.ORDERED_NODE_ITERATOR_TYPE = 5;
        XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE = 6;
        XPathResult.ORDERED_NODE_SNAPSHOT_TYPE = 7;
        XPathResult.ANY_UNORDERED_NODE_TYPE = 8;
        XPathResult.FIRST_ORDERED_NODE_TYPE = 9;
        function installDOM3XPathSupport(doc, p) {
          doc.createExpression = function(e, r) {
            try {
              return new XPathExpression(e, r, p);
            } catch (e2) {
              throw new XPathException(XPathException.INVALID_EXPRESSION_ERR, e2);
            }
          };
          doc.createNSResolver = function(n) {
            return new NodeXPathNSResolver(n);
          };
          doc.evaluate = function(e, cn, r, t, res) {
            if (t < 0 || t > 9) {
              throw { code: 0, toString: function() {
                return "Request type not supported";
              } };
            }
            return doc.createExpression(e, r, p).evaluate(cn, t, res);
          };
        }
        ;
        try {
          var shouldInstall = true;
          try {
            if (document.implementation && document.implementation.hasFeature && document.implementation.hasFeature("XPath", null)) {
              shouldInstall = false;
            }
          } catch (e) {
          }
          if (shouldInstall) {
            installDOM3XPathSupport(document, new XPathParser());
          }
        } catch (e) {
        }
        installDOM3XPathSupport(exports2, new XPathParser());
        (function() {
          var parser = new XPathParser();
          var defaultNSResolver = new NamespaceResolver();
          var defaultFunctionResolver = new FunctionResolver();
          var defaultVariableResolver = new VariableResolver();
          function makeNSResolverFromFunction(func) {
            return {
              getNamespace: function(prefix, node) {
                var ns = func(prefix, node);
                return ns || defaultNSResolver.getNamespace(prefix, node);
              }
            };
          }
          function makeNSResolverFromObject(obj) {
            return makeNSResolverFromFunction(obj.getNamespace.bind(obj));
          }
          function makeNSResolverFromMap(map2) {
            return makeNSResolverFromFunction(function(prefix) {
              return map2[prefix];
            });
          }
          function makeNSResolver(resolver) {
            if (resolver && typeof resolver.getNamespace === "function") {
              return makeNSResolverFromObject(resolver);
            }
            if (typeof resolver === "function") {
              return makeNSResolverFromFunction(resolver);
            }
            if (typeof resolver === "object") {
              return makeNSResolverFromMap(resolver);
            }
            return defaultNSResolver;
          }
          function convertValue(value) {
            if (value === null || typeof value === "undefined" || value instanceof XString || value instanceof XBoolean || value instanceof XNumber || value instanceof XNodeSet) {
              return value;
            }
            switch (typeof value) {
              case "string":
                return new XString(value);
              case "boolean":
                return new XBoolean(value);
              case "number":
                return new XNumber(value);
            }
            var ns = new XNodeSet();
            ns.addArray([].concat(value));
            return ns;
          }
          function makeEvaluator(func) {
            return function(context) {
              var args = Array.prototype.slice.call(arguments, 1).map(function(arg) {
                return arg.evaluate(context);
              });
              var result = func.apply(this, [].concat(context, args));
              return convertValue(result);
            };
          }
          function makeFunctionResolverFromFunction(func) {
            return {
              getFunction: function(name2, namespace) {
                var found = func(name2, namespace);
                if (found) {
                  return makeEvaluator(found);
                }
                return defaultFunctionResolver.getFunction(name2, namespace);
              }
            };
          }
          function makeFunctionResolverFromObject(obj) {
            return makeFunctionResolverFromFunction(obj.getFunction.bind(obj));
          }
          function makeFunctionResolverFromMap(map2) {
            return makeFunctionResolverFromFunction(function(name2) {
              return map2[name2];
            });
          }
          function makeFunctionResolver(resolver) {
            if (resolver && typeof resolver.getFunction === "function") {
              return makeFunctionResolverFromObject(resolver);
            }
            if (typeof resolver === "function") {
              return makeFunctionResolverFromFunction(resolver);
            }
            if (typeof resolver === "object") {
              return makeFunctionResolverFromMap(resolver);
            }
            return defaultFunctionResolver;
          }
          function makeVariableResolverFromFunction(func) {
            return {
              getVariable: function(name2, namespace) {
                var value = func(name2, namespace);
                return convertValue(value);
              }
            };
          }
          function makeVariableResolver(resolver) {
            if (resolver) {
              if (typeof resolver.getVariable === "function") {
                return makeVariableResolverFromFunction(resolver.getVariable.bind(resolver));
              }
              if (typeof resolver === "function") {
                return makeVariableResolverFromFunction(resolver);
              }
              if (typeof resolver === "object") {
                return makeVariableResolverFromFunction(function(name2) {
                  return resolver[name2];
                });
              }
            }
            return defaultVariableResolver;
          }
          function copyIfPresent(prop, dest, source) {
            if (prop in source) {
              dest[prop] = source[prop];
            }
          }
          function makeContext(options) {
            var context = new XPathContext();
            if (options) {
              context.namespaceResolver = makeNSResolver(options.namespaces);
              context.functionResolver = makeFunctionResolver(options.functions);
              context.variableResolver = makeVariableResolver(options.variables);
              context.expressionContextNode = options.node;
              copyIfPresent("allowAnyNamespaceForNoPrefix", context, options);
              copyIfPresent("isHtml", context, options);
            } else {
              context.namespaceResolver = defaultNSResolver;
            }
            return context;
          }
          function evaluate(parsedExpression, options) {
            var context = makeContext(options);
            return parsedExpression.evaluate(context);
          }
          var evaluatorPrototype = {
            evaluate: function(options) {
              return evaluate(this.expression, options);
            },
            evaluateNumber: function(options) {
              return this.evaluate(options).numberValue();
            },
            evaluateString: function(options) {
              return this.evaluate(options).stringValue();
            },
            evaluateBoolean: function(options) {
              return this.evaluate(options).booleanValue();
            },
            evaluateNodeSet: function(options) {
              return this.evaluate(options).nodeset();
            },
            select: function(options) {
              return this.evaluateNodeSet(options).toArray();
            },
            select1: function(options) {
              return this.select(options)[0];
            }
          };
          function parse(xpath3) {
            var parsed = parser.parse(xpath3);
            return Object.create(evaluatorPrototype, {
              expression: {
                value: parsed
              }
            });
          }
          exports2.parse = parse;
        })();
        assign(
          exports2,
          {
            XPath,
            XPathParser,
            XPathResult,
            Step,
            PathExpr,
            NodeTest,
            LocationPath,
            OrOperation,
            AndOperation,
            BarOperation,
            EqualsOperation,
            NotEqualOperation,
            LessThanOperation,
            GreaterThanOperation,
            LessThanOrEqualOperation,
            GreaterThanOrEqualOperation,
            PlusOperation,
            MinusOperation,
            MultiplyOperation,
            DivOperation,
            ModOperation,
            UnaryMinusOperation,
            FunctionCall,
            VariableReference,
            XPathContext,
            XNodeSet,
            XBoolean,
            XString,
            XNumber,
            NamespaceResolver,
            FunctionResolver,
            VariableResolver,
            Utilities
          }
        );
        exports2.select = function(e, doc, single) {
          return exports2.selectWithResolver(e, doc, null, single);
        };
        exports2.useNamespaces = function(mappings) {
          var resolver = {
            mappings: mappings || {},
            lookupNamespaceURI: function(prefix) {
              return this.mappings[prefix];
            }
          };
          return function(e, doc, single) {
            return exports2.selectWithResolver(e, doc, resolver, single);
          };
        };
        exports2.selectWithResolver = function(e, doc, resolver, single) {
          var expression = new XPathExpression(e, resolver, new XPathParser());
          var type = XPathResult.ANY_TYPE;
          var result = expression.evaluate(doc, type, null);
          if (result.resultType == XPathResult.STRING_TYPE) {
            result = result.stringValue;
          } else if (result.resultType == XPathResult.NUMBER_TYPE) {
            result = result.numberValue;
          } else if (result.resultType == XPathResult.BOOLEAN_TYPE) {
            result = result.booleanValue;
          } else {
            result = result.nodes;
            if (single) {
              result = result[0];
            }
          }
          return result;
        };
        exports2.select1 = function(e, doc) {
          return exports2.select(e, doc, true);
        };
        var isArrayOfNodes = function(value) {
          return Array.isArray(value) && value.every(isNodeLike);
        };
        var isNodeOfType = function(type) {
          return function(value) {
            return isNodeLike(value) && value.nodeType === type;
          };
        };
        assign(
          exports2,
          {
            isNodeLike,
            isArrayOfNodes,
            isElement: isNodeOfType(NodeTypes.ELEMENT_NODE),
            isAttribute: isNodeOfType(NodeTypes.ATTRIBUTE_NODE),
            isTextNode: isNodeOfType(NodeTypes.TEXT_NODE),
            isCDATASection: isNodeOfType(NodeTypes.CDATA_SECTION_NODE),
            isProcessingInstruction: isNodeOfType(NodeTypes.PROCESSING_INSTRUCTION_NODE),
            isComment: isNodeOfType(NodeTypes.COMMENT_NODE),
            isDocumentNode: isNodeOfType(NodeTypes.DOCUMENT_NODE),
            isDocumentTypeNode: isNodeOfType(NodeTypes.DOCUMENT_TYPE_NODE),
            isDocumentFragment: isNodeOfType(NodeTypes.DOCUMENT_FRAGMENT_NODE)
          }
        );
      })(xpath2);
    }
  });

  // src/index.ts
  var index_exports = {};
  __export(index_exports, {
    AppError: () => AppError,
    Duration: () => Duration,
    Generator: () => Generator,
    GeneratorHostDataKeys: () => GeneratorHostDataKeys,
    HostEnvironment: () => HostEnvironment,
    LhqCodeGenVersionSchema: () => LhqCodeGenVersionSchema,
    LhqModelCategoriesCollectionSchema: () => LhqModelCategoriesCollectionSchema,
    LhqModelCategorySchema: () => LhqModelCategorySchema,
    LhqModelDataNodeSchema: () => LhqModelDataNodeSchema,
    LhqModelLineEndingsSchema: () => LhqModelLineEndingsSchema,
    LhqModelMetadataSchema: () => LhqModelMetadataSchema,
    LhqModelOptionsResourcesSchema: () => LhqModelOptionsResourcesSchema,
    LhqModelOptionsSchema: () => LhqModelOptionsSchema,
    LhqModelResourceParameterSchema: () => LhqModelResourceParameterSchema,
    LhqModelResourceSchema: () => LhqModelResourceSchema,
    LhqModelResourceSchemaBase: () => LhqModelResourceSchemaBase,
    LhqModelResourceTranslationStateSchema: () => LhqModelResourceTranslationStateSchema,
    LhqModelResourceValueSchema: () => LhqModelResourceValueSchema,
    LhqModelResourcesCollectionSchema: () => LhqModelResourcesCollectionSchema,
    LhqModelSchema: () => LhqModelSchema,
    LhqModelUidSchema: () => LhqModelUidSchema,
    LhqModelVersionSchema: () => LhqModelVersionSchema,
    arraySortBy: () => arraySortBy,
    baseCategorySchema: () => baseCategorySchema,
    baseDataNodeSchema: () => baseDataNodeSchema,
    generatorUtils: () => generatorUtils_exports,
    getLibraryVersion: () => getLibraryVersion,
    hasItems: () => hasItems,
    isNullOrEmpty: () => isNullOrEmpty,
    isNullOrUndefined: () => isNullOrUndefined,
    iterateObject: () => iterateObject,
    jsonParseOrDefault: () => jsonParseOrDefault,
    jsonQuery: () => jsonQuery,
    normalizePath: () => normalizePath,
    objCount: () => objCount,
    removeNewLines: () => removeNewLines,
    removeProperties: () => removeProperties,
    sortBy: () => sortBy,
    sortObjectByKey: () => sortObjectByKey,
    sortObjectByValue: () => sortObjectByValue,
    textEncode: () => textEncode,
    tryJsonParse: () => tryJsonParse,
    tryRemoveBOM: () => tryRemoveBOM,
    valueOrDefault: () => valueOrDefault
  });

  // src/AppError.ts
  var AppError = class _AppError extends Error {
    constructor(message, stack) {
      super(message);
      this.message = message;
      this.name = "AppError";
      if (Error.captureStackTrace) {
        Error.captureStackTrace(this, _AppError);
      }
      if (stack !== void 0 && stack !== null) {
        this.stack = stack;
      }
    }
  };

  // src/duration.ts
  var Duration = class _Duration {
    constructor() {
      this._start = Date.now();
    }
    static start() {
      return new _Duration();
    }
    end() {
      this._end = Date.now();
    }
    get elapsed() {
      var _a;
      const now = (_a = this._end) != null ? _a : Date.now();
      return now - this._start;
    }
    get elapsedTime() {
      return _Duration.formatDuration(this.elapsed);
    }
    static formatDuration(ms) {
      const seconds = Math.floor(ms / 1e3);
      const milliseconds = ms % 1e3;
      return seconds > 0 ? `${seconds} second${seconds > 1 ? "s" : ""} and ${milliseconds} ms` : `${milliseconds} ms`;
    }
  };

  // src/utils.ts
  var import_jmespath = __toESM(require_jmespath());
  var encodingCharMaps = {
    html: {
      ">": "&gt;",
      "<": "&lt;",
      '"': "&quot;",
      "'": "&apos;",
      "&": "&amp;"
    },
    xml: {
      ">": "&gt;",
      "<": "&lt;",
      "&": "&amp;"
    },
    xml_quotes: {
      ">": "&gt;",
      "<": "&lt;",
      '"': "&quot;",
      "'": "&apos;",
      "&": "&amp;"
    },
    json: {
      "\\": "\\\\",
      '"': '\\"',
      "\b": "\\b",
      "\f": "\\f",
      "\n": "\\n",
      "\r": "\\r",
      "	": "\\t"
    }
  };
  function tryRemoveBOM(value) {
    return isNullOrEmpty(value) ? value : value.charCodeAt(0) === 65279 ? value.slice(1) : value;
  }
  function jsonParseOrDefault(value, defaultValue, removeBOM = false) {
    const { success, data: result } = tryJsonParse(value, removeBOM);
    return success ? result != null ? result : defaultValue : defaultValue;
  }
  function tryJsonParse(value, removeBOM = false) {
    if (removeBOM) {
      value = tryRemoveBOM(value);
    }
    let result;
    let success = false;
    let error = void 0;
    if (!isNullOrEmpty(value)) {
      try {
        result = JSON.parse(value);
        success = !isNullOrEmpty(result);
      } catch (e) {
        if (!isNullOrEmpty(e)) {
          error = e instanceof Error ? e.message : String(e);
        } else {
          error = "Unknown error occurred";
        }
      }
    }
    return { success, data: result, error };
  }
  function jsonQuery(obj, query, defaultValue) {
    var _a;
    return (_a = (0, import_jmespath.search)(obj, query)) != null ? _a : defaultValue;
  }
  function normalizePath(path) {
    return path.replace(/\\/g, "/").replace(/\/\//g, "/").replace(/[\\/]$/g, "");
  }
  function isNullOrEmpty(value) {
    return value === null || value === void 0 || value === "";
  }
  function isNullOrUndefined(value) {
    return value === null || value === void 0;
  }
  function sortObjectByKey(obj, sortOrder = "asc", locales) {
    locales = locales != null ? locales : "en";
    return Object.fromEntries(
      Object.entries(obj).sort(
        ([a], [b]) => sortOrder === "asc" ? a.localeCompare(b, locales) : b.localeCompare(a, locales)
      )
    );
  }
  function sortObjectByValue(obj, predicate, sortOrder = "asc") {
    return Object.fromEntries(Object.entries(obj).sort(([, a], [, b]) => {
      const aValue = predicate(a);
      const bValue = predicate(b);
      if (typeof aValue === "number" || typeof bValue === "number") {
        let res = 0;
        if (aValue > bValue) {
          res = 1;
        } else if (bValue > aValue) {
          res = -1;
        }
        return sortOrder === "asc" ? res : res * -1;
      }
      if (typeof aValue === "string" && typeof bValue === "string") {
        return sortOrder === "asc" ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
      }
      return 0;
    }));
  }
  function sortBy(source, key, sortOrder = "asc") {
    return arraySortBy(source, (x) => key === void 0 ? x : x[key], sortOrder);
  }
  function arraySortBy(source, predicate, sortOrder = "asc") {
    return source.concat([]).sort((a, b) => {
      const v1 = predicate(a);
      const v2 = predicate(b);
      const res = v1 > v2 ? 1 : v2 > v1 ? -1 : 0;
      return sortOrder === "asc" ? res : res * -1;
    });
  }
  function iterateObject(obj, callback) {
    const entries = Object.entries(obj);
    if (entries.length > 0) {
      const lastIndex = entries.length - 1;
      let index = -1;
      for (const [key, value] of entries) {
        index++;
        callback(value, key, index, index == lastIndex);
      }
    }
  }
  function textEncode(str, encoder) {
    var _a;
    if (isNullOrEmpty(str)) {
      return str;
    }
    const encodedChars = [];
    for (let i = 0; i < str.length; i++) {
      const ch = str.charAt(i);
      let map = void 0;
      if (encoder.mode === "html") {
        map = encodingCharMaps.html;
      } else if (encoder.mode === "xml") {
        map = ((_a = encoder.quotes) != null ? _a : true) ? encodingCharMaps.xml_quotes : encodingCharMaps.xml;
      } else {
        map = encodingCharMaps.json;
      }
      if (Object.prototype.hasOwnProperty.call(map, ch)) {
        encodedChars.push(map[ch]);
      } else {
        encodedChars.push(ch);
      }
    }
    return encodedChars.join("");
  }
  function valueOrDefault(value, defaultValue, defaultOnEmptyString = false) {
    defaultOnEmptyString = defaultOnEmptyString != null ? defaultOnEmptyString : false;
    return defaultOnEmptyString ? isNullOrEmpty(value) ? defaultValue : value : isNullOrUndefined(value) ? defaultValue : value;
  }
  function hasItems(obj) {
    return objCount(obj) > 0;
  }
  function objCount(obj) {
    if (isNullOrEmpty(obj)) {
      return 0;
    }
    if (Array.isArray(obj)) {
      return obj.length;
    }
    if (typeof obj === "object") {
      return Object.keys(obj).length;
    }
    return 0;
  }
  function removeNewLines(input) {
    if (isNullOrEmpty(input)) return input;
    return input.replace(/\r\n|\r|\n/g, "");
  }
  function removeProperties(obj, ...propertiesToRemove) {
    if (isNullOrUndefined(obj)) return obj;
    propertiesToRemove.forEach((propObj) => {
      for (const key in propObj) {
        if (Object.prototype.hasOwnProperty.call(obj, key)) {
          delete obj[key];
        }
      }
    });
    return obj;
  }

  // src/hostEnv.ts
  var HostEnvironment = class {
    debugLog(msg) {
      console.log(msg);
    }
    pathCombine(path1, path2) {
      return `${path1 != null ? path1 : ""}/` + (path2 != null ? path2 : "");
    }
    webHtmlEncode(input) {
      return textEncode(input, { mode: "html" });
    }
    stopwatchStart() {
      return performance.now();
    }
    stopwatchEnd(start) {
      return `${(performance.now() - start).toFixed(2)}ms`;
    }
  };

  // src/model/treeElementPaths.ts
  var TreeElementPaths = class _TreeElementPaths {
    constructor(element) {
      this.paths = [];
      this.getParentPath = (separator, includeRoot) => {
        separator != null ? separator : separator = "";
        includeRoot != null ? includeRoot : includeRoot = false;
        return includeRoot || this.paths.length === 1 ? this.paths.join(separator) : this.paths.slice(1).join(separator);
      };
      if (element.parent) {
        const parentPaths = element.parent.paths;
        if (parentPaths instanceof _TreeElementPaths) {
          this.paths = [...parentPaths.paths];
        }
      }
      this.paths.push(element.name);
    }
  };

  // src/model/treeElement.ts
  var TreeElement = class {
    constructor(root, elementType, name2, description, parent) {
      this.addToTempData = (key, value) => {
        this._data[key] = value;
      };
      this.clearTempData = () => {
        this._data = {};
      };
      this._name = name2 != null ? name2 : "";
      this._elementType = elementType;
      this._description = description != null ? description : "";
      this._root = isNullOrEmpty(root) && isNullOrEmpty(parent) ? this : root;
      this._parent = parent;
      this._paths = new TreeElementPaths(this);
      this._isRoot = isNullOrEmpty(this.parent);
      this._data = {};
    }
    get isRoot() {
      return this._isRoot;
    }
    get root() {
      return this._root;
    }
    get parent() {
      return this._parent;
    }
    get data() {
      return this._data;
    }
    get name() {
      return this._name;
    }
    get elementType() {
      return this._elementType;
    }
    get description() {
      return this._description;
    }
    get paths() {
      return this._paths;
    }
  };

  // src/model/categoryLikeTreeElement.ts
  var CategoryLikeTreeElement = class extends TreeElement {
    constructor(root, elementType, name2, description, parent) {
      super(root, elementType, name2, description, parent);
      this._categories = [];
      this._resources = [];
      this._hasCategories = false;
      this._hasResources = false;
    }
    populate(sourceCategories, sourceResources) {
      const newCategories = [];
      const newResources = [];
      if (!isNullOrEmpty(sourceCategories)) {
        iterateObject(sortObjectByKey(sourceCategories), (category, name2) => {
          const newCategory = this.createCategory(this.root, name2, category, this);
          newCategories.push(newCategory);
          newCategory.populate(category.categories, category.resources);
        });
      }
      if (!isNullOrEmpty(sourceResources)) {
        iterateObject(sortObjectByKey(sourceResources), (resource, name2) => {
          const newResource = this.createResource(this.root, name2, resource, this);
          newResources.push(newResource);
        });
      }
      this._categories = newCategories;
      this._hasCategories = this.categories.length > 0;
      this._resources = newResources;
      this._hasResources = this.resources.length > 0;
    }
    get categories() {
      return this._categories;
    }
    get resources() {
      return this._resources;
    }
    get hasCategories() {
      return this._hasCategories;
    }
    get hasResources() {
      return this._hasResources;
    }
  };

  // src/model/modelConst.ts
  var ModelVersions = Object.freeze({
    model: 2,
    codeGenerator: 1
  });
  var DefaultLineEndings = "LF";
  var DefaultCodeGenSettings = {
    OutputFolder: "Resources",
    EncodingWithBOM: false,
    LineEndings: DefaultLineEndings,
    Enabled: true
  };

  // src/model/resourceParameterElement.ts
  var ResourceParameterElement = class {
    constructor(name2, source, parent) {
      var _a;
      this._name = name2;
      this._description = source.description;
      this._order = (_a = source.order) != null ? _a : 0;
      this._parent = parent;
    }
    get name() {
      return this._name;
    }
    get parent() {
      return this._parent;
    }
    get description() {
      return this._description;
    }
    get order() {
      return this._order;
    }
  };

  // src/model/resourceValueElement.ts
  var ResourceValueElement = class {
    constructor(languageName, source, parent) {
      this._languageName = languageName;
      this._value = source.value;
      this._locked = source.locked;
      this._auto = source.auto;
      this._parent = parent;
    }
    get languageName() {
      return this._languageName;
    }
    get value() {
      return this._value;
    }
    get locked() {
      return this._locked;
    }
    get auto() {
      return this._auto;
    }
    get parent() {
      return this._parent;
    }
  };

  // src/model/resourceElement.ts
  var ResourceElement = class extends TreeElement {
    constructor(root, name2, source, parent) {
      super(root, "resource", name2, source.description, parent);
      this.getComment = () => {
        var _a, _b;
        const root = this.root;
        const primaryLanguage = (_a = root.primaryLanguage) != null ? _a : "";
        if (!isNullOrEmpty(primaryLanguage) && this.values) {
          const value = this.values.find((x) => x.languageName === primaryLanguage);
          const resourceValue = value == null ? void 0 : value.value;
          const propertyComment = (_b = isNullOrEmpty(resourceValue) ? this.description : resourceValue) != null ? _b : "";
          return this.trimComment(propertyComment);
        }
        return "";
      };
      this.getValue = (language, trim) => {
        var _a;
        let result = "";
        if (!isNullOrEmpty(language) && this.values) {
          const value = this.values.find((x) => x.languageName === language);
          result = (_a = value == null ? void 0 : value.value) != null ? _a : "";
        }
        return trim === true ? result.trim() : result;
      };
      this.hasValue = (language) => {
        if (!isNullOrEmpty(language) && this.values) {
          const value = this.values.find((x) => x.languageName === language);
          return !isNullOrEmpty(value);
        }
        return false;
      };
      this._state = source.state;
      this._parameters = [];
      if (!isNullOrEmpty(source.parameters)) {
        iterateObject(sortObjectByValue(source.parameters, (x) => x.order), (parameter, name3) => {
          this._parameters.push(new ResourceParameterElement(name3, parameter, this));
        });
      }
      this._hasParameters = this.parameters.length > 0;
      this._values = [];
      if (!isNullOrEmpty(source.values)) {
        iterateObject(sortObjectByKey(source.values), (resValue, name3) => {
          this._values.push(new ResourceValueElement(name3, resValue, this));
        });
      }
      this._hasValues = this.values.length > 0;
      this._comment = this.getComment();
    }
    get hasParameters() {
      return this._hasParameters;
    }
    get hasValues() {
      return this._hasValues;
    }
    get comment() {
      return this._comment;
    }
    trimComment(value) {
      if (isNullOrEmpty(value)) {
        return "";
      }
      let trimmed = false;
      let idxNewLine = value.indexOf("\r\n");
      if (idxNewLine == -1) {
        idxNewLine = value.indexOf("\n");
      }
      if (idxNewLine == -1) {
        idxNewLine = value.indexOf("\r");
      }
      if (idxNewLine > -1) {
        value = value.substring(0, idxNewLine);
        trimmed = true;
      }
      if (value.length > 80) {
        value = value.substring(0, 80);
        trimmed = true;
      }
      if (trimmed) {
        value += "...";
      }
      return value.replace(/\t/g, " ");
    }
    get state() {
      return this._state;
    }
    get parameters() {
      return this._parameters;
    }
    get values() {
      return this._values;
    }
  };

  // src/model/categoryElement.ts
  var CategoryElement = class _CategoryElement extends CategoryLikeTreeElement {
    constructor(root, name2, source, parent) {
      super(root, "category", name2, source == null ? void 0 : source.description, parent);
    }
    createCategory(root, name2, source, parent) {
      return new _CategoryElement(root, name2, source, parent);
    }
    createResource(root, name2, source, parent) {
      return new ResourceElement(root, name2, source, parent);
    }
  };

  // src/model/rootModelElement.ts
  var CodeGenUID = "b40c8a1d-23b7-4f78-991b-c24898596dd2";
  var RootModelElement = class extends CategoryLikeTreeElement {
    constructor(model) {
      var _a, _b;
      super(void 0, "model", (_a = model.model.name) != null ? _a : "", (_b = model.model.description) != null ? _b : "", void 0);
      this._uid = model.model.uid;
      this._version = model.model.version;
      this._options = { categories: model.model.options.categories, resources: model.model.options.resources };
      this._primaryLanguage = model.model.primaryLanguage;
      this._languages = Object.freeze([...model.languages]);
      this._hasLanguages = this._languages.length > 0;
      this._metadatas = model.metadatas ? Object.freeze(__spreadValues({}, model.metadatas)) : void 0;
      this._codeGenerator = this.getCodeGenerator(model);
      this.populate(model.categories, model.resources);
    }
    createCategory(root, name2, source, parent) {
      return new CategoryElement(root, name2, source, parent);
    }
    createResource(root, name2, source, parent) {
      return new ResourceElement(root, name2, source, parent);
    }
    getCodeGenerator(model) {
      var _a, _b, _c, _d;
      let templateId = "";
      let codeGenVersion = 1;
      let node = (_b = (_a = model.metadatas) == null ? void 0 : _a.childs) == null ? void 0 : _b.find((x) => {
        var _a2;
        return x.name === "metadata" && ((_a2 = x.attrs) == null ? void 0 : _a2["descriptorUID"]) === CodeGenUID;
      });
      if (node) {
        node = (_c = node.childs) == null ? void 0 : _c.find((x) => {
          var _a2;
          return x.name === "content" && ((_a2 = x.attrs) == null ? void 0 : _a2["templateId"]) !== void 0;
        });
        if (node) {
          templateId = node.attrs["templateId"];
          const version = node.attrs["version"];
          if (!isNullOrEmpty(version)) {
            const versionInt = parseInt(version);
            if (versionInt > 0 && versionInt <= ModelVersions.codeGenerator) {
              codeGenVersion = versionInt;
            }
          }
          node = (_d = node.childs) == null ? void 0 : _d.find((x) => {
            var _a2, _b2;
            return x.name === "Settings" && ((_b2 = (_a2 = x.childs) == null ? void 0 : _a2.length) != null ? _b2 : 0) > 0;
          });
        }
      }
      if (!isNullOrEmpty(templateId) && !isNullOrEmpty(node)) {
        return { templateId, settings: node, version: codeGenVersion };
      }
    }
    get uid() {
      return this._uid;
    }
    get version() {
      return this._version;
    }
    get options() {
      return this._options;
    }
    get primaryLanguage() {
      return this._primaryLanguage;
    }
    get languages() {
      return this._languages;
    }
    get hasLanguages() {
      return this._hasLanguages;
    }
    get metadatas() {
      return this._metadatas;
    }
    get codeGenerator() {
      return this._codeGenerator;
    }
  };

  // src/helpers.ts
  var import_handlebars = __toESM(require_handlebars());

  // src/model/templateRootModel.ts
  var TemplateRootModel = class {
    constructor(model, data, host) {
      this._templateRunType = "root";
      this._childOutputs = [];
      this._inlineOutputs = [];
      this._inlineEvaluating = false;
      this.addToTempData = (key, value) => {
        this._data[key] = value;
      };
      this.clearTempData = () => {
        this._data = {};
      };
      if (isNullOrEmpty(model)) {
        throw new AppError("Missing root model !");
      }
      this._model = model;
      this._data = data != null ? data : {};
      this._host = host != null ? host : {};
    }
    setCurrentTemplateId(templateId) {
      this._currentTemplateId = templateId;
    }
    get currentTemplateId() {
      return this._currentTemplateId;
    }
    setInlineEvaluating(value) {
      let valid = true;
      if (value) {
        if (this._inlineEvaluating) {
          valid = false;
        } else {
          this._inlineEvaluating = true;
        }
      } else {
        this._inlineEvaluating = false;
      }
      return valid;
    }
    setOutput(outputFile) {
      if (isNullOrEmpty(outputFile)) {
        throw new AppError(`Input 'outputFile' could not be empty !`);
      }
      this._output = outputFile;
    }
    addChildOutput(templateId, host) {
      if (this._templateRunType === "child") {
        throw new AppError("Child template could not have other child outputs !");
      }
      this._childOutputs.push({ templateId, host });
    }
    addInlineOutputs(inlineOutput) {
      this._inlineOutputs.push(inlineOutput);
    }
    get inlineEvaluating() {
      return this._inlineEvaluating;
    }
    get childOutputs() {
      return this._childOutputs;
    }
    get inlineOutputs() {
      return this._inlineOutputs;
    }
    get templateRunType() {
      return this._templateRunType;
    }
    setAsChildTemplate(childData) {
      var _a, _b;
      if (this._templateRunType === "root") {
        this._rootHost = Object.freeze(Object.assign({}, (_a = this._host) != null ? _a : {}));
        this._templateRunType = "child";
      }
      this.clearTempData();
      this._inlineOutputs = [];
      this._host = Object.assign({}, (_b = childData.host) != null ? _b : {}, this._rootHost);
      this._output = void 0;
      const recursiveClear = (element) => {
        if (element instanceof TreeElement) {
          element.clearTempData();
        }
        if (element.hasCategories) {
          element.categories.forEach(recursiveClear);
        }
        if (element.hasResources) {
          element.resources.forEach((e) => {
            if (e instanceof TreeElement) {
              e.clearTempData();
            }
          });
        }
      };
      recursiveClear(this.model);
    }
    get output() {
      return this._output;
    }
    /**
     * loaded lhq model file as parsed json structure
     */
    get model() {
      return this._model;
    }
    /**
     * extra data defined dynamically by template run, resets on each template run.
     */
    get data() {
      return this._data;
    }
    get settings() {
      var _a;
      const settings = (_a = this._output) == null ? void 0 : _a.settings;
      if (isNullOrEmpty(settings)) {
        throw new AppError("Missing root output file settings !");
      }
      return settings;
    }
    /*
     * data from host environment, stays for all template runs within the same session.
     */
    get host() {
      return this._host;
    }
  };

  // src/helpers.ts
  var hostEnv = void 0;
  function registerHelpers(hostEnvironment) {
    hostEnv = hostEnvironment;
    Object.keys(helpersList).forEach((key) => {
      import_handlebars.default.registerHelper(key, helpersList[key]);
    });
  }
  var helpersList = {
    // generic helpers
    "x-header": headerHelper,
    "x-normalizePath": normalizePathHelper,
    "char-tab": charHelper,
    "char-quote": charHelper,
    "x-value": valueHelper,
    "x-join": joinHelper,
    "x-split": splitHelper,
    "x-concat": concatHelper,
    "x-replace": replaceHelper,
    "x-trimEnd": trimEndHelper,
    "x-equals": equalsHelper,
    "x-isTrue": isTrueHelper,
    "x-isFalse": isFalseHelper,
    "x-merge": mergeHelper,
    "x-sortBy": sortByHelper,
    "x-sortObject": sortObjectByKeyHelper,
    "x-objCount": objCountHelper,
    "x-hasItems": hasItemsHelper,
    "x-textEncode": textEncodeHelper,
    "x-host-webHtmlEncode": hostWebHtmlEncodeHelper,
    "x-render": renderHelper,
    "x-test": testHelper,
    "x-isNullOrEmpty": isNullOrEmptyHelper,
    "x-isNotNullOrEmpty": isNotNullOrEmptyHelper,
    "x-fn": callFunctionHelper,
    "x-logical": logicalHelper,
    "x-debugLog": debugLogHelper,
    "x-stringify": stringifyHelper,
    "x-toJson": toJsonHelper,
    "x-typeOf": typeOfHelper,
    // model specific helpers
    "m-data": modelDataHelper,
    "output": modelOutputHelper,
    "output-child": modelOutputChildHelper,
    "output-inline": modelOutputInlineHelper
  };
  var _knownHelpers = void 0;
  function getKnownHelpers() {
    if (_knownHelpers === void 0) {
      _knownHelpers = Object.fromEntries(Object.keys(helpersList).map((key) => [key, true]));
    }
    return _knownHelpers;
  }
  var fileHeader = `//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool - Localization HQ Editor.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------`;
  function getContextAndOptions(context, ...args) {
    var _a;
    if (args.length === 1) {
      return {
        context,
        options: args[0]
      };
    }
    return {
      context: (_a = args[0]) != null ? _a : context,
      options: args[1]
    };
  }
  function headerHelper(options) {
    var _a, _b;
    return (_b = (_a = getRoot(options).host) == null ? void 0 : _a["fileHeader"]) != null ? _b : fileHeader;
  }
  function normalizePathHelper(context, options) {
    var _a;
    if (typeof context !== "string") {
      return context;
    }
    let result = normalizePath(context);
    const replacePathSep = valueOrDefault((_a = options.hash) == null ? void 0 : _a.replacePathSep, "");
    if (!isNullOrEmpty(replacePathSep)) {
      result = result.split("/").join(replacePathSep);
    }
    return result;
  }
  function queryObjValue(context, options, flags) {
    var _a, _b, _c, _d, _e;
    const undefinedForDefault = (_a = flags == null ? void 0 : flags.undefinedForDefault) != null ? _a : false;
    const allowHash = (_b = flags == null ? void 0 : flags.allowHash) != null ? _b : true;
    const allowFn = (_c = flags == null ? void 0 : flags.allowFn) != null ? _c : true;
    let value = undefinedForDefault ? void 0 : context;
    let query = allowHash ? (_d = options == null ? void 0 : options.hash) == null ? void 0 : _d.query : void 0;
    if (typeof (options == null ? void 0 : options.fn) === "function" && allowFn) {
      query = options.fn(context);
      if (!isNullOrEmpty(query) && typeof query === "string") {
        query = removeNewLines(query);
      }
    }
    if (!isNullOrEmpty(query) && typeof query === "string" && !isNullOrEmpty(context)) {
      try {
        value = jsonQuery(context, query);
      } catch (e) {
        const templateId = (_e = getRoot(options).currentTemplateId) != null ? _e : "";
        const loc = options.loc;
        const locText = isNullOrEmpty(loc) ? "" : `starts on ${loc.start.line}:${loc.start.column}, ends: ${loc.end.line}:${loc.end.column}`;
        const msg = `Template: ${templateId}, failed on jmespath query: ${query}
${locText}`;
        throw new Error(msg);
      }
    }
    return value;
  }
  function charHelper(options) {
    var _a;
    const name2 = (_a = options.name) == null ? void 0 : _a.split("-")[1];
    switch (name2) {
      case "tab":
        return "	";
      case "quote":
        return '"';
      default: {
        throw new AppError(`Unknown '${options == null ? void 0 : options.name}' char helper !`);
      }
    }
  }
  function valueHelper() {
    var _a, _b, _c;
    const { context, options } = getContextAndOptions(this, ...arguments);
    const defaultValue = (_a = options.hash) == null ? void 0 : _a.default;
    const defaultOnEmpty = (_c = (_b = options.hash) == null ? void 0 : _b.defaultOnEmpty) != null ? _c : false;
    const result = queryObjValue(context, options);
    return valueOrDefault(result, defaultValue, defaultOnEmpty);
  }
  function splitHelper() {
    var _a;
    const { context, options } = getContextAndOptions(this, ...arguments);
    if (typeof context !== "string") return context;
    const sep = valueOrDefault((_a = options.hash) == null ? void 0 : _a.sep, "");
    return isNullOrEmpty(sep) ? context : context.split(sep);
  }
  function joinHelper(items, options) {
    var _a, _b, _c, _d;
    const separator = valueOrDefault((_a = options.hash) == null ? void 0 : _a.sep, ",");
    const start = valueOrDefault((_b = options.hash) == null ? void 0 : _b.start, 0);
    const len = items ? items.length : 0;
    let end = valueOrDefault((_c = options.hash) == null ? void 0 : _c.end, len);
    const decorator = ((_d = options.hash) == null ? void 0 : _d.decorator) || "";
    if (end > len) end = len;
    return items.map((x) => `${decorator}${x}${decorator}`).slice(start, end).join(separator);
  }
  var concatHelperArgsDefault = {
    sep: "",
    empty: false
  };
  function concatHelper(...args) {
    var _a;
    const options = args.pop();
    const hash = Object.assign({}, concatHelperArgsDefault, (_a = options.hash) != null ? _a : {});
    const array = hash.empty ? args : args.filter((x) => !isNullOrEmpty(x));
    removeProperties(options.hash, hash);
    options.hash = { sep: hash.sep };
    return joinHelper(array, options);
  }
  function replaceHelper(value, options) {
    var _a, _b, _c, _d;
    const what = valueOrDefault((_a = options.hash) == null ? void 0 : _a.what, "");
    const withStr = valueOrDefault((_b = options.hash) == null ? void 0 : _b.with, "");
    const regexopts = valueOrDefault((_c = options.hash) == null ? void 0 : _c.opts, "g");
    const hasOpts = !isNullOrEmpty((_d = options.hash) == null ? void 0 : _d.opts);
    if (isNullOrEmpty(what) || isNullOrEmpty(withStr) || what === withStr && !hasOpts) {
      return value;
    }
    const regex = new RegExp(what, regexopts);
    return value.replace(regex, withStr);
  }
  function trimEndHelper(input, endPattern) {
    try {
      const regex = new RegExp(endPattern + "$");
      return input.replace(regex, "");
    } catch (error) {
      hostEnv.debugLog("Invalid regex pattern:" + endPattern);
      return input;
    }
  }
  function equalsHelper(input, value, options) {
    var _a, _b, _c;
    const cs = (((_a = options.hash) == null ? void 0 : _a.cs) || "true").toString().toLowerCase() == "true";
    const val1 = typeof input === "string" ? input : (_b = input == null ? void 0 : input.toString()) != null ? _b : "";
    const val2 = typeof value === "string" ? value : (_c = value == null ? void 0 : value.toString()) != null ? _c : "";
    return cs ? val1 === val2 : val1.toLowerCase() === val2.toLowerCase();
  }
  function isTrueHelper(input) {
    return input === true;
  }
  function isFalseHelper(input) {
    return input === false;
  }
  function logicalHelper(input, value, options) {
    var _a;
    const op = valueOrDefault((_a = options.hash) == null ? void 0 : _a.op, "and").toLowerCase();
    if (op === "and") {
      return input === true && value === true;
    } else if (op === "or") {
      return input === true || value === true;
    }
    return false;
  }
  function mergeHelper(...args) {
    var _a, _b;
    const options = args.pop();
    const context = args.length === 0 ? this : args.shift();
    if (typeof context !== "object") return context;
    if (isNullOrEmpty(context)) return context;
    Object.assign(context, ...args, (_a = options.hash) != null ? _a : {});
    let result;
    if (typeof (options == null ? void 0 : options.fn) === "function") {
      try {
        result = options.fn(context);
      } finally {
        removeProperties(context, ...args, (_b = options.hash) != null ? _b : {});
      }
    }
    return result;
  }
  function sortByHelper(source, propName, sortOrder = "asc") {
    return sortBy(source, propName, sortOrder);
  }
  function sortObjectByKeyHelper(obj, options) {
    var _a;
    const sortOrder = valueOrDefault((_a = options.hash) == null ? void 0 : _a.sortOrder, "asc");
    return sortObjectByKey(obj, sortOrder);
  }
  function objCountHelper(obj) {
    return objCount(obj);
  }
  function hasItemsHelper(obj) {
    return hasItems(obj);
  }
  function textEncodeHelper(input, options) {
    var _a, _b;
    const mode = valueOrDefault((_a = options == null ? void 0 : options.hash) == null ? void 0 : _a.mode, "html");
    const quotes = valueOrDefault((_b = options == null ? void 0 : options.hash) == null ? void 0 : _b.quotes, false);
    const s = textEncode(input, { mode, quotes });
    return new import_handlebars.default.SafeString(s);
  }
  function hostWebHtmlEncodeHelper(input) {
    if (isNullOrEmpty(input)) {
      return input;
    }
    const encoded = hostEnv.webHtmlEncode(input);
    return new import_handlebars.default.SafeString(encoded);
  }
  function renderHelper(input, options) {
    var _a;
    const when = valueOrDefault((_a = options.hash) == null ? void 0 : _a.when, true);
    return !isNullOrEmpty(when) && (when === true || when === "true") ? input : "";
  }
  function testHelper() {
    var _a, _b;
    const { context, options } = getContextAndOptions(this, ...arguments);
    const condition = context;
    if (isNullOrEmpty(options.hash) || isNullOrEmpty(condition)) {
      return "";
    }
    const then = valueOrDefault((_a = options.hash) == null ? void 0 : _a.then, "");
    const _else = valueOrDefault((_b = options.hash) == null ? void 0 : _b.else, "");
    return condition === true ? then : _else;
  }
  function isNullOrEmptyHelper(value) {
    return isNullOrEmpty(value);
  }
  function isNotNullOrEmptyHelper(input) {
    return !isNullOrEmpty(input);
  }
  function callFunctionHelper(fn2, ...args) {
    let fnArgs = [];
    if (arguments.length > 0) {
      fnArgs = args.slice(0, -1);
    }
    return fnArgs.length === 0 ? fn2() : fn2(...fnArgs);
  }
  function debugLogHelper(...args) {
    hostEnv.debugLog(args.join(" "));
    return "";
  }
  function getRoot(options) {
    if (isNullOrEmpty(options) || isNullOrEmpty(options == null ? void 0 : options.data)) {
      throw new AppError("Template has unknown definition for root data !");
    }
    return options.data["root"];
  }
  function stringifyHelper() {
    var _a, _b;
    const { context, options } = getContextAndOptions(this, ...arguments);
    let space = (_b = (_a = options.hash) == null ? void 0 : _a.space) != null ? _b : void 0;
    if (typeof space === "string") {
      space = space.replace(/\\\\t/gm, "	");
    }
    return new import_handlebars.default.SafeString(JSON.stringify(context, null, space));
  }
  function toJsonHelper(context) {
    if (typeof context === "string") {
      return JSON.parse(context);
    }
    return context;
  }
  function typeOfHelper(context) {
    if (context === void 0) {
      return "undefined";
    }
    if (context === null) {
      return "null";
    }
    if (typeof context === "object") {
      return context.constructor ? context.constructor.name : "object";
    } else {
      return context === void 0 ? "undefined" : `${context}[${typeof context}]`;
    }
  }
  function modelDataHelper() {
    var _a, _b, _c, _d, _e, _f;
    const { context, options } = getContextAndOptions(this, ...arguments);
    const defaultValue = (_a = options.hash) == null ? void 0 : _a.default;
    const defaultOnEmpty = (_c = (_b = options.hash) == null ? void 0 : _b.defaultOnEmpty) != null ? _c : false;
    const value = valueOrDefault(queryObjValue(context, options), defaultValue, defaultOnEmpty);
    const forceToRoot = valueOrDefault((_d = options.hash) == null ? void 0 : _d.root, false);
    const key = (_f = (_e = options == null ? void 0 : options.hash) == null ? void 0 : _e.key) != null ? _f : "";
    if (isNullOrEmpty(key)) {
      throw new AppError(`Helper '${options.name}' missing hash param 'key' !`);
    }
    setCustomData(this, options, value, forceToRoot);
  }
  function setCustomData(context, options, valueOrFn, forceToRoot) {
    var _a;
    const value = typeof valueOrFn === "function" ? valueOrFn() : valueOrFn;
    const key = valueOrDefault((_a = options == null ? void 0 : options.hash) == null ? void 0 : _a.key, "");
    if (!isNullOrEmpty(key)) {
      if (forceToRoot) {
        const root = getRoot(options);
        root.addToTempData(key, value);
      } else if (context instanceof TreeElement) {
        context.addToTempData(key, value);
      } else if (context instanceof TemplateRootModel) {
        context.addToTempData(key, value);
      } else {
        hostEnv.debugLog(`[setCustomData] unknown context: ${typeof context} for key '${key}' !`);
      }
    }
  }
  var modelOutputFlags = { undefinedForDefault: true, allowHash: false };
  function modelOutputHelper() {
    var _a, _b, _c, _d, _e, _f, _g;
    const { context, options } = getContextAndOptions(this, ...arguments);
    if (!(context instanceof TemplateRootModel)) {
      throw new AppError(`Helper '${options.name}' can be used only on TemplateRootModel (@root) type !`);
    }
    if (context.inlineEvaluating) {
      throw new AppError(`Helper '${options.name}' cannot be used as a child helper inside 'output-inline' helper !`);
    }
    if (isNullOrEmpty(options.hash)) {
      throw new AppError(`Helper '${options.name}' missing hash properties !`);
    }
    const fileName = (_b = (_a = options == null ? void 0 : options.hash) == null ? void 0 : _a.fileName) != null ? _b : "";
    const settingsNode = (_c = context.model.codeGenerator) == null ? void 0 : _c.settings;
    const settingsObj = (_e = (_d = options.hash) == null ? void 0 : _d.settings) != null ? _e : queryObjValue(settingsNode, options, modelOutputFlags);
    let outputFile = context.output;
    let updateSettings = true;
    if (outputFile) {
      if (fileName !== void 0) {
        outputFile.fileName = fileName;
        if (settingsObj === void 0) {
          updateSettings = false;
        }
      }
    } else {
      outputFile = {
        fileName,
        settings: void 0
      };
      updateSettings = !isNullOrEmpty(settingsObj);
    }
    if (updateSettings) {
      if (isNullOrEmpty(settingsObj)) {
        throw new AppError(`Helper '${options.name}' must have child content with jmespath query expression to retrieve settings (must be compatible with CodeGeneratorBasicSettings type) !`);
      }
      const mergeWithDefaults = (_g = (_f = options.hash) == null ? void 0 : _f.mergeWithDefaults) != null ? _g : true;
      const settings = Object.assign({}, mergeWithDefaults ? DefaultCodeGenSettings : {}, settingsObj);
      outputFile.settings = settings;
    }
    if (isNullOrEmpty(outputFile.fileName) && isNullOrEmpty(outputFile.settings)) {
      throw new AppError(`Helper '${options.name}' missing hash property 'fileName' or 'settings' or child content with jmespath query expression !`);
    }
    context.setOutput(outputFile);
  }
  function modelOutputChildHelper(options) {
    var _a, _b;
    const context = getRoot(options);
    if (!(context instanceof TemplateRootModel)) {
      throw new AppError(`Helper '${options.name}' can be used only on TemplateRootModel (@root) type !`);
    }
    if (context.inlineEvaluating) {
      throw new AppError(`Helper '${options.name}' cannot be used as a child helper inside 'output-inline' helper !`);
    }
    if (typeof (options == null ? void 0 : options.fn) === "function") {
      throw new AppError(`Helper '${options.name}' cannot be used as block helper (no child content is allowed) !`);
    }
    if (isNullOrEmpty(options.hash)) {
      throw new AppError(`Helper '${options.name}' missing hash properties !`);
    }
    const templateId = (_a = options.hash) == null ? void 0 : _a.templateId;
    if (isNullOrEmpty(templateId)) {
      throw new AppError(`Helper '${options.name}' missing hash property 'templateId' !`);
    }
    context.addChildOutput(templateId, (_b = options.hash) == null ? void 0 : _b.host);
  }
  var modelOutputInlineFlags = { undefinedForDefault: true, allowHash: true, allowFn: false };
  function modelOutputInlineHelper() {
    var _a, _b, _c, _d, _e, _f, _g, _h;
    const { context, options } = getContextAndOptions(this, ...arguments);
    if (!(context instanceof TemplateRootModel)) {
      throw new AppError(`Helper '${options.name}' can be used only on TemplateRootModel (@root) type !`);
    }
    if (arguments.length > 1) {
      throw new AppError(`Helper '${options.name}' can be only use as block helper (value must be child of '${options.name}' begin/end tags) !`);
    }
    const fileName = (_b = (_a = options.hash) == null ? void 0 : _a.fileName) != null ? _b : "";
    if (isNullOrEmpty(fileName)) {
      throw new AppError(`Helper '${options.name}' missing property 'fileName' !`);
    }
    if (typeof (options == null ? void 0 : options.fn) !== "function") {
      throw new AppError(`Helper '${options.name}' can be only use as block helper (value must be child of '${options.name}' begin/end tags) !`);
    }
    const settingsNode = (_c = context.model.codeGenerator) == null ? void 0 : _c.settings;
    let settingsObj = (_e = (_d = options.hash) == null ? void 0 : _d.settings) != null ? _e : queryObjValue(settingsNode, options, modelOutputInlineFlags);
    if (isNullOrEmpty(settingsObj)) {
      settingsObj = getRoot(options).settings;
      if (isNullOrEmpty(settingsObj)) {
        throw new AppError(`Helper '${options.name}' could not find code gen settings from query (nor root settings) !`);
      }
    }
    const mergeWithDefaults = (_g = (_f = options == null ? void 0 : options.hash) == null ? void 0 : _f.mergeWithDefaults) != null ? _g : true;
    const settings = Object.assign({}, mergeWithDefaults ? DefaultCodeGenSettings : {}, settingsObj);
    let fileContent = "";
    if (context.setInlineEvaluating(true)) {
      try {
        fileContent = (_h = options.fn(context)) != null ? _h : "";
      } catch (e) {
        throw new AppError(`Helper '${options.name}' error: ${e.message}`);
      } finally {
        context.setInlineEvaluating(false);
      }
    } else {
      throw new AppError(`Helper '${options.name}' cannot be used as a child helper inside another '${options.name}' helper !`);
    }
    const inlineOutput = {
      fileName,
      settings,
      content: fileContent
    };
    context.addInlineOutputs(inlineOutput);
  }

  // src/hbsManager.ts
  var import_handlebars2 = __toESM(require_handlebars());
  var HbsTemplateManager = class _HbsTemplateManager {
    static init(data) {
      if (isNullOrEmpty(data)) {
        throw new Error("Missing templates data !");
      }
      _HbsTemplateManager._sources = data;
    }
    /* public static registerTemplate(templateId: string, handlebarContent: string): void {
        HbsTemplateManager._sources ??= {};
        HbsTemplateManager._sources[templateId] = handlebarContent;
    } */
    static hasTemplate(templateId) {
      return _HbsTemplateManager._sources.hasOwnProperty(templateId);
    }
    static runTemplate(templateId, data) {
      var _a;
      let compiled;
      (_a = _HbsTemplateManager._compiled) != null ? _a : _HbsTemplateManager._compiled = {};
      if (!_HbsTemplateManager._compiled.hasOwnProperty(templateId)) {
        if (!_HbsTemplateManager._sources.hasOwnProperty(templateId)) {
          const allTemplates = Object.keys(_HbsTemplateManager._sources).join(", ");
          throw new AppError(`Template with id '${templateId}' not found (available templates: ${allTemplates})!`);
        }
        const source = _HbsTemplateManager._sources[templateId];
        compiled = import_handlebars2.default.compile(source, { knownHelpers: getKnownHelpers() });
        _HbsTemplateManager._compiled[templateId] = compiled;
      } else {
        compiled = _HbsTemplateManager._compiled[templateId];
      }
      const result = compiled(data, {
        allowProtoPropertiesByDefault: true,
        allowProtoMethodsByDefault: true,
        allowCallsToHelperMissing: true
      });
      if (result.indexOf("\xA4") > -1) {
        return result.replace(/\t¤$/gm, "");
      }
      return result;
    }
  };

  // src/generatorUtils.ts
  var generatorUtils_exports = {};
  __export(generatorUtils_exports, {
    generateLhqSchema: () => generateLhqSchema,
    getGeneratedFileContent: () => getGeneratedFileContent,
    getRootNamespaceFromCsProj: () => getRootNamespaceFromCsProj,
    validateLhqModel: () => validateLhqModel
  });
  var import_xmldom = __toESM(require_lib());
  var xpath = __toESM(require_xpath());

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/Options.js
  var ignoreOverride = Symbol("Let zodToJsonSchema decide on which parser to use");
  var defaultOptions = {
    name: void 0,
    $refStrategy: "root",
    basePath: ["#"],
    effectStrategy: "input",
    pipeStrategy: "all",
    dateStrategy: "format:date-time",
    mapStrategy: "entries",
    removeAdditionalStrategy: "passthrough",
    allowedAdditionalProperties: true,
    rejectedAdditionalProperties: false,
    definitionPath: "definitions",
    target: "jsonSchema7",
    strictUnions: false,
    definitions: {},
    errorMessages: false,
    markdownDescription: false,
    patternStrategy: "escape",
    applyRegexFlags: false,
    emailStrategy: "format:email",
    base64Strategy: "contentEncoding:base64",
    nameStrategy: "ref"
  };
  var getDefaultOptions = (options) => typeof options === "string" ? __spreadProps(__spreadValues({}, defaultOptions), {
    name: options
  }) : __spreadValues(__spreadValues({}, defaultOptions), options);

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/Refs.js
  var getRefs = (options) => {
    const _options = getDefaultOptions(options);
    const currentPath = _options.name !== void 0 ? [..._options.basePath, _options.definitionPath, _options.name] : _options.basePath;
    return __spreadProps(__spreadValues({}, _options), {
      currentPath,
      propertyPath: void 0,
      seen: new Map(Object.entries(_options.definitions).map(([name2, def]) => [
        def._def,
        {
          def: def._def,
          path: [..._options.basePath, _options.definitionPath, name2],
          // Resolution of references will be forced even though seen, so it's ok that the schema is undefined here for now.
          jsonSchema: void 0
        }
      ]))
    });
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/errorMessages.js
  function addErrorMessage(res, key, errorMessage, refs) {
    if (!(refs == null ? void 0 : refs.errorMessages))
      return;
    if (errorMessage) {
      res.errorMessage = __spreadProps(__spreadValues({}, res.errorMessage), {
        [key]: errorMessage
      });
    }
  }
  function setResponseValueAndErrors(res, key, value, errorMessage, refs) {
    res[key] = value;
    addErrorMessage(res, key, errorMessage, refs);
  }

  // node_modules/.pnpm/zod@3.24.2/node_modules/zod/lib/index.mjs
  var util;
  (function(util2) {
    util2.assertEqual = (val) => val;
    function assertIs(_arg) {
    }
    util2.assertIs = assertIs;
    function assertNever(_x) {
      throw new Error();
    }
    util2.assertNever = assertNever;
    util2.arrayToEnum = (items) => {
      const obj = {};
      for (const item of items) {
        obj[item] = item;
      }
      return obj;
    };
    util2.getValidEnumValues = (obj) => {
      const validKeys = util2.objectKeys(obj).filter((k) => typeof obj[obj[k]] !== "number");
      const filtered = {};
      for (const k of validKeys) {
        filtered[k] = obj[k];
      }
      return util2.objectValues(filtered);
    };
    util2.objectValues = (obj) => {
      return util2.objectKeys(obj).map(function(e) {
        return obj[e];
      });
    };
    util2.objectKeys = typeof Object.keys === "function" ? (obj) => Object.keys(obj) : (object) => {
      const keys = [];
      for (const key in object) {
        if (Object.prototype.hasOwnProperty.call(object, key)) {
          keys.push(key);
        }
      }
      return keys;
    };
    util2.find = (arr, checker) => {
      for (const item of arr) {
        if (checker(item))
          return item;
      }
      return void 0;
    };
    util2.isInteger = typeof Number.isInteger === "function" ? (val) => Number.isInteger(val) : (val) => typeof val === "number" && isFinite(val) && Math.floor(val) === val;
    function joinValues(array, separator = " | ") {
      return array.map((val) => typeof val === "string" ? `'${val}'` : val).join(separator);
    }
    util2.joinValues = joinValues;
    util2.jsonStringifyReplacer = (_, value) => {
      if (typeof value === "bigint") {
        return value.toString();
      }
      return value;
    };
  })(util || (util = {}));
  var objectUtil;
  (function(objectUtil2) {
    objectUtil2.mergeShapes = (first, second) => {
      return __spreadValues(__spreadValues({}, first), second);
    };
  })(objectUtil || (objectUtil = {}));
  var ZodParsedType = util.arrayToEnum([
    "string",
    "nan",
    "number",
    "integer",
    "float",
    "boolean",
    "date",
    "bigint",
    "symbol",
    "function",
    "undefined",
    "null",
    "array",
    "object",
    "unknown",
    "promise",
    "void",
    "never",
    "map",
    "set"
  ]);
  var getParsedType = (data) => {
    const t = typeof data;
    switch (t) {
      case "undefined":
        return ZodParsedType.undefined;
      case "string":
        return ZodParsedType.string;
      case "number":
        return isNaN(data) ? ZodParsedType.nan : ZodParsedType.number;
      case "boolean":
        return ZodParsedType.boolean;
      case "function":
        return ZodParsedType.function;
      case "bigint":
        return ZodParsedType.bigint;
      case "symbol":
        return ZodParsedType.symbol;
      case "object":
        if (Array.isArray(data)) {
          return ZodParsedType.array;
        }
        if (data === null) {
          return ZodParsedType.null;
        }
        if (data.then && typeof data.then === "function" && data.catch && typeof data.catch === "function") {
          return ZodParsedType.promise;
        }
        if (typeof Map !== "undefined" && data instanceof Map) {
          return ZodParsedType.map;
        }
        if (typeof Set !== "undefined" && data instanceof Set) {
          return ZodParsedType.set;
        }
        if (typeof Date !== "undefined" && data instanceof Date) {
          return ZodParsedType.date;
        }
        return ZodParsedType.object;
      default:
        return ZodParsedType.unknown;
    }
  };
  var ZodIssueCode = util.arrayToEnum([
    "invalid_type",
    "invalid_literal",
    "custom",
    "invalid_union",
    "invalid_union_discriminator",
    "invalid_enum_value",
    "unrecognized_keys",
    "invalid_arguments",
    "invalid_return_type",
    "invalid_date",
    "invalid_string",
    "too_small",
    "too_big",
    "invalid_intersection_types",
    "not_multiple_of",
    "not_finite"
  ]);
  var quotelessJson = (obj) => {
    const json = JSON.stringify(obj, null, 2);
    return json.replace(/"([^"]+)":/g, "$1:");
  };
  var ZodError = class _ZodError extends Error {
    get errors() {
      return this.issues;
    }
    constructor(issues) {
      super();
      this.issues = [];
      this.addIssue = (sub) => {
        this.issues = [...this.issues, sub];
      };
      this.addIssues = (subs = []) => {
        this.issues = [...this.issues, ...subs];
      };
      const actualProto = new.target.prototype;
      if (Object.setPrototypeOf) {
        Object.setPrototypeOf(this, actualProto);
      } else {
        this.__proto__ = actualProto;
      }
      this.name = "ZodError";
      this.issues = issues;
    }
    format(_mapper) {
      const mapper = _mapper || function(issue) {
        return issue.message;
      };
      const fieldErrors = { _errors: [] };
      const processError = (error) => {
        for (const issue of error.issues) {
          if (issue.code === "invalid_union") {
            issue.unionErrors.map(processError);
          } else if (issue.code === "invalid_return_type") {
            processError(issue.returnTypeError);
          } else if (issue.code === "invalid_arguments") {
            processError(issue.argumentsError);
          } else if (issue.path.length === 0) {
            fieldErrors._errors.push(mapper(issue));
          } else {
            let curr = fieldErrors;
            let i = 0;
            while (i < issue.path.length) {
              const el = issue.path[i];
              const terminal = i === issue.path.length - 1;
              if (!terminal) {
                curr[el] = curr[el] || { _errors: [] };
              } else {
                curr[el] = curr[el] || { _errors: [] };
                curr[el]._errors.push(mapper(issue));
              }
              curr = curr[el];
              i++;
            }
          }
        }
      };
      processError(this);
      return fieldErrors;
    }
    static assert(value) {
      if (!(value instanceof _ZodError)) {
        throw new Error(`Not a ZodError: ${value}`);
      }
    }
    toString() {
      return this.message;
    }
    get message() {
      return JSON.stringify(this.issues, util.jsonStringifyReplacer, 2);
    }
    get isEmpty() {
      return this.issues.length === 0;
    }
    flatten(mapper = (issue) => issue.message) {
      const fieldErrors = {};
      const formErrors = [];
      for (const sub of this.issues) {
        if (sub.path.length > 0) {
          fieldErrors[sub.path[0]] = fieldErrors[sub.path[0]] || [];
          fieldErrors[sub.path[0]].push(mapper(sub));
        } else {
          formErrors.push(mapper(sub));
        }
      }
      return { formErrors, fieldErrors };
    }
    get formErrors() {
      return this.flatten();
    }
  };
  ZodError.create = (issues) => {
    const error = new ZodError(issues);
    return error;
  };
  var errorMap = (issue, _ctx) => {
    let message;
    switch (issue.code) {
      case ZodIssueCode.invalid_type:
        if (issue.received === ZodParsedType.undefined) {
          message = "Required";
        } else {
          message = `Expected ${issue.expected}, received ${issue.received}`;
        }
        break;
      case ZodIssueCode.invalid_literal:
        message = `Invalid literal value, expected ${JSON.stringify(issue.expected, util.jsonStringifyReplacer)}`;
        break;
      case ZodIssueCode.unrecognized_keys:
        message = `Unrecognized key(s) in object: ${util.joinValues(issue.keys, ", ")}`;
        break;
      case ZodIssueCode.invalid_union:
        message = `Invalid input`;
        break;
      case ZodIssueCode.invalid_union_discriminator:
        message = `Invalid discriminator value. Expected ${util.joinValues(issue.options)}`;
        break;
      case ZodIssueCode.invalid_enum_value:
        message = `Invalid enum value. Expected ${util.joinValues(issue.options)}, received '${issue.received}'`;
        break;
      case ZodIssueCode.invalid_arguments:
        message = `Invalid function arguments`;
        break;
      case ZodIssueCode.invalid_return_type:
        message = `Invalid function return type`;
        break;
      case ZodIssueCode.invalid_date:
        message = `Invalid date`;
        break;
      case ZodIssueCode.invalid_string:
        if (typeof issue.validation === "object") {
          if ("includes" in issue.validation) {
            message = `Invalid input: must include "${issue.validation.includes}"`;
            if (typeof issue.validation.position === "number") {
              message = `${message} at one or more positions greater than or equal to ${issue.validation.position}`;
            }
          } else if ("startsWith" in issue.validation) {
            message = `Invalid input: must start with "${issue.validation.startsWith}"`;
          } else if ("endsWith" in issue.validation) {
            message = `Invalid input: must end with "${issue.validation.endsWith}"`;
          } else {
            util.assertNever(issue.validation);
          }
        } else if (issue.validation !== "regex") {
          message = `Invalid ${issue.validation}`;
        } else {
          message = "Invalid";
        }
        break;
      case ZodIssueCode.too_small:
        if (issue.type === "array")
          message = `Array must contain ${issue.exact ? "exactly" : issue.inclusive ? `at least` : `more than`} ${issue.minimum} element(s)`;
        else if (issue.type === "string")
          message = `String must contain ${issue.exact ? "exactly" : issue.inclusive ? `at least` : `over`} ${issue.minimum} character(s)`;
        else if (issue.type === "number")
          message = `Number must be ${issue.exact ? `exactly equal to ` : issue.inclusive ? `greater than or equal to ` : `greater than `}${issue.minimum}`;
        else if (issue.type === "date")
          message = `Date must be ${issue.exact ? `exactly equal to ` : issue.inclusive ? `greater than or equal to ` : `greater than `}${new Date(Number(issue.minimum))}`;
        else
          message = "Invalid input";
        break;
      case ZodIssueCode.too_big:
        if (issue.type === "array")
          message = `Array must contain ${issue.exact ? `exactly` : issue.inclusive ? `at most` : `less than`} ${issue.maximum} element(s)`;
        else if (issue.type === "string")
          message = `String must contain ${issue.exact ? `exactly` : issue.inclusive ? `at most` : `under`} ${issue.maximum} character(s)`;
        else if (issue.type === "number")
          message = `Number must be ${issue.exact ? `exactly` : issue.inclusive ? `less than or equal to` : `less than`} ${issue.maximum}`;
        else if (issue.type === "bigint")
          message = `BigInt must be ${issue.exact ? `exactly` : issue.inclusive ? `less than or equal to` : `less than`} ${issue.maximum}`;
        else if (issue.type === "date")
          message = `Date must be ${issue.exact ? `exactly` : issue.inclusive ? `smaller than or equal to` : `smaller than`} ${new Date(Number(issue.maximum))}`;
        else
          message = "Invalid input";
        break;
      case ZodIssueCode.custom:
        message = `Invalid input`;
        break;
      case ZodIssueCode.invalid_intersection_types:
        message = `Intersection results could not be merged`;
        break;
      case ZodIssueCode.not_multiple_of:
        message = `Number must be a multiple of ${issue.multipleOf}`;
        break;
      case ZodIssueCode.not_finite:
        message = "Number must be finite";
        break;
      default:
        message = _ctx.defaultError;
        util.assertNever(issue);
    }
    return { message };
  };
  var overrideErrorMap = errorMap;
  function setErrorMap(map) {
    overrideErrorMap = map;
  }
  function getErrorMap() {
    return overrideErrorMap;
  }
  var makeIssue = (params) => {
    const { data, path, errorMaps, issueData } = params;
    const fullPath = [...path, ...issueData.path || []];
    const fullIssue = __spreadProps(__spreadValues({}, issueData), {
      path: fullPath
    });
    if (issueData.message !== void 0) {
      return __spreadProps(__spreadValues({}, issueData), {
        path: fullPath,
        message: issueData.message
      });
    }
    let errorMessage = "";
    const maps = errorMaps.filter((m) => !!m).slice().reverse();
    for (const map of maps) {
      errorMessage = map(fullIssue, { data, defaultError: errorMessage }).message;
    }
    return __spreadProps(__spreadValues({}, issueData), {
      path: fullPath,
      message: errorMessage
    });
  };
  var EMPTY_PATH = [];
  function addIssueToContext(ctx, issueData) {
    const overrideMap = getErrorMap();
    const issue = makeIssue({
      issueData,
      data: ctx.data,
      path: ctx.path,
      errorMaps: [
        ctx.common.contextualErrorMap,
        // contextual error map is first priority
        ctx.schemaErrorMap,
        // then schema-bound map if available
        overrideMap,
        // then global override map
        overrideMap === errorMap ? void 0 : errorMap
        // then global default map
      ].filter((x) => !!x)
    });
    ctx.common.issues.push(issue);
  }
  var ParseStatus = class _ParseStatus {
    constructor() {
      this.value = "valid";
    }
    dirty() {
      if (this.value === "valid")
        this.value = "dirty";
    }
    abort() {
      if (this.value !== "aborted")
        this.value = "aborted";
    }
    static mergeArray(status, results) {
      const arrayValue = [];
      for (const s of results) {
        if (s.status === "aborted")
          return INVALID;
        if (s.status === "dirty")
          status.dirty();
        arrayValue.push(s.value);
      }
      return { status: status.value, value: arrayValue };
    }
    static mergeObjectAsync(status, pairs) {
      return __async(this, null, function* () {
        const syncPairs = [];
        for (const pair of pairs) {
          const key = yield pair.key;
          const value = yield pair.value;
          syncPairs.push({
            key,
            value
          });
        }
        return _ParseStatus.mergeObjectSync(status, syncPairs);
      });
    }
    static mergeObjectSync(status, pairs) {
      const finalObject = {};
      for (const pair of pairs) {
        const { key, value } = pair;
        if (key.status === "aborted")
          return INVALID;
        if (value.status === "aborted")
          return INVALID;
        if (key.status === "dirty")
          status.dirty();
        if (value.status === "dirty")
          status.dirty();
        if (key.value !== "__proto__" && (typeof value.value !== "undefined" || pair.alwaysSet)) {
          finalObject[key.value] = value.value;
        }
      }
      return { status: status.value, value: finalObject };
    }
  };
  var INVALID = Object.freeze({
    status: "aborted"
  });
  var DIRTY = (value) => ({ status: "dirty", value });
  var OK = (value) => ({ status: "valid", value });
  var isAborted = (x) => x.status === "aborted";
  var isDirty = (x) => x.status === "dirty";
  var isValid = (x) => x.status === "valid";
  var isAsync = (x) => typeof Promise !== "undefined" && x instanceof Promise;
  function __classPrivateFieldGet(receiver, state, kind, f) {
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a getter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot read private member from an object whose class did not declare it");
    return kind === "m" ? f : kind === "a" ? f.call(receiver) : f ? f.value : state.get(receiver);
  }
  function __classPrivateFieldSet(receiver, state, value, kind, f) {
    if (kind === "m") throw new TypeError("Private method is not writable");
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a setter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot write private member to an object whose class did not declare it");
    return kind === "a" ? f.call(receiver, value) : f ? f.value = value : state.set(receiver, value), value;
  }
  var errorUtil;
  (function(errorUtil2) {
    errorUtil2.errToObj = (message) => typeof message === "string" ? { message } : message || {};
    errorUtil2.toString = (message) => typeof message === "string" ? message : message === null || message === void 0 ? void 0 : message.message;
  })(errorUtil || (errorUtil = {}));
  var _ZodEnum_cache;
  var _ZodNativeEnum_cache;
  var ParseInputLazyPath = class {
    constructor(parent, value, path, key) {
      this._cachedPath = [];
      this.parent = parent;
      this.data = value;
      this._path = path;
      this._key = key;
    }
    get path() {
      if (!this._cachedPath.length) {
        if (this._key instanceof Array) {
          this._cachedPath.push(...this._path, ...this._key);
        } else {
          this._cachedPath.push(...this._path, this._key);
        }
      }
      return this._cachedPath;
    }
  };
  var handleResult = (ctx, result) => {
    if (isValid(result)) {
      return { success: true, data: result.value };
    } else {
      if (!ctx.common.issues.length) {
        throw new Error("Validation failed but no issues detected.");
      }
      return {
        success: false,
        get error() {
          if (this._error)
            return this._error;
          const error = new ZodError(ctx.common.issues);
          this._error = error;
          return this._error;
        }
      };
    }
  };
  function processCreateParams(params) {
    if (!params)
      return {};
    const { errorMap: errorMap2, invalid_type_error, required_error, description } = params;
    if (errorMap2 && (invalid_type_error || required_error)) {
      throw new Error(`Can't use "invalid_type_error" or "required_error" in conjunction with custom error map.`);
    }
    if (errorMap2)
      return { errorMap: errorMap2, description };
    const customMap = (iss, ctx) => {
      var _a, _b;
      const { message } = params;
      if (iss.code === "invalid_enum_value") {
        return { message: message !== null && message !== void 0 ? message : ctx.defaultError };
      }
      if (typeof ctx.data === "undefined") {
        return { message: (_a = message !== null && message !== void 0 ? message : required_error) !== null && _a !== void 0 ? _a : ctx.defaultError };
      }
      if (iss.code !== "invalid_type")
        return { message: ctx.defaultError };
      return { message: (_b = message !== null && message !== void 0 ? message : invalid_type_error) !== null && _b !== void 0 ? _b : ctx.defaultError };
    };
    return { errorMap: customMap, description };
  }
  var ZodType = class {
    get description() {
      return this._def.description;
    }
    _getType(input) {
      return getParsedType(input.data);
    }
    _getOrReturnCtx(input, ctx) {
      return ctx || {
        common: input.parent.common,
        data: input.data,
        parsedType: getParsedType(input.data),
        schemaErrorMap: this._def.errorMap,
        path: input.path,
        parent: input.parent
      };
    }
    _processInputParams(input) {
      return {
        status: new ParseStatus(),
        ctx: {
          common: input.parent.common,
          data: input.data,
          parsedType: getParsedType(input.data),
          schemaErrorMap: this._def.errorMap,
          path: input.path,
          parent: input.parent
        }
      };
    }
    _parseSync(input) {
      const result = this._parse(input);
      if (isAsync(result)) {
        throw new Error("Synchronous parse encountered promise.");
      }
      return result;
    }
    _parseAsync(input) {
      const result = this._parse(input);
      return Promise.resolve(result);
    }
    parse(data, params) {
      const result = this.safeParse(data, params);
      if (result.success)
        return result.data;
      throw result.error;
    }
    safeParse(data, params) {
      var _a;
      const ctx = {
        common: {
          issues: [],
          async: (_a = params === null || params === void 0 ? void 0 : params.async) !== null && _a !== void 0 ? _a : false,
          contextualErrorMap: params === null || params === void 0 ? void 0 : params.errorMap
        },
        path: (params === null || params === void 0 ? void 0 : params.path) || [],
        schemaErrorMap: this._def.errorMap,
        parent: null,
        data,
        parsedType: getParsedType(data)
      };
      const result = this._parseSync({ data, path: ctx.path, parent: ctx });
      return handleResult(ctx, result);
    }
    "~validate"(data) {
      var _a, _b;
      const ctx = {
        common: {
          issues: [],
          async: !!this["~standard"].async
        },
        path: [],
        schemaErrorMap: this._def.errorMap,
        parent: null,
        data,
        parsedType: getParsedType(data)
      };
      if (!this["~standard"].async) {
        try {
          const result = this._parseSync({ data, path: [], parent: ctx });
          return isValid(result) ? {
            value: result.value
          } : {
            issues: ctx.common.issues
          };
        } catch (err) {
          if ((_b = (_a = err === null || err === void 0 ? void 0 : err.message) === null || _a === void 0 ? void 0 : _a.toLowerCase()) === null || _b === void 0 ? void 0 : _b.includes("encountered")) {
            this["~standard"].async = true;
          }
          ctx.common = {
            issues: [],
            async: true
          };
        }
      }
      return this._parseAsync({ data, path: [], parent: ctx }).then((result) => isValid(result) ? {
        value: result.value
      } : {
        issues: ctx.common.issues
      });
    }
    parseAsync(data, params) {
      return __async(this, null, function* () {
        const result = yield this.safeParseAsync(data, params);
        if (result.success)
          return result.data;
        throw result.error;
      });
    }
    safeParseAsync(data, params) {
      return __async(this, null, function* () {
        const ctx = {
          common: {
            issues: [],
            contextualErrorMap: params === null || params === void 0 ? void 0 : params.errorMap,
            async: true
          },
          path: (params === null || params === void 0 ? void 0 : params.path) || [],
          schemaErrorMap: this._def.errorMap,
          parent: null,
          data,
          parsedType: getParsedType(data)
        };
        const maybeAsyncResult = this._parse({ data, path: ctx.path, parent: ctx });
        const result = yield isAsync(maybeAsyncResult) ? maybeAsyncResult : Promise.resolve(maybeAsyncResult);
        return handleResult(ctx, result);
      });
    }
    refine(check, message) {
      const getIssueProperties = (val) => {
        if (typeof message === "string" || typeof message === "undefined") {
          return { message };
        } else if (typeof message === "function") {
          return message(val);
        } else {
          return message;
        }
      };
      return this._refinement((val, ctx) => {
        const result = check(val);
        const setError = () => ctx.addIssue(__spreadValues({
          code: ZodIssueCode.custom
        }, getIssueProperties(val)));
        if (typeof Promise !== "undefined" && result instanceof Promise) {
          return result.then((data) => {
            if (!data) {
              setError();
              return false;
            } else {
              return true;
            }
          });
        }
        if (!result) {
          setError();
          return false;
        } else {
          return true;
        }
      });
    }
    refinement(check, refinementData) {
      return this._refinement((val, ctx) => {
        if (!check(val)) {
          ctx.addIssue(typeof refinementData === "function" ? refinementData(val, ctx) : refinementData);
          return false;
        } else {
          return true;
        }
      });
    }
    _refinement(refinement) {
      return new ZodEffects({
        schema: this,
        typeName: ZodFirstPartyTypeKind.ZodEffects,
        effect: { type: "refinement", refinement }
      });
    }
    superRefine(refinement) {
      return this._refinement(refinement);
    }
    constructor(def) {
      this.spa = this.safeParseAsync;
      this._def = def;
      this.parse = this.parse.bind(this);
      this.safeParse = this.safeParse.bind(this);
      this.parseAsync = this.parseAsync.bind(this);
      this.safeParseAsync = this.safeParseAsync.bind(this);
      this.spa = this.spa.bind(this);
      this.refine = this.refine.bind(this);
      this.refinement = this.refinement.bind(this);
      this.superRefine = this.superRefine.bind(this);
      this.optional = this.optional.bind(this);
      this.nullable = this.nullable.bind(this);
      this.nullish = this.nullish.bind(this);
      this.array = this.array.bind(this);
      this.promise = this.promise.bind(this);
      this.or = this.or.bind(this);
      this.and = this.and.bind(this);
      this.transform = this.transform.bind(this);
      this.brand = this.brand.bind(this);
      this.default = this.default.bind(this);
      this.catch = this.catch.bind(this);
      this.describe = this.describe.bind(this);
      this.pipe = this.pipe.bind(this);
      this.readonly = this.readonly.bind(this);
      this.isNullable = this.isNullable.bind(this);
      this.isOptional = this.isOptional.bind(this);
      this["~standard"] = {
        version: 1,
        vendor: "zod",
        validate: (data) => this["~validate"](data)
      };
    }
    optional() {
      return ZodOptional.create(this, this._def);
    }
    nullable() {
      return ZodNullable.create(this, this._def);
    }
    nullish() {
      return this.nullable().optional();
    }
    array() {
      return ZodArray.create(this);
    }
    promise() {
      return ZodPromise.create(this, this._def);
    }
    or(option) {
      return ZodUnion.create([this, option], this._def);
    }
    and(incoming) {
      return ZodIntersection.create(this, incoming, this._def);
    }
    transform(transform) {
      return new ZodEffects(__spreadProps(__spreadValues({}, processCreateParams(this._def)), {
        schema: this,
        typeName: ZodFirstPartyTypeKind.ZodEffects,
        effect: { type: "transform", transform }
      }));
    }
    default(def) {
      const defaultValueFunc = typeof def === "function" ? def : () => def;
      return new ZodDefault(__spreadProps(__spreadValues({}, processCreateParams(this._def)), {
        innerType: this,
        defaultValue: defaultValueFunc,
        typeName: ZodFirstPartyTypeKind.ZodDefault
      }));
    }
    brand() {
      return new ZodBranded(__spreadValues({
        typeName: ZodFirstPartyTypeKind.ZodBranded,
        type: this
      }, processCreateParams(this._def)));
    }
    catch(def) {
      const catchValueFunc = typeof def === "function" ? def : () => def;
      return new ZodCatch(__spreadProps(__spreadValues({}, processCreateParams(this._def)), {
        innerType: this,
        catchValue: catchValueFunc,
        typeName: ZodFirstPartyTypeKind.ZodCatch
      }));
    }
    describe(description) {
      const This = this.constructor;
      return new This(__spreadProps(__spreadValues({}, this._def), {
        description
      }));
    }
    pipe(target) {
      return ZodPipeline.create(this, target);
    }
    readonly() {
      return ZodReadonly.create(this);
    }
    isOptional() {
      return this.safeParse(void 0).success;
    }
    isNullable() {
      return this.safeParse(null).success;
    }
  };
  var cuidRegex = /^c[^\s-]{8,}$/i;
  var cuid2Regex = /^[0-9a-z]+$/;
  var ulidRegex = /^[0-9A-HJKMNP-TV-Z]{26}$/i;
  var uuidRegex = /^[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}$/i;
  var nanoidRegex = /^[a-z0-9_-]{21}$/i;
  var jwtRegex = /^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]*$/;
  var durationRegex = /^[-+]?P(?!$)(?:(?:[-+]?\d+Y)|(?:[-+]?\d+[.,]\d+Y$))?(?:(?:[-+]?\d+M)|(?:[-+]?\d+[.,]\d+M$))?(?:(?:[-+]?\d+W)|(?:[-+]?\d+[.,]\d+W$))?(?:(?:[-+]?\d+D)|(?:[-+]?\d+[.,]\d+D$))?(?:T(?=[\d+-])(?:(?:[-+]?\d+H)|(?:[-+]?\d+[.,]\d+H$))?(?:(?:[-+]?\d+M)|(?:[-+]?\d+[.,]\d+M$))?(?:[-+]?\d+(?:[.,]\d+)?S)?)??$/;
  var emailRegex = /^(?!\.)(?!.*\.\.)([A-Z0-9_'+\-\.]*)[A-Z0-9_+-]@([A-Z0-9][A-Z0-9\-]*\.)+[A-Z]{2,}$/i;
  var _emojiRegex = `^(\\p{Extended_Pictographic}|\\p{Emoji_Component})+$`;
  var emojiRegex;
  var ipv4Regex = /^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])$/;
  var ipv4CidrRegex = /^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\/(3[0-2]|[12]?[0-9])$/;
  var ipv6Regex = /^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$/;
  var ipv6CidrRegex = /^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))\/(12[0-8]|1[01][0-9]|[1-9]?[0-9])$/;
  var base64Regex = /^([0-9a-zA-Z+/]{4})*(([0-9a-zA-Z+/]{2}==)|([0-9a-zA-Z+/]{3}=))?$/;
  var base64urlRegex = /^([0-9a-zA-Z-_]{4})*(([0-9a-zA-Z-_]{2}(==)?)|([0-9a-zA-Z-_]{3}(=)?))?$/;
  var dateRegexSource = `((\\d\\d[2468][048]|\\d\\d[13579][26]|\\d\\d0[48]|[02468][048]00|[13579][26]00)-02-29|\\d{4}-((0[13578]|1[02])-(0[1-9]|[12]\\d|3[01])|(0[469]|11)-(0[1-9]|[12]\\d|30)|(02)-(0[1-9]|1\\d|2[0-8])))`;
  var dateRegex = new RegExp(`^${dateRegexSource}$`);
  function timeRegexSource(args) {
    let regex = `([01]\\d|2[0-3]):[0-5]\\d:[0-5]\\d`;
    if (args.precision) {
      regex = `${regex}\\.\\d{${args.precision}}`;
    } else if (args.precision == null) {
      regex = `${regex}(\\.\\d+)?`;
    }
    return regex;
  }
  function timeRegex(args) {
    return new RegExp(`^${timeRegexSource(args)}$`);
  }
  function datetimeRegex(args) {
    let regex = `${dateRegexSource}T${timeRegexSource(args)}`;
    const opts = [];
    opts.push(args.local ? `Z?` : `Z`);
    if (args.offset)
      opts.push(`([+-]\\d{2}:?\\d{2})`);
    regex = `${regex}(${opts.join("|")})`;
    return new RegExp(`^${regex}$`);
  }
  function isValidIP(ip, version) {
    if ((version === "v4" || !version) && ipv4Regex.test(ip)) {
      return true;
    }
    if ((version === "v6" || !version) && ipv6Regex.test(ip)) {
      return true;
    }
    return false;
  }
  function isValidJWT(jwt, alg) {
    if (!jwtRegex.test(jwt))
      return false;
    try {
      const [header] = jwt.split(".");
      const base64 = header.replace(/-/g, "+").replace(/_/g, "/").padEnd(header.length + (4 - header.length % 4) % 4, "=");
      const decoded = JSON.parse(atob(base64));
      if (typeof decoded !== "object" || decoded === null)
        return false;
      if (!decoded.typ || !decoded.alg)
        return false;
      if (alg && decoded.alg !== alg)
        return false;
      return true;
    } catch (_a) {
      return false;
    }
  }
  function isValidCidr(ip, version) {
    if ((version === "v4" || !version) && ipv4CidrRegex.test(ip)) {
      return true;
    }
    if ((version === "v6" || !version) && ipv6CidrRegex.test(ip)) {
      return true;
    }
    return false;
  }
  var ZodString = class _ZodString extends ZodType {
    _parse(input) {
      if (this._def.coerce) {
        input.data = String(input.data);
      }
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.string) {
        const ctx2 = this._getOrReturnCtx(input);
        addIssueToContext(ctx2, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.string,
          received: ctx2.parsedType
        });
        return INVALID;
      }
      const status = new ParseStatus();
      let ctx = void 0;
      for (const check of this._def.checks) {
        if (check.kind === "min") {
          if (input.data.length < check.value) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_small,
              minimum: check.value,
              type: "string",
              inclusive: true,
              exact: false,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "max") {
          if (input.data.length > check.value) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_big,
              maximum: check.value,
              type: "string",
              inclusive: true,
              exact: false,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "length") {
          const tooBig = input.data.length > check.value;
          const tooSmall = input.data.length < check.value;
          if (tooBig || tooSmall) {
            ctx = this._getOrReturnCtx(input, ctx);
            if (tooBig) {
              addIssueToContext(ctx, {
                code: ZodIssueCode.too_big,
                maximum: check.value,
                type: "string",
                inclusive: true,
                exact: true,
                message: check.message
              });
            } else if (tooSmall) {
              addIssueToContext(ctx, {
                code: ZodIssueCode.too_small,
                minimum: check.value,
                type: "string",
                inclusive: true,
                exact: true,
                message: check.message
              });
            }
            status.dirty();
          }
        } else if (check.kind === "email") {
          if (!emailRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "email",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "emoji") {
          if (!emojiRegex) {
            emojiRegex = new RegExp(_emojiRegex, "u");
          }
          if (!emojiRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "emoji",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "uuid") {
          if (!uuidRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "uuid",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "nanoid") {
          if (!nanoidRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "nanoid",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "cuid") {
          if (!cuidRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "cuid",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "cuid2") {
          if (!cuid2Regex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "cuid2",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "ulid") {
          if (!ulidRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "ulid",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "url") {
          try {
            new URL(input.data);
          } catch (_a) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "url",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "regex") {
          check.regex.lastIndex = 0;
          const testResult = check.regex.test(input.data);
          if (!testResult) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "regex",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "trim") {
          input.data = input.data.trim();
        } else if (check.kind === "includes") {
          if (!input.data.includes(check.value, check.position)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_string,
              validation: { includes: check.value, position: check.position },
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "toLowerCase") {
          input.data = input.data.toLowerCase();
        } else if (check.kind === "toUpperCase") {
          input.data = input.data.toUpperCase();
        } else if (check.kind === "startsWith") {
          if (!input.data.startsWith(check.value)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_string,
              validation: { startsWith: check.value },
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "endsWith") {
          if (!input.data.endsWith(check.value)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_string,
              validation: { endsWith: check.value },
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "datetime") {
          const regex = datetimeRegex(check);
          if (!regex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_string,
              validation: "datetime",
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "date") {
          const regex = dateRegex;
          if (!regex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_string,
              validation: "date",
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "time") {
          const regex = timeRegex(check);
          if (!regex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_string,
              validation: "time",
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "duration") {
          if (!durationRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "duration",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "ip") {
          if (!isValidIP(input.data, check.version)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "ip",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "jwt") {
          if (!isValidJWT(input.data, check.alg)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "jwt",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "cidr") {
          if (!isValidCidr(input.data, check.version)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "cidr",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "base64") {
          if (!base64Regex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "base64",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "base64url") {
          if (!base64urlRegex.test(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              validation: "base64url",
              code: ZodIssueCode.invalid_string,
              message: check.message
            });
            status.dirty();
          }
        } else {
          util.assertNever(check);
        }
      }
      return { status: status.value, value: input.data };
    }
    _regex(regex, validation, message) {
      return this.refinement((data) => regex.test(data), __spreadValues({
        validation,
        code: ZodIssueCode.invalid_string
      }, errorUtil.errToObj(message)));
    }
    _addCheck(check) {
      return new _ZodString(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, check]
      }));
    }
    email(message) {
      return this._addCheck(__spreadValues({ kind: "email" }, errorUtil.errToObj(message)));
    }
    url(message) {
      return this._addCheck(__spreadValues({ kind: "url" }, errorUtil.errToObj(message)));
    }
    emoji(message) {
      return this._addCheck(__spreadValues({ kind: "emoji" }, errorUtil.errToObj(message)));
    }
    uuid(message) {
      return this._addCheck(__spreadValues({ kind: "uuid" }, errorUtil.errToObj(message)));
    }
    nanoid(message) {
      return this._addCheck(__spreadValues({ kind: "nanoid" }, errorUtil.errToObj(message)));
    }
    cuid(message) {
      return this._addCheck(__spreadValues({ kind: "cuid" }, errorUtil.errToObj(message)));
    }
    cuid2(message) {
      return this._addCheck(__spreadValues({ kind: "cuid2" }, errorUtil.errToObj(message)));
    }
    ulid(message) {
      return this._addCheck(__spreadValues({ kind: "ulid" }, errorUtil.errToObj(message)));
    }
    base64(message) {
      return this._addCheck(__spreadValues({ kind: "base64" }, errorUtil.errToObj(message)));
    }
    base64url(message) {
      return this._addCheck(__spreadValues({
        kind: "base64url"
      }, errorUtil.errToObj(message)));
    }
    jwt(options) {
      return this._addCheck(__spreadValues({ kind: "jwt" }, errorUtil.errToObj(options)));
    }
    ip(options) {
      return this._addCheck(__spreadValues({ kind: "ip" }, errorUtil.errToObj(options)));
    }
    cidr(options) {
      return this._addCheck(__spreadValues({ kind: "cidr" }, errorUtil.errToObj(options)));
    }
    datetime(options) {
      var _a, _b;
      if (typeof options === "string") {
        return this._addCheck({
          kind: "datetime",
          precision: null,
          offset: false,
          local: false,
          message: options
        });
      }
      return this._addCheck(__spreadValues({
        kind: "datetime",
        precision: typeof (options === null || options === void 0 ? void 0 : options.precision) === "undefined" ? null : options === null || options === void 0 ? void 0 : options.precision,
        offset: (_a = options === null || options === void 0 ? void 0 : options.offset) !== null && _a !== void 0 ? _a : false,
        local: (_b = options === null || options === void 0 ? void 0 : options.local) !== null && _b !== void 0 ? _b : false
      }, errorUtil.errToObj(options === null || options === void 0 ? void 0 : options.message)));
    }
    date(message) {
      return this._addCheck({ kind: "date", message });
    }
    time(options) {
      if (typeof options === "string") {
        return this._addCheck({
          kind: "time",
          precision: null,
          message: options
        });
      }
      return this._addCheck(__spreadValues({
        kind: "time",
        precision: typeof (options === null || options === void 0 ? void 0 : options.precision) === "undefined" ? null : options === null || options === void 0 ? void 0 : options.precision
      }, errorUtil.errToObj(options === null || options === void 0 ? void 0 : options.message)));
    }
    duration(message) {
      return this._addCheck(__spreadValues({ kind: "duration" }, errorUtil.errToObj(message)));
    }
    regex(regex, message) {
      return this._addCheck(__spreadValues({
        kind: "regex",
        regex
      }, errorUtil.errToObj(message)));
    }
    includes(value, options) {
      return this._addCheck(__spreadValues({
        kind: "includes",
        value,
        position: options === null || options === void 0 ? void 0 : options.position
      }, errorUtil.errToObj(options === null || options === void 0 ? void 0 : options.message)));
    }
    startsWith(value, message) {
      return this._addCheck(__spreadValues({
        kind: "startsWith",
        value
      }, errorUtil.errToObj(message)));
    }
    endsWith(value, message) {
      return this._addCheck(__spreadValues({
        kind: "endsWith",
        value
      }, errorUtil.errToObj(message)));
    }
    min(minLength, message) {
      return this._addCheck(__spreadValues({
        kind: "min",
        value: minLength
      }, errorUtil.errToObj(message)));
    }
    max(maxLength, message) {
      return this._addCheck(__spreadValues({
        kind: "max",
        value: maxLength
      }, errorUtil.errToObj(message)));
    }
    length(len, message) {
      return this._addCheck(__spreadValues({
        kind: "length",
        value: len
      }, errorUtil.errToObj(message)));
    }
    /**
     * Equivalent to `.min(1)`
     */
    nonempty(message) {
      return this.min(1, errorUtil.errToObj(message));
    }
    trim() {
      return new _ZodString(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, { kind: "trim" }]
      }));
    }
    toLowerCase() {
      return new _ZodString(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, { kind: "toLowerCase" }]
      }));
    }
    toUpperCase() {
      return new _ZodString(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, { kind: "toUpperCase" }]
      }));
    }
    get isDatetime() {
      return !!this._def.checks.find((ch) => ch.kind === "datetime");
    }
    get isDate() {
      return !!this._def.checks.find((ch) => ch.kind === "date");
    }
    get isTime() {
      return !!this._def.checks.find((ch) => ch.kind === "time");
    }
    get isDuration() {
      return !!this._def.checks.find((ch) => ch.kind === "duration");
    }
    get isEmail() {
      return !!this._def.checks.find((ch) => ch.kind === "email");
    }
    get isURL() {
      return !!this._def.checks.find((ch) => ch.kind === "url");
    }
    get isEmoji() {
      return !!this._def.checks.find((ch) => ch.kind === "emoji");
    }
    get isUUID() {
      return !!this._def.checks.find((ch) => ch.kind === "uuid");
    }
    get isNANOID() {
      return !!this._def.checks.find((ch) => ch.kind === "nanoid");
    }
    get isCUID() {
      return !!this._def.checks.find((ch) => ch.kind === "cuid");
    }
    get isCUID2() {
      return !!this._def.checks.find((ch) => ch.kind === "cuid2");
    }
    get isULID() {
      return !!this._def.checks.find((ch) => ch.kind === "ulid");
    }
    get isIP() {
      return !!this._def.checks.find((ch) => ch.kind === "ip");
    }
    get isCIDR() {
      return !!this._def.checks.find((ch) => ch.kind === "cidr");
    }
    get isBase64() {
      return !!this._def.checks.find((ch) => ch.kind === "base64");
    }
    get isBase64url() {
      return !!this._def.checks.find((ch) => ch.kind === "base64url");
    }
    get minLength() {
      let min = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "min") {
          if (min === null || ch.value > min)
            min = ch.value;
        }
      }
      return min;
    }
    get maxLength() {
      let max = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "max") {
          if (max === null || ch.value < max)
            max = ch.value;
        }
      }
      return max;
    }
  };
  ZodString.create = (params) => {
    var _a;
    return new ZodString(__spreadValues({
      checks: [],
      typeName: ZodFirstPartyTypeKind.ZodString,
      coerce: (_a = params === null || params === void 0 ? void 0 : params.coerce) !== null && _a !== void 0 ? _a : false
    }, processCreateParams(params)));
  };
  function floatSafeRemainder(val, step) {
    const valDecCount = (val.toString().split(".")[1] || "").length;
    const stepDecCount = (step.toString().split(".")[1] || "").length;
    const decCount = valDecCount > stepDecCount ? valDecCount : stepDecCount;
    const valInt = parseInt(val.toFixed(decCount).replace(".", ""));
    const stepInt = parseInt(step.toFixed(decCount).replace(".", ""));
    return valInt % stepInt / Math.pow(10, decCount);
  }
  var ZodNumber = class _ZodNumber extends ZodType {
    constructor() {
      super(...arguments);
      this.min = this.gte;
      this.max = this.lte;
      this.step = this.multipleOf;
    }
    _parse(input) {
      if (this._def.coerce) {
        input.data = Number(input.data);
      }
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.number) {
        const ctx2 = this._getOrReturnCtx(input);
        addIssueToContext(ctx2, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.number,
          received: ctx2.parsedType
        });
        return INVALID;
      }
      let ctx = void 0;
      const status = new ParseStatus();
      for (const check of this._def.checks) {
        if (check.kind === "int") {
          if (!util.isInteger(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.invalid_type,
              expected: "integer",
              received: "float",
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "min") {
          const tooSmall = check.inclusive ? input.data < check.value : input.data <= check.value;
          if (tooSmall) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_small,
              minimum: check.value,
              type: "number",
              inclusive: check.inclusive,
              exact: false,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "max") {
          const tooBig = check.inclusive ? input.data > check.value : input.data >= check.value;
          if (tooBig) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_big,
              maximum: check.value,
              type: "number",
              inclusive: check.inclusive,
              exact: false,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "multipleOf") {
          if (floatSafeRemainder(input.data, check.value) !== 0) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.not_multiple_of,
              multipleOf: check.value,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "finite") {
          if (!Number.isFinite(input.data)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.not_finite,
              message: check.message
            });
            status.dirty();
          }
        } else {
          util.assertNever(check);
        }
      }
      return { status: status.value, value: input.data };
    }
    gte(value, message) {
      return this.setLimit("min", value, true, errorUtil.toString(message));
    }
    gt(value, message) {
      return this.setLimit("min", value, false, errorUtil.toString(message));
    }
    lte(value, message) {
      return this.setLimit("max", value, true, errorUtil.toString(message));
    }
    lt(value, message) {
      return this.setLimit("max", value, false, errorUtil.toString(message));
    }
    setLimit(kind, value, inclusive, message) {
      return new _ZodNumber(__spreadProps(__spreadValues({}, this._def), {
        checks: [
          ...this._def.checks,
          {
            kind,
            value,
            inclusive,
            message: errorUtil.toString(message)
          }
        ]
      }));
    }
    _addCheck(check) {
      return new _ZodNumber(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, check]
      }));
    }
    int(message) {
      return this._addCheck({
        kind: "int",
        message: errorUtil.toString(message)
      });
    }
    positive(message) {
      return this._addCheck({
        kind: "min",
        value: 0,
        inclusive: false,
        message: errorUtil.toString(message)
      });
    }
    negative(message) {
      return this._addCheck({
        kind: "max",
        value: 0,
        inclusive: false,
        message: errorUtil.toString(message)
      });
    }
    nonpositive(message) {
      return this._addCheck({
        kind: "max",
        value: 0,
        inclusive: true,
        message: errorUtil.toString(message)
      });
    }
    nonnegative(message) {
      return this._addCheck({
        kind: "min",
        value: 0,
        inclusive: true,
        message: errorUtil.toString(message)
      });
    }
    multipleOf(value, message) {
      return this._addCheck({
        kind: "multipleOf",
        value,
        message: errorUtil.toString(message)
      });
    }
    finite(message) {
      return this._addCheck({
        kind: "finite",
        message: errorUtil.toString(message)
      });
    }
    safe(message) {
      return this._addCheck({
        kind: "min",
        inclusive: true,
        value: Number.MIN_SAFE_INTEGER,
        message: errorUtil.toString(message)
      })._addCheck({
        kind: "max",
        inclusive: true,
        value: Number.MAX_SAFE_INTEGER,
        message: errorUtil.toString(message)
      });
    }
    get minValue() {
      let min = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "min") {
          if (min === null || ch.value > min)
            min = ch.value;
        }
      }
      return min;
    }
    get maxValue() {
      let max = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "max") {
          if (max === null || ch.value < max)
            max = ch.value;
        }
      }
      return max;
    }
    get isInt() {
      return !!this._def.checks.find((ch) => ch.kind === "int" || ch.kind === "multipleOf" && util.isInteger(ch.value));
    }
    get isFinite() {
      let max = null, min = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "finite" || ch.kind === "int" || ch.kind === "multipleOf") {
          return true;
        } else if (ch.kind === "min") {
          if (min === null || ch.value > min)
            min = ch.value;
        } else if (ch.kind === "max") {
          if (max === null || ch.value < max)
            max = ch.value;
        }
      }
      return Number.isFinite(min) && Number.isFinite(max);
    }
  };
  ZodNumber.create = (params) => {
    return new ZodNumber(__spreadValues({
      checks: [],
      typeName: ZodFirstPartyTypeKind.ZodNumber,
      coerce: (params === null || params === void 0 ? void 0 : params.coerce) || false
    }, processCreateParams(params)));
  };
  var ZodBigInt = class _ZodBigInt extends ZodType {
    constructor() {
      super(...arguments);
      this.min = this.gte;
      this.max = this.lte;
    }
    _parse(input) {
      if (this._def.coerce) {
        try {
          input.data = BigInt(input.data);
        } catch (_a) {
          return this._getInvalidInput(input);
        }
      }
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.bigint) {
        return this._getInvalidInput(input);
      }
      let ctx = void 0;
      const status = new ParseStatus();
      for (const check of this._def.checks) {
        if (check.kind === "min") {
          const tooSmall = check.inclusive ? input.data < check.value : input.data <= check.value;
          if (tooSmall) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_small,
              type: "bigint",
              minimum: check.value,
              inclusive: check.inclusive,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "max") {
          const tooBig = check.inclusive ? input.data > check.value : input.data >= check.value;
          if (tooBig) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_big,
              type: "bigint",
              maximum: check.value,
              inclusive: check.inclusive,
              message: check.message
            });
            status.dirty();
          }
        } else if (check.kind === "multipleOf") {
          if (input.data % check.value !== BigInt(0)) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.not_multiple_of,
              multipleOf: check.value,
              message: check.message
            });
            status.dirty();
          }
        } else {
          util.assertNever(check);
        }
      }
      return { status: status.value, value: input.data };
    }
    _getInvalidInput(input) {
      const ctx = this._getOrReturnCtx(input);
      addIssueToContext(ctx, {
        code: ZodIssueCode.invalid_type,
        expected: ZodParsedType.bigint,
        received: ctx.parsedType
      });
      return INVALID;
    }
    gte(value, message) {
      return this.setLimit("min", value, true, errorUtil.toString(message));
    }
    gt(value, message) {
      return this.setLimit("min", value, false, errorUtil.toString(message));
    }
    lte(value, message) {
      return this.setLimit("max", value, true, errorUtil.toString(message));
    }
    lt(value, message) {
      return this.setLimit("max", value, false, errorUtil.toString(message));
    }
    setLimit(kind, value, inclusive, message) {
      return new _ZodBigInt(__spreadProps(__spreadValues({}, this._def), {
        checks: [
          ...this._def.checks,
          {
            kind,
            value,
            inclusive,
            message: errorUtil.toString(message)
          }
        ]
      }));
    }
    _addCheck(check) {
      return new _ZodBigInt(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, check]
      }));
    }
    positive(message) {
      return this._addCheck({
        kind: "min",
        value: BigInt(0),
        inclusive: false,
        message: errorUtil.toString(message)
      });
    }
    negative(message) {
      return this._addCheck({
        kind: "max",
        value: BigInt(0),
        inclusive: false,
        message: errorUtil.toString(message)
      });
    }
    nonpositive(message) {
      return this._addCheck({
        kind: "max",
        value: BigInt(0),
        inclusive: true,
        message: errorUtil.toString(message)
      });
    }
    nonnegative(message) {
      return this._addCheck({
        kind: "min",
        value: BigInt(0),
        inclusive: true,
        message: errorUtil.toString(message)
      });
    }
    multipleOf(value, message) {
      return this._addCheck({
        kind: "multipleOf",
        value,
        message: errorUtil.toString(message)
      });
    }
    get minValue() {
      let min = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "min") {
          if (min === null || ch.value > min)
            min = ch.value;
        }
      }
      return min;
    }
    get maxValue() {
      let max = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "max") {
          if (max === null || ch.value < max)
            max = ch.value;
        }
      }
      return max;
    }
  };
  ZodBigInt.create = (params) => {
    var _a;
    return new ZodBigInt(__spreadValues({
      checks: [],
      typeName: ZodFirstPartyTypeKind.ZodBigInt,
      coerce: (_a = params === null || params === void 0 ? void 0 : params.coerce) !== null && _a !== void 0 ? _a : false
    }, processCreateParams(params)));
  };
  var ZodBoolean = class extends ZodType {
    _parse(input) {
      if (this._def.coerce) {
        input.data = Boolean(input.data);
      }
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.boolean) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.boolean,
          received: ctx.parsedType
        });
        return INVALID;
      }
      return OK(input.data);
    }
  };
  ZodBoolean.create = (params) => {
    return new ZodBoolean(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodBoolean,
      coerce: (params === null || params === void 0 ? void 0 : params.coerce) || false
    }, processCreateParams(params)));
  };
  var ZodDate = class _ZodDate extends ZodType {
    _parse(input) {
      if (this._def.coerce) {
        input.data = new Date(input.data);
      }
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.date) {
        const ctx2 = this._getOrReturnCtx(input);
        addIssueToContext(ctx2, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.date,
          received: ctx2.parsedType
        });
        return INVALID;
      }
      if (isNaN(input.data.getTime())) {
        const ctx2 = this._getOrReturnCtx(input);
        addIssueToContext(ctx2, {
          code: ZodIssueCode.invalid_date
        });
        return INVALID;
      }
      const status = new ParseStatus();
      let ctx = void 0;
      for (const check of this._def.checks) {
        if (check.kind === "min") {
          if (input.data.getTime() < check.value) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_small,
              message: check.message,
              inclusive: true,
              exact: false,
              minimum: check.value,
              type: "date"
            });
            status.dirty();
          }
        } else if (check.kind === "max") {
          if (input.data.getTime() > check.value) {
            ctx = this._getOrReturnCtx(input, ctx);
            addIssueToContext(ctx, {
              code: ZodIssueCode.too_big,
              message: check.message,
              inclusive: true,
              exact: false,
              maximum: check.value,
              type: "date"
            });
            status.dirty();
          }
        } else {
          util.assertNever(check);
        }
      }
      return {
        status: status.value,
        value: new Date(input.data.getTime())
      };
    }
    _addCheck(check) {
      return new _ZodDate(__spreadProps(__spreadValues({}, this._def), {
        checks: [...this._def.checks, check]
      }));
    }
    min(minDate, message) {
      return this._addCheck({
        kind: "min",
        value: minDate.getTime(),
        message: errorUtil.toString(message)
      });
    }
    max(maxDate, message) {
      return this._addCheck({
        kind: "max",
        value: maxDate.getTime(),
        message: errorUtil.toString(message)
      });
    }
    get minDate() {
      let min = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "min") {
          if (min === null || ch.value > min)
            min = ch.value;
        }
      }
      return min != null ? new Date(min) : null;
    }
    get maxDate() {
      let max = null;
      for (const ch of this._def.checks) {
        if (ch.kind === "max") {
          if (max === null || ch.value < max)
            max = ch.value;
        }
      }
      return max != null ? new Date(max) : null;
    }
  };
  ZodDate.create = (params) => {
    return new ZodDate(__spreadValues({
      checks: [],
      coerce: (params === null || params === void 0 ? void 0 : params.coerce) || false,
      typeName: ZodFirstPartyTypeKind.ZodDate
    }, processCreateParams(params)));
  };
  var ZodSymbol = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.symbol) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.symbol,
          received: ctx.parsedType
        });
        return INVALID;
      }
      return OK(input.data);
    }
  };
  ZodSymbol.create = (params) => {
    return new ZodSymbol(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodSymbol
    }, processCreateParams(params)));
  };
  var ZodUndefined = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.undefined) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.undefined,
          received: ctx.parsedType
        });
        return INVALID;
      }
      return OK(input.data);
    }
  };
  ZodUndefined.create = (params) => {
    return new ZodUndefined(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodUndefined
    }, processCreateParams(params)));
  };
  var ZodNull = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.null) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.null,
          received: ctx.parsedType
        });
        return INVALID;
      }
      return OK(input.data);
    }
  };
  ZodNull.create = (params) => {
    return new ZodNull(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodNull
    }, processCreateParams(params)));
  };
  var ZodAny = class extends ZodType {
    constructor() {
      super(...arguments);
      this._any = true;
    }
    _parse(input) {
      return OK(input.data);
    }
  };
  ZodAny.create = (params) => {
    return new ZodAny(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodAny
    }, processCreateParams(params)));
  };
  var ZodUnknown = class extends ZodType {
    constructor() {
      super(...arguments);
      this._unknown = true;
    }
    _parse(input) {
      return OK(input.data);
    }
  };
  ZodUnknown.create = (params) => {
    return new ZodUnknown(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodUnknown
    }, processCreateParams(params)));
  };
  var ZodNever = class extends ZodType {
    _parse(input) {
      const ctx = this._getOrReturnCtx(input);
      addIssueToContext(ctx, {
        code: ZodIssueCode.invalid_type,
        expected: ZodParsedType.never,
        received: ctx.parsedType
      });
      return INVALID;
    }
  };
  ZodNever.create = (params) => {
    return new ZodNever(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodNever
    }, processCreateParams(params)));
  };
  var ZodVoid = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.undefined) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.void,
          received: ctx.parsedType
        });
        return INVALID;
      }
      return OK(input.data);
    }
  };
  ZodVoid.create = (params) => {
    return new ZodVoid(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodVoid
    }, processCreateParams(params)));
  };
  var ZodArray = class _ZodArray extends ZodType {
    _parse(input) {
      const { ctx, status } = this._processInputParams(input);
      const def = this._def;
      if (ctx.parsedType !== ZodParsedType.array) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.array,
          received: ctx.parsedType
        });
        return INVALID;
      }
      if (def.exactLength !== null) {
        const tooBig = ctx.data.length > def.exactLength.value;
        const tooSmall = ctx.data.length < def.exactLength.value;
        if (tooBig || tooSmall) {
          addIssueToContext(ctx, {
            code: tooBig ? ZodIssueCode.too_big : ZodIssueCode.too_small,
            minimum: tooSmall ? def.exactLength.value : void 0,
            maximum: tooBig ? def.exactLength.value : void 0,
            type: "array",
            inclusive: true,
            exact: true,
            message: def.exactLength.message
          });
          status.dirty();
        }
      }
      if (def.minLength !== null) {
        if (ctx.data.length < def.minLength.value) {
          addIssueToContext(ctx, {
            code: ZodIssueCode.too_small,
            minimum: def.minLength.value,
            type: "array",
            inclusive: true,
            exact: false,
            message: def.minLength.message
          });
          status.dirty();
        }
      }
      if (def.maxLength !== null) {
        if (ctx.data.length > def.maxLength.value) {
          addIssueToContext(ctx, {
            code: ZodIssueCode.too_big,
            maximum: def.maxLength.value,
            type: "array",
            inclusive: true,
            exact: false,
            message: def.maxLength.message
          });
          status.dirty();
        }
      }
      if (ctx.common.async) {
        return Promise.all([...ctx.data].map((item, i) => {
          return def.type._parseAsync(new ParseInputLazyPath(ctx, item, ctx.path, i));
        })).then((result2) => {
          return ParseStatus.mergeArray(status, result2);
        });
      }
      const result = [...ctx.data].map((item, i) => {
        return def.type._parseSync(new ParseInputLazyPath(ctx, item, ctx.path, i));
      });
      return ParseStatus.mergeArray(status, result);
    }
    get element() {
      return this._def.type;
    }
    min(minLength, message) {
      return new _ZodArray(__spreadProps(__spreadValues({}, this._def), {
        minLength: { value: minLength, message: errorUtil.toString(message) }
      }));
    }
    max(maxLength, message) {
      return new _ZodArray(__spreadProps(__spreadValues({}, this._def), {
        maxLength: { value: maxLength, message: errorUtil.toString(message) }
      }));
    }
    length(len, message) {
      return new _ZodArray(__spreadProps(__spreadValues({}, this._def), {
        exactLength: { value: len, message: errorUtil.toString(message) }
      }));
    }
    nonempty(message) {
      return this.min(1, message);
    }
  };
  ZodArray.create = (schema, params) => {
    return new ZodArray(__spreadValues({
      type: schema,
      minLength: null,
      maxLength: null,
      exactLength: null,
      typeName: ZodFirstPartyTypeKind.ZodArray
    }, processCreateParams(params)));
  };
  function deepPartialify(schema) {
    if (schema instanceof ZodObject) {
      const newShape = {};
      for (const key in schema.shape) {
        const fieldSchema = schema.shape[key];
        newShape[key] = ZodOptional.create(deepPartialify(fieldSchema));
      }
      return new ZodObject(__spreadProps(__spreadValues({}, schema._def), {
        shape: () => newShape
      }));
    } else if (schema instanceof ZodArray) {
      return new ZodArray(__spreadProps(__spreadValues({}, schema._def), {
        type: deepPartialify(schema.element)
      }));
    } else if (schema instanceof ZodOptional) {
      return ZodOptional.create(deepPartialify(schema.unwrap()));
    } else if (schema instanceof ZodNullable) {
      return ZodNullable.create(deepPartialify(schema.unwrap()));
    } else if (schema instanceof ZodTuple) {
      return ZodTuple.create(schema.items.map((item) => deepPartialify(item)));
    } else {
      return schema;
    }
  }
  var ZodObject = class _ZodObject extends ZodType {
    constructor() {
      super(...arguments);
      this._cached = null;
      this.nonstrict = this.passthrough;
      this.augment = this.extend;
    }
    _getCached() {
      if (this._cached !== null)
        return this._cached;
      const shape = this._def.shape();
      const keys = util.objectKeys(shape);
      return this._cached = { shape, keys };
    }
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.object) {
        const ctx2 = this._getOrReturnCtx(input);
        addIssueToContext(ctx2, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.object,
          received: ctx2.parsedType
        });
        return INVALID;
      }
      const { status, ctx } = this._processInputParams(input);
      const { shape, keys: shapeKeys } = this._getCached();
      const extraKeys = [];
      if (!(this._def.catchall instanceof ZodNever && this._def.unknownKeys === "strip")) {
        for (const key in ctx.data) {
          if (!shapeKeys.includes(key)) {
            extraKeys.push(key);
          }
        }
      }
      const pairs = [];
      for (const key of shapeKeys) {
        const keyValidator = shape[key];
        const value = ctx.data[key];
        pairs.push({
          key: { status: "valid", value: key },
          value: keyValidator._parse(new ParseInputLazyPath(ctx, value, ctx.path, key)),
          alwaysSet: key in ctx.data
        });
      }
      if (this._def.catchall instanceof ZodNever) {
        const unknownKeys = this._def.unknownKeys;
        if (unknownKeys === "passthrough") {
          for (const key of extraKeys) {
            pairs.push({
              key: { status: "valid", value: key },
              value: { status: "valid", value: ctx.data[key] }
            });
          }
        } else if (unknownKeys === "strict") {
          if (extraKeys.length > 0) {
            addIssueToContext(ctx, {
              code: ZodIssueCode.unrecognized_keys,
              keys: extraKeys
            });
            status.dirty();
          }
        } else if (unknownKeys === "strip") ;
        else {
          throw new Error(`Internal ZodObject error: invalid unknownKeys value.`);
        }
      } else {
        const catchall = this._def.catchall;
        for (const key of extraKeys) {
          const value = ctx.data[key];
          pairs.push({
            key: { status: "valid", value: key },
            value: catchall._parse(
              new ParseInputLazyPath(ctx, value, ctx.path, key)
              //, ctx.child(key), value, getParsedType(value)
            ),
            alwaysSet: key in ctx.data
          });
        }
      }
      if (ctx.common.async) {
        return Promise.resolve().then(() => __async(this, null, function* () {
          const syncPairs = [];
          for (const pair of pairs) {
            const key = yield pair.key;
            const value = yield pair.value;
            syncPairs.push({
              key,
              value,
              alwaysSet: pair.alwaysSet
            });
          }
          return syncPairs;
        })).then((syncPairs) => {
          return ParseStatus.mergeObjectSync(status, syncPairs);
        });
      } else {
        return ParseStatus.mergeObjectSync(status, pairs);
      }
    }
    get shape() {
      return this._def.shape();
    }
    strict(message) {
      errorUtil.errToObj;
      return new _ZodObject(__spreadValues(__spreadProps(__spreadValues({}, this._def), {
        unknownKeys: "strict"
      }), message !== void 0 ? {
        errorMap: (issue, ctx) => {
          var _a, _b, _c, _d;
          const defaultError = (_c = (_b = (_a = this._def).errorMap) === null || _b === void 0 ? void 0 : _b.call(_a, issue, ctx).message) !== null && _c !== void 0 ? _c : ctx.defaultError;
          if (issue.code === "unrecognized_keys")
            return {
              message: (_d = errorUtil.errToObj(message).message) !== null && _d !== void 0 ? _d : defaultError
            };
          return {
            message: defaultError
          };
        }
      } : {}));
    }
    strip() {
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        unknownKeys: "strip"
      }));
    }
    passthrough() {
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        unknownKeys: "passthrough"
      }));
    }
    // const AugmentFactory =
    //   <Def extends ZodObjectDef>(def: Def) =>
    //   <Augmentation extends ZodRawShape>(
    //     augmentation: Augmentation
    //   ): ZodObject<
    //     extendShape<ReturnType<Def["shape"]>, Augmentation>,
    //     Def["unknownKeys"],
    //     Def["catchall"]
    //   > => {
    //     return new ZodObject({
    //       ...def,
    //       shape: () => ({
    //         ...def.shape(),
    //         ...augmentation,
    //       }),
    //     }) as any;
    //   };
    extend(augmentation) {
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        shape: () => __spreadValues(__spreadValues({}, this._def.shape()), augmentation)
      }));
    }
    /**
     * Prior to zod@1.0.12 there was a bug in the
     * inferred type of merged objects. Please
     * upgrade if you are experiencing issues.
     */
    merge(merging) {
      const merged = new _ZodObject({
        unknownKeys: merging._def.unknownKeys,
        catchall: merging._def.catchall,
        shape: () => __spreadValues(__spreadValues({}, this._def.shape()), merging._def.shape()),
        typeName: ZodFirstPartyTypeKind.ZodObject
      });
      return merged;
    }
    // merge<
    //   Incoming extends AnyZodObject,
    //   Augmentation extends Incoming["shape"],
    //   NewOutput extends {
    //     [k in keyof Augmentation | keyof Output]: k extends keyof Augmentation
    //       ? Augmentation[k]["_output"]
    //       : k extends keyof Output
    //       ? Output[k]
    //       : never;
    //   },
    //   NewInput extends {
    //     [k in keyof Augmentation | keyof Input]: k extends keyof Augmentation
    //       ? Augmentation[k]["_input"]
    //       : k extends keyof Input
    //       ? Input[k]
    //       : never;
    //   }
    // >(
    //   merging: Incoming
    // ): ZodObject<
    //   extendShape<T, ReturnType<Incoming["_def"]["shape"]>>,
    //   Incoming["_def"]["unknownKeys"],
    //   Incoming["_def"]["catchall"],
    //   NewOutput,
    //   NewInput
    // > {
    //   const merged: any = new ZodObject({
    //     unknownKeys: merging._def.unknownKeys,
    //     catchall: merging._def.catchall,
    //     shape: () =>
    //       objectUtil.mergeShapes(this._def.shape(), merging._def.shape()),
    //     typeName: ZodFirstPartyTypeKind.ZodObject,
    //   }) as any;
    //   return merged;
    // }
    setKey(key, schema) {
      return this.augment({ [key]: schema });
    }
    // merge<Incoming extends AnyZodObject>(
    //   merging: Incoming
    // ): //ZodObject<T & Incoming["_shape"], UnknownKeys, Catchall> = (merging) => {
    // ZodObject<
    //   extendShape<T, ReturnType<Incoming["_def"]["shape"]>>,
    //   Incoming["_def"]["unknownKeys"],
    //   Incoming["_def"]["catchall"]
    // > {
    //   // const mergedShape = objectUtil.mergeShapes(
    //   //   this._def.shape(),
    //   //   merging._def.shape()
    //   // );
    //   const merged: any = new ZodObject({
    //     unknownKeys: merging._def.unknownKeys,
    //     catchall: merging._def.catchall,
    //     shape: () =>
    //       objectUtil.mergeShapes(this._def.shape(), merging._def.shape()),
    //     typeName: ZodFirstPartyTypeKind.ZodObject,
    //   }) as any;
    //   return merged;
    // }
    catchall(index) {
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        catchall: index
      }));
    }
    pick(mask) {
      const shape = {};
      util.objectKeys(mask).forEach((key) => {
        if (mask[key] && this.shape[key]) {
          shape[key] = this.shape[key];
        }
      });
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        shape: () => shape
      }));
    }
    omit(mask) {
      const shape = {};
      util.objectKeys(this.shape).forEach((key) => {
        if (!mask[key]) {
          shape[key] = this.shape[key];
        }
      });
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        shape: () => shape
      }));
    }
    /**
     * @deprecated
     */
    deepPartial() {
      return deepPartialify(this);
    }
    partial(mask) {
      const newShape = {};
      util.objectKeys(this.shape).forEach((key) => {
        const fieldSchema = this.shape[key];
        if (mask && !mask[key]) {
          newShape[key] = fieldSchema;
        } else {
          newShape[key] = fieldSchema.optional();
        }
      });
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        shape: () => newShape
      }));
    }
    required(mask) {
      const newShape = {};
      util.objectKeys(this.shape).forEach((key) => {
        if (mask && !mask[key]) {
          newShape[key] = this.shape[key];
        } else {
          const fieldSchema = this.shape[key];
          let newField = fieldSchema;
          while (newField instanceof ZodOptional) {
            newField = newField._def.innerType;
          }
          newShape[key] = newField;
        }
      });
      return new _ZodObject(__spreadProps(__spreadValues({}, this._def), {
        shape: () => newShape
      }));
    }
    keyof() {
      return createZodEnum(util.objectKeys(this.shape));
    }
  };
  ZodObject.create = (shape, params) => {
    return new ZodObject(__spreadValues({
      shape: () => shape,
      unknownKeys: "strip",
      catchall: ZodNever.create(),
      typeName: ZodFirstPartyTypeKind.ZodObject
    }, processCreateParams(params)));
  };
  ZodObject.strictCreate = (shape, params) => {
    return new ZodObject(__spreadValues({
      shape: () => shape,
      unknownKeys: "strict",
      catchall: ZodNever.create(),
      typeName: ZodFirstPartyTypeKind.ZodObject
    }, processCreateParams(params)));
  };
  ZodObject.lazycreate = (shape, params) => {
    return new ZodObject(__spreadValues({
      shape,
      unknownKeys: "strip",
      catchall: ZodNever.create(),
      typeName: ZodFirstPartyTypeKind.ZodObject
    }, processCreateParams(params)));
  };
  var ZodUnion = class extends ZodType {
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      const options = this._def.options;
      function handleResults(results) {
        for (const result of results) {
          if (result.result.status === "valid") {
            return result.result;
          }
        }
        for (const result of results) {
          if (result.result.status === "dirty") {
            ctx.common.issues.push(...result.ctx.common.issues);
            return result.result;
          }
        }
        const unionErrors = results.map((result) => new ZodError(result.ctx.common.issues));
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_union,
          unionErrors
        });
        return INVALID;
      }
      if (ctx.common.async) {
        return Promise.all(options.map((option) => __async(this, null, function* () {
          const childCtx = __spreadProps(__spreadValues({}, ctx), {
            common: __spreadProps(__spreadValues({}, ctx.common), {
              issues: []
            }),
            parent: null
          });
          return {
            result: yield option._parseAsync({
              data: ctx.data,
              path: ctx.path,
              parent: childCtx
            }),
            ctx: childCtx
          };
        }))).then(handleResults);
      } else {
        let dirty = void 0;
        const issues = [];
        for (const option of options) {
          const childCtx = __spreadProps(__spreadValues({}, ctx), {
            common: __spreadProps(__spreadValues({}, ctx.common), {
              issues: []
            }),
            parent: null
          });
          const result = option._parseSync({
            data: ctx.data,
            path: ctx.path,
            parent: childCtx
          });
          if (result.status === "valid") {
            return result;
          } else if (result.status === "dirty" && !dirty) {
            dirty = { result, ctx: childCtx };
          }
          if (childCtx.common.issues.length) {
            issues.push(childCtx.common.issues);
          }
        }
        if (dirty) {
          ctx.common.issues.push(...dirty.ctx.common.issues);
          return dirty.result;
        }
        const unionErrors = issues.map((issues2) => new ZodError(issues2));
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_union,
          unionErrors
        });
        return INVALID;
      }
    }
    get options() {
      return this._def.options;
    }
  };
  ZodUnion.create = (types, params) => {
    return new ZodUnion(__spreadValues({
      options: types,
      typeName: ZodFirstPartyTypeKind.ZodUnion
    }, processCreateParams(params)));
  };
  var getDiscriminator = (type) => {
    if (type instanceof ZodLazy) {
      return getDiscriminator(type.schema);
    } else if (type instanceof ZodEffects) {
      return getDiscriminator(type.innerType());
    } else if (type instanceof ZodLiteral) {
      return [type.value];
    } else if (type instanceof ZodEnum) {
      return type.options;
    } else if (type instanceof ZodNativeEnum) {
      return util.objectValues(type.enum);
    } else if (type instanceof ZodDefault) {
      return getDiscriminator(type._def.innerType);
    } else if (type instanceof ZodUndefined) {
      return [void 0];
    } else if (type instanceof ZodNull) {
      return [null];
    } else if (type instanceof ZodOptional) {
      return [void 0, ...getDiscriminator(type.unwrap())];
    } else if (type instanceof ZodNullable) {
      return [null, ...getDiscriminator(type.unwrap())];
    } else if (type instanceof ZodBranded) {
      return getDiscriminator(type.unwrap());
    } else if (type instanceof ZodReadonly) {
      return getDiscriminator(type.unwrap());
    } else if (type instanceof ZodCatch) {
      return getDiscriminator(type._def.innerType);
    } else {
      return [];
    }
  };
  var ZodDiscriminatedUnion = class _ZodDiscriminatedUnion extends ZodType {
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.object) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.object,
          received: ctx.parsedType
        });
        return INVALID;
      }
      const discriminator = this.discriminator;
      const discriminatorValue = ctx.data[discriminator];
      const option = this.optionsMap.get(discriminatorValue);
      if (!option) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_union_discriminator,
          options: Array.from(this.optionsMap.keys()),
          path: [discriminator]
        });
        return INVALID;
      }
      if (ctx.common.async) {
        return option._parseAsync({
          data: ctx.data,
          path: ctx.path,
          parent: ctx
        });
      } else {
        return option._parseSync({
          data: ctx.data,
          path: ctx.path,
          parent: ctx
        });
      }
    }
    get discriminator() {
      return this._def.discriminator;
    }
    get options() {
      return this._def.options;
    }
    get optionsMap() {
      return this._def.optionsMap;
    }
    /**
     * The constructor of the discriminated union schema. Its behaviour is very similar to that of the normal z.union() constructor.
     * However, it only allows a union of objects, all of which need to share a discriminator property. This property must
     * have a different value for each object in the union.
     * @param discriminator the name of the discriminator property
     * @param types an array of object schemas
     * @param params
     */
    static create(discriminator, options, params) {
      const optionsMap = /* @__PURE__ */ new Map();
      for (const type of options) {
        const discriminatorValues = getDiscriminator(type.shape[discriminator]);
        if (!discriminatorValues.length) {
          throw new Error(`A discriminator value for key \`${discriminator}\` could not be extracted from all schema options`);
        }
        for (const value of discriminatorValues) {
          if (optionsMap.has(value)) {
            throw new Error(`Discriminator property ${String(discriminator)} has duplicate value ${String(value)}`);
          }
          optionsMap.set(value, type);
        }
      }
      return new _ZodDiscriminatedUnion(__spreadValues({
        typeName: ZodFirstPartyTypeKind.ZodDiscriminatedUnion,
        discriminator,
        options,
        optionsMap
      }, processCreateParams(params)));
    }
  };
  function mergeValues(a, b) {
    const aType = getParsedType(a);
    const bType = getParsedType(b);
    if (a === b) {
      return { valid: true, data: a };
    } else if (aType === ZodParsedType.object && bType === ZodParsedType.object) {
      const bKeys = util.objectKeys(b);
      const sharedKeys = util.objectKeys(a).filter((key) => bKeys.indexOf(key) !== -1);
      const newObj = __spreadValues(__spreadValues({}, a), b);
      for (const key of sharedKeys) {
        const sharedValue = mergeValues(a[key], b[key]);
        if (!sharedValue.valid) {
          return { valid: false };
        }
        newObj[key] = sharedValue.data;
      }
      return { valid: true, data: newObj };
    } else if (aType === ZodParsedType.array && bType === ZodParsedType.array) {
      if (a.length !== b.length) {
        return { valid: false };
      }
      const newArray = [];
      for (let index = 0; index < a.length; index++) {
        const itemA = a[index];
        const itemB = b[index];
        const sharedValue = mergeValues(itemA, itemB);
        if (!sharedValue.valid) {
          return { valid: false };
        }
        newArray.push(sharedValue.data);
      }
      return { valid: true, data: newArray };
    } else if (aType === ZodParsedType.date && bType === ZodParsedType.date && +a === +b) {
      return { valid: true, data: a };
    } else {
      return { valid: false };
    }
  }
  var ZodIntersection = class extends ZodType {
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      const handleParsed = (parsedLeft, parsedRight) => {
        if (isAborted(parsedLeft) || isAborted(parsedRight)) {
          return INVALID;
        }
        const merged = mergeValues(parsedLeft.value, parsedRight.value);
        if (!merged.valid) {
          addIssueToContext(ctx, {
            code: ZodIssueCode.invalid_intersection_types
          });
          return INVALID;
        }
        if (isDirty(parsedLeft) || isDirty(parsedRight)) {
          status.dirty();
        }
        return { status: status.value, value: merged.data };
      };
      if (ctx.common.async) {
        return Promise.all([
          this._def.left._parseAsync({
            data: ctx.data,
            path: ctx.path,
            parent: ctx
          }),
          this._def.right._parseAsync({
            data: ctx.data,
            path: ctx.path,
            parent: ctx
          })
        ]).then(([left, right]) => handleParsed(left, right));
      } else {
        return handleParsed(this._def.left._parseSync({
          data: ctx.data,
          path: ctx.path,
          parent: ctx
        }), this._def.right._parseSync({
          data: ctx.data,
          path: ctx.path,
          parent: ctx
        }));
      }
    }
  };
  ZodIntersection.create = (left, right, params) => {
    return new ZodIntersection(__spreadValues({
      left,
      right,
      typeName: ZodFirstPartyTypeKind.ZodIntersection
    }, processCreateParams(params)));
  };
  var ZodTuple = class _ZodTuple extends ZodType {
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.array) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.array,
          received: ctx.parsedType
        });
        return INVALID;
      }
      if (ctx.data.length < this._def.items.length) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.too_small,
          minimum: this._def.items.length,
          inclusive: true,
          exact: false,
          type: "array"
        });
        return INVALID;
      }
      const rest = this._def.rest;
      if (!rest && ctx.data.length > this._def.items.length) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.too_big,
          maximum: this._def.items.length,
          inclusive: true,
          exact: false,
          type: "array"
        });
        status.dirty();
      }
      const items = [...ctx.data].map((item, itemIndex) => {
        const schema = this._def.items[itemIndex] || this._def.rest;
        if (!schema)
          return null;
        return schema._parse(new ParseInputLazyPath(ctx, item, ctx.path, itemIndex));
      }).filter((x) => !!x);
      if (ctx.common.async) {
        return Promise.all(items).then((results) => {
          return ParseStatus.mergeArray(status, results);
        });
      } else {
        return ParseStatus.mergeArray(status, items);
      }
    }
    get items() {
      return this._def.items;
    }
    rest(rest) {
      return new _ZodTuple(__spreadProps(__spreadValues({}, this._def), {
        rest
      }));
    }
  };
  ZodTuple.create = (schemas, params) => {
    if (!Array.isArray(schemas)) {
      throw new Error("You must pass an array of schemas to z.tuple([ ... ])");
    }
    return new ZodTuple(__spreadValues({
      items: schemas,
      typeName: ZodFirstPartyTypeKind.ZodTuple,
      rest: null
    }, processCreateParams(params)));
  };
  var ZodRecord = class _ZodRecord extends ZodType {
    get keySchema() {
      return this._def.keyType;
    }
    get valueSchema() {
      return this._def.valueType;
    }
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.object) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.object,
          received: ctx.parsedType
        });
        return INVALID;
      }
      const pairs = [];
      const keyType = this._def.keyType;
      const valueType = this._def.valueType;
      for (const key in ctx.data) {
        pairs.push({
          key: keyType._parse(new ParseInputLazyPath(ctx, key, ctx.path, key)),
          value: valueType._parse(new ParseInputLazyPath(ctx, ctx.data[key], ctx.path, key)),
          alwaysSet: key in ctx.data
        });
      }
      if (ctx.common.async) {
        return ParseStatus.mergeObjectAsync(status, pairs);
      } else {
        return ParseStatus.mergeObjectSync(status, pairs);
      }
    }
    get element() {
      return this._def.valueType;
    }
    static create(first, second, third) {
      if (second instanceof ZodType) {
        return new _ZodRecord(__spreadValues({
          keyType: first,
          valueType: second,
          typeName: ZodFirstPartyTypeKind.ZodRecord
        }, processCreateParams(third)));
      }
      return new _ZodRecord(__spreadValues({
        keyType: ZodString.create(),
        valueType: first,
        typeName: ZodFirstPartyTypeKind.ZodRecord
      }, processCreateParams(second)));
    }
  };
  var ZodMap = class extends ZodType {
    get keySchema() {
      return this._def.keyType;
    }
    get valueSchema() {
      return this._def.valueType;
    }
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.map) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.map,
          received: ctx.parsedType
        });
        return INVALID;
      }
      const keyType = this._def.keyType;
      const valueType = this._def.valueType;
      const pairs = [...ctx.data.entries()].map(([key, value], index) => {
        return {
          key: keyType._parse(new ParseInputLazyPath(ctx, key, ctx.path, [index, "key"])),
          value: valueType._parse(new ParseInputLazyPath(ctx, value, ctx.path, [index, "value"]))
        };
      });
      if (ctx.common.async) {
        const finalMap = /* @__PURE__ */ new Map();
        return Promise.resolve().then(() => __async(this, null, function* () {
          for (const pair of pairs) {
            const key = yield pair.key;
            const value = yield pair.value;
            if (key.status === "aborted" || value.status === "aborted") {
              return INVALID;
            }
            if (key.status === "dirty" || value.status === "dirty") {
              status.dirty();
            }
            finalMap.set(key.value, value.value);
          }
          return { status: status.value, value: finalMap };
        }));
      } else {
        const finalMap = /* @__PURE__ */ new Map();
        for (const pair of pairs) {
          const key = pair.key;
          const value = pair.value;
          if (key.status === "aborted" || value.status === "aborted") {
            return INVALID;
          }
          if (key.status === "dirty" || value.status === "dirty") {
            status.dirty();
          }
          finalMap.set(key.value, value.value);
        }
        return { status: status.value, value: finalMap };
      }
    }
  };
  ZodMap.create = (keyType, valueType, params) => {
    return new ZodMap(__spreadValues({
      valueType,
      keyType,
      typeName: ZodFirstPartyTypeKind.ZodMap
    }, processCreateParams(params)));
  };
  var ZodSet = class _ZodSet extends ZodType {
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.set) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.set,
          received: ctx.parsedType
        });
        return INVALID;
      }
      const def = this._def;
      if (def.minSize !== null) {
        if (ctx.data.size < def.minSize.value) {
          addIssueToContext(ctx, {
            code: ZodIssueCode.too_small,
            minimum: def.minSize.value,
            type: "set",
            inclusive: true,
            exact: false,
            message: def.minSize.message
          });
          status.dirty();
        }
      }
      if (def.maxSize !== null) {
        if (ctx.data.size > def.maxSize.value) {
          addIssueToContext(ctx, {
            code: ZodIssueCode.too_big,
            maximum: def.maxSize.value,
            type: "set",
            inclusive: true,
            exact: false,
            message: def.maxSize.message
          });
          status.dirty();
        }
      }
      const valueType = this._def.valueType;
      function finalizeSet(elements2) {
        const parsedSet = /* @__PURE__ */ new Set();
        for (const element of elements2) {
          if (element.status === "aborted")
            return INVALID;
          if (element.status === "dirty")
            status.dirty();
          parsedSet.add(element.value);
        }
        return { status: status.value, value: parsedSet };
      }
      const elements = [...ctx.data.values()].map((item, i) => valueType._parse(new ParseInputLazyPath(ctx, item, ctx.path, i)));
      if (ctx.common.async) {
        return Promise.all(elements).then((elements2) => finalizeSet(elements2));
      } else {
        return finalizeSet(elements);
      }
    }
    min(minSize, message) {
      return new _ZodSet(__spreadProps(__spreadValues({}, this._def), {
        minSize: { value: minSize, message: errorUtil.toString(message) }
      }));
    }
    max(maxSize, message) {
      return new _ZodSet(__spreadProps(__spreadValues({}, this._def), {
        maxSize: { value: maxSize, message: errorUtil.toString(message) }
      }));
    }
    size(size, message) {
      return this.min(size, message).max(size, message);
    }
    nonempty(message) {
      return this.min(1, message);
    }
  };
  ZodSet.create = (valueType, params) => {
    return new ZodSet(__spreadValues({
      valueType,
      minSize: null,
      maxSize: null,
      typeName: ZodFirstPartyTypeKind.ZodSet
    }, processCreateParams(params)));
  };
  var ZodFunction = class _ZodFunction extends ZodType {
    constructor() {
      super(...arguments);
      this.validate = this.implement;
    }
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.function) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.function,
          received: ctx.parsedType
        });
        return INVALID;
      }
      function makeArgsIssue(args, error) {
        return makeIssue({
          data: args,
          path: ctx.path,
          errorMaps: [
            ctx.common.contextualErrorMap,
            ctx.schemaErrorMap,
            getErrorMap(),
            errorMap
          ].filter((x) => !!x),
          issueData: {
            code: ZodIssueCode.invalid_arguments,
            argumentsError: error
          }
        });
      }
      function makeReturnsIssue(returns, error) {
        return makeIssue({
          data: returns,
          path: ctx.path,
          errorMaps: [
            ctx.common.contextualErrorMap,
            ctx.schemaErrorMap,
            getErrorMap(),
            errorMap
          ].filter((x) => !!x),
          issueData: {
            code: ZodIssueCode.invalid_return_type,
            returnTypeError: error
          }
        });
      }
      const params = { errorMap: ctx.common.contextualErrorMap };
      const fn2 = ctx.data;
      if (this._def.returns instanceof ZodPromise) {
        const me = this;
        return OK(function(...args) {
          return __async(this, null, function* () {
            const error = new ZodError([]);
            const parsedArgs = yield me._def.args.parseAsync(args, params).catch((e) => {
              error.addIssue(makeArgsIssue(args, e));
              throw error;
            });
            const result = yield Reflect.apply(fn2, this, parsedArgs);
            const parsedReturns = yield me._def.returns._def.type.parseAsync(result, params).catch((e) => {
              error.addIssue(makeReturnsIssue(result, e));
              throw error;
            });
            return parsedReturns;
          });
        });
      } else {
        const me = this;
        return OK(function(...args) {
          const parsedArgs = me._def.args.safeParse(args, params);
          if (!parsedArgs.success) {
            throw new ZodError([makeArgsIssue(args, parsedArgs.error)]);
          }
          const result = Reflect.apply(fn2, this, parsedArgs.data);
          const parsedReturns = me._def.returns.safeParse(result, params);
          if (!parsedReturns.success) {
            throw new ZodError([makeReturnsIssue(result, parsedReturns.error)]);
          }
          return parsedReturns.data;
        });
      }
    }
    parameters() {
      return this._def.args;
    }
    returnType() {
      return this._def.returns;
    }
    args(...items) {
      return new _ZodFunction(__spreadProps(__spreadValues({}, this._def), {
        args: ZodTuple.create(items).rest(ZodUnknown.create())
      }));
    }
    returns(returnType) {
      return new _ZodFunction(__spreadProps(__spreadValues({}, this._def), {
        returns: returnType
      }));
    }
    implement(func) {
      const validatedFunc = this.parse(func);
      return validatedFunc;
    }
    strictImplement(func) {
      const validatedFunc = this.parse(func);
      return validatedFunc;
    }
    static create(args, returns, params) {
      return new _ZodFunction(__spreadValues({
        args: args ? args : ZodTuple.create([]).rest(ZodUnknown.create()),
        returns: returns || ZodUnknown.create(),
        typeName: ZodFirstPartyTypeKind.ZodFunction
      }, processCreateParams(params)));
    }
  };
  var ZodLazy = class extends ZodType {
    get schema() {
      return this._def.getter();
    }
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      const lazySchema = this._def.getter();
      return lazySchema._parse({ data: ctx.data, path: ctx.path, parent: ctx });
    }
  };
  ZodLazy.create = (getter, params) => {
    return new ZodLazy(__spreadValues({
      getter,
      typeName: ZodFirstPartyTypeKind.ZodLazy
    }, processCreateParams(params)));
  };
  var ZodLiteral = class extends ZodType {
    _parse(input) {
      if (input.data !== this._def.value) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          received: ctx.data,
          code: ZodIssueCode.invalid_literal,
          expected: this._def.value
        });
        return INVALID;
      }
      return { status: "valid", value: input.data };
    }
    get value() {
      return this._def.value;
    }
  };
  ZodLiteral.create = (value, params) => {
    return new ZodLiteral(__spreadValues({
      value,
      typeName: ZodFirstPartyTypeKind.ZodLiteral
    }, processCreateParams(params)));
  };
  function createZodEnum(values, params) {
    return new ZodEnum(__spreadValues({
      values,
      typeName: ZodFirstPartyTypeKind.ZodEnum
    }, processCreateParams(params)));
  }
  var ZodEnum = class _ZodEnum extends ZodType {
    constructor() {
      super(...arguments);
      _ZodEnum_cache.set(this, void 0);
    }
    _parse(input) {
      if (typeof input.data !== "string") {
        const ctx = this._getOrReturnCtx(input);
        const expectedValues = this._def.values;
        addIssueToContext(ctx, {
          expected: util.joinValues(expectedValues),
          received: ctx.parsedType,
          code: ZodIssueCode.invalid_type
        });
        return INVALID;
      }
      if (!__classPrivateFieldGet(this, _ZodEnum_cache, "f")) {
        __classPrivateFieldSet(this, _ZodEnum_cache, new Set(this._def.values), "f");
      }
      if (!__classPrivateFieldGet(this, _ZodEnum_cache, "f").has(input.data)) {
        const ctx = this._getOrReturnCtx(input);
        const expectedValues = this._def.values;
        addIssueToContext(ctx, {
          received: ctx.data,
          code: ZodIssueCode.invalid_enum_value,
          options: expectedValues
        });
        return INVALID;
      }
      return OK(input.data);
    }
    get options() {
      return this._def.values;
    }
    get enum() {
      const enumValues = {};
      for (const val of this._def.values) {
        enumValues[val] = val;
      }
      return enumValues;
    }
    get Values() {
      const enumValues = {};
      for (const val of this._def.values) {
        enumValues[val] = val;
      }
      return enumValues;
    }
    get Enum() {
      const enumValues = {};
      for (const val of this._def.values) {
        enumValues[val] = val;
      }
      return enumValues;
    }
    extract(values, newDef = this._def) {
      return _ZodEnum.create(values, __spreadValues(__spreadValues({}, this._def), newDef));
    }
    exclude(values, newDef = this._def) {
      return _ZodEnum.create(this.options.filter((opt) => !values.includes(opt)), __spreadValues(__spreadValues({}, this._def), newDef));
    }
  };
  _ZodEnum_cache = /* @__PURE__ */ new WeakMap();
  ZodEnum.create = createZodEnum;
  var ZodNativeEnum = class extends ZodType {
    constructor() {
      super(...arguments);
      _ZodNativeEnum_cache.set(this, void 0);
    }
    _parse(input) {
      const nativeEnumValues = util.getValidEnumValues(this._def.values);
      const ctx = this._getOrReturnCtx(input);
      if (ctx.parsedType !== ZodParsedType.string && ctx.parsedType !== ZodParsedType.number) {
        const expectedValues = util.objectValues(nativeEnumValues);
        addIssueToContext(ctx, {
          expected: util.joinValues(expectedValues),
          received: ctx.parsedType,
          code: ZodIssueCode.invalid_type
        });
        return INVALID;
      }
      if (!__classPrivateFieldGet(this, _ZodNativeEnum_cache, "f")) {
        __classPrivateFieldSet(this, _ZodNativeEnum_cache, new Set(util.getValidEnumValues(this._def.values)), "f");
      }
      if (!__classPrivateFieldGet(this, _ZodNativeEnum_cache, "f").has(input.data)) {
        const expectedValues = util.objectValues(nativeEnumValues);
        addIssueToContext(ctx, {
          received: ctx.data,
          code: ZodIssueCode.invalid_enum_value,
          options: expectedValues
        });
        return INVALID;
      }
      return OK(input.data);
    }
    get enum() {
      return this._def.values;
    }
  };
  _ZodNativeEnum_cache = /* @__PURE__ */ new WeakMap();
  ZodNativeEnum.create = (values, params) => {
    return new ZodNativeEnum(__spreadValues({
      values,
      typeName: ZodFirstPartyTypeKind.ZodNativeEnum
    }, processCreateParams(params)));
  };
  var ZodPromise = class extends ZodType {
    unwrap() {
      return this._def.type;
    }
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      if (ctx.parsedType !== ZodParsedType.promise && ctx.common.async === false) {
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.promise,
          received: ctx.parsedType
        });
        return INVALID;
      }
      const promisified = ctx.parsedType === ZodParsedType.promise ? ctx.data : Promise.resolve(ctx.data);
      return OK(promisified.then((data) => {
        return this._def.type.parseAsync(data, {
          path: ctx.path,
          errorMap: ctx.common.contextualErrorMap
        });
      }));
    }
  };
  ZodPromise.create = (schema, params) => {
    return new ZodPromise(__spreadValues({
      type: schema,
      typeName: ZodFirstPartyTypeKind.ZodPromise
    }, processCreateParams(params)));
  };
  var ZodEffects = class extends ZodType {
    innerType() {
      return this._def.schema;
    }
    sourceType() {
      return this._def.schema._def.typeName === ZodFirstPartyTypeKind.ZodEffects ? this._def.schema.sourceType() : this._def.schema;
    }
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      const effect = this._def.effect || null;
      const checkCtx = {
        addIssue: (arg) => {
          addIssueToContext(ctx, arg);
          if (arg.fatal) {
            status.abort();
          } else {
            status.dirty();
          }
        },
        get path() {
          return ctx.path;
        }
      };
      checkCtx.addIssue = checkCtx.addIssue.bind(checkCtx);
      if (effect.type === "preprocess") {
        const processed = effect.transform(ctx.data, checkCtx);
        if (ctx.common.async) {
          return Promise.resolve(processed).then((processed2) => __async(this, null, function* () {
            if (status.value === "aborted")
              return INVALID;
            const result = yield this._def.schema._parseAsync({
              data: processed2,
              path: ctx.path,
              parent: ctx
            });
            if (result.status === "aborted")
              return INVALID;
            if (result.status === "dirty")
              return DIRTY(result.value);
            if (status.value === "dirty")
              return DIRTY(result.value);
            return result;
          }));
        } else {
          if (status.value === "aborted")
            return INVALID;
          const result = this._def.schema._parseSync({
            data: processed,
            path: ctx.path,
            parent: ctx
          });
          if (result.status === "aborted")
            return INVALID;
          if (result.status === "dirty")
            return DIRTY(result.value);
          if (status.value === "dirty")
            return DIRTY(result.value);
          return result;
        }
      }
      if (effect.type === "refinement") {
        const executeRefinement = (acc) => {
          const result = effect.refinement(acc, checkCtx);
          if (ctx.common.async) {
            return Promise.resolve(result);
          }
          if (result instanceof Promise) {
            throw new Error("Async refinement encountered during synchronous parse operation. Use .parseAsync instead.");
          }
          return acc;
        };
        if (ctx.common.async === false) {
          const inner = this._def.schema._parseSync({
            data: ctx.data,
            path: ctx.path,
            parent: ctx
          });
          if (inner.status === "aborted")
            return INVALID;
          if (inner.status === "dirty")
            status.dirty();
          executeRefinement(inner.value);
          return { status: status.value, value: inner.value };
        } else {
          return this._def.schema._parseAsync({ data: ctx.data, path: ctx.path, parent: ctx }).then((inner) => {
            if (inner.status === "aborted")
              return INVALID;
            if (inner.status === "dirty")
              status.dirty();
            return executeRefinement(inner.value).then(() => {
              return { status: status.value, value: inner.value };
            });
          });
        }
      }
      if (effect.type === "transform") {
        if (ctx.common.async === false) {
          const base = this._def.schema._parseSync({
            data: ctx.data,
            path: ctx.path,
            parent: ctx
          });
          if (!isValid(base))
            return base;
          const result = effect.transform(base.value, checkCtx);
          if (result instanceof Promise) {
            throw new Error(`Asynchronous transform encountered during synchronous parse operation. Use .parseAsync instead.`);
          }
          return { status: status.value, value: result };
        } else {
          return this._def.schema._parseAsync({ data: ctx.data, path: ctx.path, parent: ctx }).then((base) => {
            if (!isValid(base))
              return base;
            return Promise.resolve(effect.transform(base.value, checkCtx)).then((result) => ({ status: status.value, value: result }));
          });
        }
      }
      util.assertNever(effect);
    }
  };
  ZodEffects.create = (schema, effect, params) => {
    return new ZodEffects(__spreadValues({
      schema,
      typeName: ZodFirstPartyTypeKind.ZodEffects,
      effect
    }, processCreateParams(params)));
  };
  ZodEffects.createWithPreprocess = (preprocess, schema, params) => {
    return new ZodEffects(__spreadValues({
      schema,
      effect: { type: "preprocess", transform: preprocess },
      typeName: ZodFirstPartyTypeKind.ZodEffects
    }, processCreateParams(params)));
  };
  var ZodOptional = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType === ZodParsedType.undefined) {
        return OK(void 0);
      }
      return this._def.innerType._parse(input);
    }
    unwrap() {
      return this._def.innerType;
    }
  };
  ZodOptional.create = (type, params) => {
    return new ZodOptional(__spreadValues({
      innerType: type,
      typeName: ZodFirstPartyTypeKind.ZodOptional
    }, processCreateParams(params)));
  };
  var ZodNullable = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType === ZodParsedType.null) {
        return OK(null);
      }
      return this._def.innerType._parse(input);
    }
    unwrap() {
      return this._def.innerType;
    }
  };
  ZodNullable.create = (type, params) => {
    return new ZodNullable(__spreadValues({
      innerType: type,
      typeName: ZodFirstPartyTypeKind.ZodNullable
    }, processCreateParams(params)));
  };
  var ZodDefault = class extends ZodType {
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      let data = ctx.data;
      if (ctx.parsedType === ZodParsedType.undefined) {
        data = this._def.defaultValue();
      }
      return this._def.innerType._parse({
        data,
        path: ctx.path,
        parent: ctx
      });
    }
    removeDefault() {
      return this._def.innerType;
    }
  };
  ZodDefault.create = (type, params) => {
    return new ZodDefault(__spreadValues({
      innerType: type,
      typeName: ZodFirstPartyTypeKind.ZodDefault,
      defaultValue: typeof params.default === "function" ? params.default : () => params.default
    }, processCreateParams(params)));
  };
  var ZodCatch = class extends ZodType {
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      const newCtx = __spreadProps(__spreadValues({}, ctx), {
        common: __spreadProps(__spreadValues({}, ctx.common), {
          issues: []
        })
      });
      const result = this._def.innerType._parse({
        data: newCtx.data,
        path: newCtx.path,
        parent: __spreadValues({}, newCtx)
      });
      if (isAsync(result)) {
        return result.then((result2) => {
          return {
            status: "valid",
            value: result2.status === "valid" ? result2.value : this._def.catchValue({
              get error() {
                return new ZodError(newCtx.common.issues);
              },
              input: newCtx.data
            })
          };
        });
      } else {
        return {
          status: "valid",
          value: result.status === "valid" ? result.value : this._def.catchValue({
            get error() {
              return new ZodError(newCtx.common.issues);
            },
            input: newCtx.data
          })
        };
      }
    }
    removeCatch() {
      return this._def.innerType;
    }
  };
  ZodCatch.create = (type, params) => {
    return new ZodCatch(__spreadValues({
      innerType: type,
      typeName: ZodFirstPartyTypeKind.ZodCatch,
      catchValue: typeof params.catch === "function" ? params.catch : () => params.catch
    }, processCreateParams(params)));
  };
  var ZodNaN = class extends ZodType {
    _parse(input) {
      const parsedType = this._getType(input);
      if (parsedType !== ZodParsedType.nan) {
        const ctx = this._getOrReturnCtx(input);
        addIssueToContext(ctx, {
          code: ZodIssueCode.invalid_type,
          expected: ZodParsedType.nan,
          received: ctx.parsedType
        });
        return INVALID;
      }
      return { status: "valid", value: input.data };
    }
  };
  ZodNaN.create = (params) => {
    return new ZodNaN(__spreadValues({
      typeName: ZodFirstPartyTypeKind.ZodNaN
    }, processCreateParams(params)));
  };
  var BRAND = Symbol("zod_brand");
  var ZodBranded = class extends ZodType {
    _parse(input) {
      const { ctx } = this._processInputParams(input);
      const data = ctx.data;
      return this._def.type._parse({
        data,
        path: ctx.path,
        parent: ctx
      });
    }
    unwrap() {
      return this._def.type;
    }
  };
  var ZodPipeline = class _ZodPipeline extends ZodType {
    _parse(input) {
      const { status, ctx } = this._processInputParams(input);
      if (ctx.common.async) {
        const handleAsync = () => __async(this, null, function* () {
          const inResult = yield this._def.in._parseAsync({
            data: ctx.data,
            path: ctx.path,
            parent: ctx
          });
          if (inResult.status === "aborted")
            return INVALID;
          if (inResult.status === "dirty") {
            status.dirty();
            return DIRTY(inResult.value);
          } else {
            return this._def.out._parseAsync({
              data: inResult.value,
              path: ctx.path,
              parent: ctx
            });
          }
        });
        return handleAsync();
      } else {
        const inResult = this._def.in._parseSync({
          data: ctx.data,
          path: ctx.path,
          parent: ctx
        });
        if (inResult.status === "aborted")
          return INVALID;
        if (inResult.status === "dirty") {
          status.dirty();
          return {
            status: "dirty",
            value: inResult.value
          };
        } else {
          return this._def.out._parseSync({
            data: inResult.value,
            path: ctx.path,
            parent: ctx
          });
        }
      }
    }
    static create(a, b) {
      return new _ZodPipeline({
        in: a,
        out: b,
        typeName: ZodFirstPartyTypeKind.ZodPipeline
      });
    }
  };
  var ZodReadonly = class extends ZodType {
    _parse(input) {
      const result = this._def.innerType._parse(input);
      const freeze = (data) => {
        if (isValid(data)) {
          data.value = Object.freeze(data.value);
        }
        return data;
      };
      return isAsync(result) ? result.then((data) => freeze(data)) : freeze(result);
    }
    unwrap() {
      return this._def.innerType;
    }
  };
  ZodReadonly.create = (type, params) => {
    return new ZodReadonly(__spreadValues({
      innerType: type,
      typeName: ZodFirstPartyTypeKind.ZodReadonly
    }, processCreateParams(params)));
  };
  function cleanParams(params, data) {
    const p = typeof params === "function" ? params(data) : typeof params === "string" ? { message: params } : params;
    const p2 = typeof p === "string" ? { message: p } : p;
    return p2;
  }
  function custom(check, _params = {}, fatal) {
    if (check)
      return ZodAny.create().superRefine((data, ctx) => {
        var _a, _b;
        const r = check(data);
        if (r instanceof Promise) {
          return r.then((r2) => {
            var _a2, _b2;
            if (!r2) {
              const params = cleanParams(_params, data);
              const _fatal = (_b2 = (_a2 = params.fatal) !== null && _a2 !== void 0 ? _a2 : fatal) !== null && _b2 !== void 0 ? _b2 : true;
              ctx.addIssue(__spreadProps(__spreadValues({ code: "custom" }, params), { fatal: _fatal }));
            }
          });
        }
        if (!r) {
          const params = cleanParams(_params, data);
          const _fatal = (_b = (_a = params.fatal) !== null && _a !== void 0 ? _a : fatal) !== null && _b !== void 0 ? _b : true;
          ctx.addIssue(__spreadProps(__spreadValues({ code: "custom" }, params), { fatal: _fatal }));
        }
        return;
      });
    return ZodAny.create();
  }
  var late = {
    object: ZodObject.lazycreate
  };
  var ZodFirstPartyTypeKind;
  (function(ZodFirstPartyTypeKind2) {
    ZodFirstPartyTypeKind2["ZodString"] = "ZodString";
    ZodFirstPartyTypeKind2["ZodNumber"] = "ZodNumber";
    ZodFirstPartyTypeKind2["ZodNaN"] = "ZodNaN";
    ZodFirstPartyTypeKind2["ZodBigInt"] = "ZodBigInt";
    ZodFirstPartyTypeKind2["ZodBoolean"] = "ZodBoolean";
    ZodFirstPartyTypeKind2["ZodDate"] = "ZodDate";
    ZodFirstPartyTypeKind2["ZodSymbol"] = "ZodSymbol";
    ZodFirstPartyTypeKind2["ZodUndefined"] = "ZodUndefined";
    ZodFirstPartyTypeKind2["ZodNull"] = "ZodNull";
    ZodFirstPartyTypeKind2["ZodAny"] = "ZodAny";
    ZodFirstPartyTypeKind2["ZodUnknown"] = "ZodUnknown";
    ZodFirstPartyTypeKind2["ZodNever"] = "ZodNever";
    ZodFirstPartyTypeKind2["ZodVoid"] = "ZodVoid";
    ZodFirstPartyTypeKind2["ZodArray"] = "ZodArray";
    ZodFirstPartyTypeKind2["ZodObject"] = "ZodObject";
    ZodFirstPartyTypeKind2["ZodUnion"] = "ZodUnion";
    ZodFirstPartyTypeKind2["ZodDiscriminatedUnion"] = "ZodDiscriminatedUnion";
    ZodFirstPartyTypeKind2["ZodIntersection"] = "ZodIntersection";
    ZodFirstPartyTypeKind2["ZodTuple"] = "ZodTuple";
    ZodFirstPartyTypeKind2["ZodRecord"] = "ZodRecord";
    ZodFirstPartyTypeKind2["ZodMap"] = "ZodMap";
    ZodFirstPartyTypeKind2["ZodSet"] = "ZodSet";
    ZodFirstPartyTypeKind2["ZodFunction"] = "ZodFunction";
    ZodFirstPartyTypeKind2["ZodLazy"] = "ZodLazy";
    ZodFirstPartyTypeKind2["ZodLiteral"] = "ZodLiteral";
    ZodFirstPartyTypeKind2["ZodEnum"] = "ZodEnum";
    ZodFirstPartyTypeKind2["ZodEffects"] = "ZodEffects";
    ZodFirstPartyTypeKind2["ZodNativeEnum"] = "ZodNativeEnum";
    ZodFirstPartyTypeKind2["ZodOptional"] = "ZodOptional";
    ZodFirstPartyTypeKind2["ZodNullable"] = "ZodNullable";
    ZodFirstPartyTypeKind2["ZodDefault"] = "ZodDefault";
    ZodFirstPartyTypeKind2["ZodCatch"] = "ZodCatch";
    ZodFirstPartyTypeKind2["ZodPromise"] = "ZodPromise";
    ZodFirstPartyTypeKind2["ZodBranded"] = "ZodBranded";
    ZodFirstPartyTypeKind2["ZodPipeline"] = "ZodPipeline";
    ZodFirstPartyTypeKind2["ZodReadonly"] = "ZodReadonly";
  })(ZodFirstPartyTypeKind || (ZodFirstPartyTypeKind = {}));
  var instanceOfType = (cls, params = {
    message: `Input not instance of ${cls.name}`
  }) => custom((data) => data instanceof cls, params);
  var stringType = ZodString.create;
  var numberType = ZodNumber.create;
  var nanType = ZodNaN.create;
  var bigIntType = ZodBigInt.create;
  var booleanType = ZodBoolean.create;
  var dateType = ZodDate.create;
  var symbolType = ZodSymbol.create;
  var undefinedType = ZodUndefined.create;
  var nullType = ZodNull.create;
  var anyType = ZodAny.create;
  var unknownType = ZodUnknown.create;
  var neverType = ZodNever.create;
  var voidType = ZodVoid.create;
  var arrayType = ZodArray.create;
  var objectType = ZodObject.create;
  var strictObjectType = ZodObject.strictCreate;
  var unionType = ZodUnion.create;
  var discriminatedUnionType = ZodDiscriminatedUnion.create;
  var intersectionType = ZodIntersection.create;
  var tupleType = ZodTuple.create;
  var recordType = ZodRecord.create;
  var mapType = ZodMap.create;
  var setType = ZodSet.create;
  var functionType = ZodFunction.create;
  var lazyType = ZodLazy.create;
  var literalType = ZodLiteral.create;
  var enumType = ZodEnum.create;
  var nativeEnumType = ZodNativeEnum.create;
  var promiseType = ZodPromise.create;
  var effectsType = ZodEffects.create;
  var optionalType = ZodOptional.create;
  var nullableType = ZodNullable.create;
  var preprocessType = ZodEffects.createWithPreprocess;
  var pipelineType = ZodPipeline.create;
  var ostring = () => stringType().optional();
  var onumber = () => numberType().optional();
  var oboolean = () => booleanType().optional();
  var coerce = {
    string: (arg) => ZodString.create(__spreadProps(__spreadValues({}, arg), { coerce: true })),
    number: (arg) => ZodNumber.create(__spreadProps(__spreadValues({}, arg), { coerce: true })),
    boolean: (arg) => ZodBoolean.create(__spreadProps(__spreadValues({}, arg), {
      coerce: true
    })),
    bigint: (arg) => ZodBigInt.create(__spreadProps(__spreadValues({}, arg), { coerce: true })),
    date: (arg) => ZodDate.create(__spreadProps(__spreadValues({}, arg), { coerce: true }))
  };
  var NEVER = INVALID;
  var z = /* @__PURE__ */ Object.freeze({
    __proto__: null,
    defaultErrorMap: errorMap,
    setErrorMap,
    getErrorMap,
    makeIssue,
    EMPTY_PATH,
    addIssueToContext,
    ParseStatus,
    INVALID,
    DIRTY,
    OK,
    isAborted,
    isDirty,
    isValid,
    isAsync,
    get util() {
      return util;
    },
    get objectUtil() {
      return objectUtil;
    },
    ZodParsedType,
    getParsedType,
    ZodType,
    datetimeRegex,
    ZodString,
    ZodNumber,
    ZodBigInt,
    ZodBoolean,
    ZodDate,
    ZodSymbol,
    ZodUndefined,
    ZodNull,
    ZodAny,
    ZodUnknown,
    ZodNever,
    ZodVoid,
    ZodArray,
    ZodObject,
    ZodUnion,
    ZodDiscriminatedUnion,
    ZodIntersection,
    ZodTuple,
    ZodRecord,
    ZodMap,
    ZodSet,
    ZodFunction,
    ZodLazy,
    ZodLiteral,
    ZodEnum,
    ZodNativeEnum,
    ZodPromise,
    ZodEffects,
    ZodTransformer: ZodEffects,
    ZodOptional,
    ZodNullable,
    ZodDefault,
    ZodCatch,
    ZodNaN,
    BRAND,
    ZodBranded,
    ZodPipeline,
    ZodReadonly,
    custom,
    Schema: ZodType,
    ZodSchema: ZodType,
    late,
    get ZodFirstPartyTypeKind() {
      return ZodFirstPartyTypeKind;
    },
    coerce,
    any: anyType,
    array: arrayType,
    bigint: bigIntType,
    boolean: booleanType,
    date: dateType,
    discriminatedUnion: discriminatedUnionType,
    effect: effectsType,
    "enum": enumType,
    "function": functionType,
    "instanceof": instanceOfType,
    intersection: intersectionType,
    lazy: lazyType,
    literal: literalType,
    map: mapType,
    nan: nanType,
    nativeEnum: nativeEnumType,
    never: neverType,
    "null": nullType,
    nullable: nullableType,
    number: numberType,
    object: objectType,
    oboolean,
    onumber,
    optional: optionalType,
    ostring,
    pipeline: pipelineType,
    preprocess: preprocessType,
    promise: promiseType,
    record: recordType,
    set: setType,
    strictObject: strictObjectType,
    string: stringType,
    symbol: symbolType,
    transformer: effectsType,
    tuple: tupleType,
    "undefined": undefinedType,
    union: unionType,
    unknown: unknownType,
    "void": voidType,
    NEVER,
    ZodIssueCode,
    quotelessJson,
    ZodError
  });

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/any.js
  function parseAnyDef() {
    return {};
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/array.js
  function parseArrayDef(def, refs) {
    var _a, _b, _c;
    const res = {
      type: "array"
    };
    if (((_a = def.type) == null ? void 0 : _a._def) && ((_c = (_b = def.type) == null ? void 0 : _b._def) == null ? void 0 : _c.typeName) !== ZodFirstPartyTypeKind.ZodAny) {
      res.items = parseDef(def.type._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath, "items"]
      }));
    }
    if (def.minLength) {
      setResponseValueAndErrors(res, "minItems", def.minLength.value, def.minLength.message, refs);
    }
    if (def.maxLength) {
      setResponseValueAndErrors(res, "maxItems", def.maxLength.value, def.maxLength.message, refs);
    }
    if (def.exactLength) {
      setResponseValueAndErrors(res, "minItems", def.exactLength.value, def.exactLength.message, refs);
      setResponseValueAndErrors(res, "maxItems", def.exactLength.value, def.exactLength.message, refs);
    }
    return res;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/bigint.js
  function parseBigintDef(def, refs) {
    const res = {
      type: "integer",
      format: "int64"
    };
    if (!def.checks)
      return res;
    for (const check of def.checks) {
      switch (check.kind) {
        case "min":
          if (refs.target === "jsonSchema7") {
            if (check.inclusive) {
              setResponseValueAndErrors(res, "minimum", check.value, check.message, refs);
            } else {
              setResponseValueAndErrors(res, "exclusiveMinimum", check.value, check.message, refs);
            }
          } else {
            if (!check.inclusive) {
              res.exclusiveMinimum = true;
            }
            setResponseValueAndErrors(res, "minimum", check.value, check.message, refs);
          }
          break;
        case "max":
          if (refs.target === "jsonSchema7") {
            if (check.inclusive) {
              setResponseValueAndErrors(res, "maximum", check.value, check.message, refs);
            } else {
              setResponseValueAndErrors(res, "exclusiveMaximum", check.value, check.message, refs);
            }
          } else {
            if (!check.inclusive) {
              res.exclusiveMaximum = true;
            }
            setResponseValueAndErrors(res, "maximum", check.value, check.message, refs);
          }
          break;
        case "multipleOf":
          setResponseValueAndErrors(res, "multipleOf", check.value, check.message, refs);
          break;
      }
    }
    return res;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/boolean.js
  function parseBooleanDef() {
    return {
      type: "boolean"
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/branded.js
  function parseBrandedDef(_def, refs) {
    return parseDef(_def.type._def, refs);
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/catch.js
  var parseCatchDef = (def, refs) => {
    return parseDef(def.innerType._def, refs);
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/date.js
  function parseDateDef(def, refs, overrideDateStrategy) {
    const strategy = overrideDateStrategy != null ? overrideDateStrategy : refs.dateStrategy;
    if (Array.isArray(strategy)) {
      return {
        anyOf: strategy.map((item, i) => parseDateDef(def, refs, item))
      };
    }
    switch (strategy) {
      case "string":
      case "format:date-time":
        return {
          type: "string",
          format: "date-time"
        };
      case "format:date":
        return {
          type: "string",
          format: "date"
        };
      case "integer":
        return integerDateParser(def, refs);
    }
  }
  var integerDateParser = (def, refs) => {
    const res = {
      type: "integer",
      format: "unix-time"
    };
    if (refs.target === "openApi3") {
      return res;
    }
    for (const check of def.checks) {
      switch (check.kind) {
        case "min":
          setResponseValueAndErrors(
            res,
            "minimum",
            check.value,
            // This is in milliseconds
            check.message,
            refs
          );
          break;
        case "max":
          setResponseValueAndErrors(
            res,
            "maximum",
            check.value,
            // This is in milliseconds
            check.message,
            refs
          );
          break;
      }
    }
    return res;
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/default.js
  function parseDefaultDef(_def, refs) {
    return __spreadProps(__spreadValues({}, parseDef(_def.innerType._def, refs)), {
      default: _def.defaultValue()
    });
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/effects.js
  function parseEffectsDef(_def, refs) {
    return refs.effectStrategy === "input" ? parseDef(_def.schema._def, refs) : {};
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/enum.js
  function parseEnumDef(def) {
    return {
      type: "string",
      enum: Array.from(def.values)
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/intersection.js
  var isJsonSchema7AllOfType = (type) => {
    if ("type" in type && type.type === "string")
      return false;
    return "allOf" in type;
  };
  function parseIntersectionDef(def, refs) {
    const allOf = [
      parseDef(def.left._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath, "allOf", "0"]
      })),
      parseDef(def.right._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath, "allOf", "1"]
      }))
    ].filter((x) => !!x);
    let unevaluatedProperties = refs.target === "jsonSchema2019-09" ? { unevaluatedProperties: false } : void 0;
    const mergedAllOf = [];
    allOf.forEach((schema) => {
      if (isJsonSchema7AllOfType(schema)) {
        mergedAllOf.push(...schema.allOf);
        if (schema.unevaluatedProperties === void 0) {
          unevaluatedProperties = void 0;
        }
      } else {
        let nestedSchema = schema;
        if ("additionalProperties" in schema && schema.additionalProperties === false) {
          const _a = schema, { additionalProperties } = _a, rest = __objRest(_a, ["additionalProperties"]);
          nestedSchema = rest;
        } else {
          unevaluatedProperties = void 0;
        }
        mergedAllOf.push(nestedSchema);
      }
    });
    return mergedAllOf.length ? __spreadValues({
      allOf: mergedAllOf
    }, unevaluatedProperties) : void 0;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/literal.js
  function parseLiteralDef(def, refs) {
    const parsedType = typeof def.value;
    if (parsedType !== "bigint" && parsedType !== "number" && parsedType !== "boolean" && parsedType !== "string") {
      return {
        type: Array.isArray(def.value) ? "array" : "object"
      };
    }
    if (refs.target === "openApi3") {
      return {
        type: parsedType === "bigint" ? "integer" : parsedType,
        enum: [def.value]
      };
    }
    return {
      type: parsedType === "bigint" ? "integer" : parsedType,
      const: def.value
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/string.js
  var emojiRegex2 = void 0;
  var zodPatterns = {
    /**
     * `c` was changed to `[cC]` to replicate /i flag
     */
    cuid: /^[cC][^\s-]{8,}$/,
    cuid2: /^[0-9a-z]+$/,
    ulid: /^[0-9A-HJKMNP-TV-Z]{26}$/,
    /**
     * `a-z` was added to replicate /i flag
     */
    email: /^(?!\.)(?!.*\.\.)([a-zA-Z0-9_'+\-\.]*)[a-zA-Z0-9_+-]@([a-zA-Z0-9][a-zA-Z0-9\-]*\.)+[a-zA-Z]{2,}$/,
    /**
     * Constructed a valid Unicode RegExp
     *
     * Lazily instantiate since this type of regex isn't supported
     * in all envs (e.g. React Native).
     *
     * See:
     * https://github.com/colinhacks/zod/issues/2433
     * Fix in Zod:
     * https://github.com/colinhacks/zod/commit/9340fd51e48576a75adc919bff65dbc4a5d4c99b
     */
    emoji: () => {
      if (emojiRegex2 === void 0) {
        emojiRegex2 = RegExp("^(\\p{Extended_Pictographic}|\\p{Emoji_Component})+$", "u");
      }
      return emojiRegex2;
    },
    /**
     * Unused
     */
    uuid: /^[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}$/,
    /**
     * Unused
     */
    ipv4: /^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])$/,
    ipv4Cidr: /^(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\/(3[0-2]|[12]?[0-9])$/,
    /**
     * Unused
     */
    ipv6: /^(([a-f0-9]{1,4}:){7}|::([a-f0-9]{1,4}:){0,6}|([a-f0-9]{1,4}:){1}:([a-f0-9]{1,4}:){0,5}|([a-f0-9]{1,4}:){2}:([a-f0-9]{1,4}:){0,4}|([a-f0-9]{1,4}:){3}:([a-f0-9]{1,4}:){0,3}|([a-f0-9]{1,4}:){4}:([a-f0-9]{1,4}:){0,2}|([a-f0-9]{1,4}:){5}:([a-f0-9]{1,4}:){0,1})([a-f0-9]{1,4}|(((25[0-5])|(2[0-4][0-9])|(1[0-9]{2})|([0-9]{1,2}))\.){3}((25[0-5])|(2[0-4][0-9])|(1[0-9]{2})|([0-9]{1,2})))$/,
    ipv6Cidr: /^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))\/(12[0-8]|1[01][0-9]|[1-9]?[0-9])$/,
    base64: /^([0-9a-zA-Z+/]{4})*(([0-9a-zA-Z+/]{2}==)|([0-9a-zA-Z+/]{3}=))?$/,
    base64url: /^([0-9a-zA-Z-_]{4})*(([0-9a-zA-Z-_]{2}(==)?)|([0-9a-zA-Z-_]{3}(=)?))?$/,
    nanoid: /^[a-zA-Z0-9_-]{21}$/,
    jwt: /^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]*$/
  };
  function parseStringDef(def, refs) {
    const res = {
      type: "string"
    };
    if (def.checks) {
      for (const check of def.checks) {
        switch (check.kind) {
          case "min":
            setResponseValueAndErrors(res, "minLength", typeof res.minLength === "number" ? Math.max(res.minLength, check.value) : check.value, check.message, refs);
            break;
          case "max":
            setResponseValueAndErrors(res, "maxLength", typeof res.maxLength === "number" ? Math.min(res.maxLength, check.value) : check.value, check.message, refs);
            break;
          case "email":
            switch (refs.emailStrategy) {
              case "format:email":
                addFormat(res, "email", check.message, refs);
                break;
              case "format:idn-email":
                addFormat(res, "idn-email", check.message, refs);
                break;
              case "pattern:zod":
                addPattern(res, zodPatterns.email, check.message, refs);
                break;
            }
            break;
          case "url":
            addFormat(res, "uri", check.message, refs);
            break;
          case "uuid":
            addFormat(res, "uuid", check.message, refs);
            break;
          case "regex":
            addPattern(res, check.regex, check.message, refs);
            break;
          case "cuid":
            addPattern(res, zodPatterns.cuid, check.message, refs);
            break;
          case "cuid2":
            addPattern(res, zodPatterns.cuid2, check.message, refs);
            break;
          case "startsWith":
            addPattern(res, RegExp(`^${escapeLiteralCheckValue(check.value, refs)}`), check.message, refs);
            break;
          case "endsWith":
            addPattern(res, RegExp(`${escapeLiteralCheckValue(check.value, refs)}$`), check.message, refs);
            break;
          case "datetime":
            addFormat(res, "date-time", check.message, refs);
            break;
          case "date":
            addFormat(res, "date", check.message, refs);
            break;
          case "time":
            addFormat(res, "time", check.message, refs);
            break;
          case "duration":
            addFormat(res, "duration", check.message, refs);
            break;
          case "length":
            setResponseValueAndErrors(res, "minLength", typeof res.minLength === "number" ? Math.max(res.minLength, check.value) : check.value, check.message, refs);
            setResponseValueAndErrors(res, "maxLength", typeof res.maxLength === "number" ? Math.min(res.maxLength, check.value) : check.value, check.message, refs);
            break;
          case "includes": {
            addPattern(res, RegExp(escapeLiteralCheckValue(check.value, refs)), check.message, refs);
            break;
          }
          case "ip": {
            if (check.version !== "v6") {
              addFormat(res, "ipv4", check.message, refs);
            }
            if (check.version !== "v4") {
              addFormat(res, "ipv6", check.message, refs);
            }
            break;
          }
          case "base64url":
            addPattern(res, zodPatterns.base64url, check.message, refs);
            break;
          case "jwt":
            addPattern(res, zodPatterns.jwt, check.message, refs);
            break;
          case "cidr": {
            if (check.version !== "v6") {
              addPattern(res, zodPatterns.ipv4Cidr, check.message, refs);
            }
            if (check.version !== "v4") {
              addPattern(res, zodPatterns.ipv6Cidr, check.message, refs);
            }
            break;
          }
          case "emoji":
            addPattern(res, zodPatterns.emoji(), check.message, refs);
            break;
          case "ulid": {
            addPattern(res, zodPatterns.ulid, check.message, refs);
            break;
          }
          case "base64": {
            switch (refs.base64Strategy) {
              case "format:binary": {
                addFormat(res, "binary", check.message, refs);
                break;
              }
              case "contentEncoding:base64": {
                setResponseValueAndErrors(res, "contentEncoding", "base64", check.message, refs);
                break;
              }
              case "pattern:zod": {
                addPattern(res, zodPatterns.base64, check.message, refs);
                break;
              }
            }
            break;
          }
          case "nanoid": {
            addPattern(res, zodPatterns.nanoid, check.message, refs);
          }
          case "toLowerCase":
          case "toUpperCase":
          case "trim":
            break;
          default:
            /* @__PURE__ */ ((_) => {
            })(check);
        }
      }
    }
    return res;
  }
  function escapeLiteralCheckValue(literal, refs) {
    return refs.patternStrategy === "escape" ? escapeNonAlphaNumeric(literal) : literal;
  }
  var ALPHA_NUMERIC = new Set("ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz0123456789");
  function escapeNonAlphaNumeric(source) {
    let result = "";
    for (let i = 0; i < source.length; i++) {
      if (!ALPHA_NUMERIC.has(source[i])) {
        result += "\\";
      }
      result += source[i];
    }
    return result;
  }
  function addFormat(schema, value, message, refs) {
    var _a;
    if (schema.format || ((_a = schema.anyOf) == null ? void 0 : _a.some((x) => x.format))) {
      if (!schema.anyOf) {
        schema.anyOf = [];
      }
      if (schema.format) {
        schema.anyOf.push(__spreadValues({
          format: schema.format
        }, schema.errorMessage && refs.errorMessages && {
          errorMessage: { format: schema.errorMessage.format }
        }));
        delete schema.format;
        if (schema.errorMessage) {
          delete schema.errorMessage.format;
          if (Object.keys(schema.errorMessage).length === 0) {
            delete schema.errorMessage;
          }
        }
      }
      schema.anyOf.push(__spreadValues({
        format: value
      }, message && refs.errorMessages && { errorMessage: { format: message } }));
    } else {
      setResponseValueAndErrors(schema, "format", value, message, refs);
    }
  }
  function addPattern(schema, regex, message, refs) {
    var _a;
    if (schema.pattern || ((_a = schema.allOf) == null ? void 0 : _a.some((x) => x.pattern))) {
      if (!schema.allOf) {
        schema.allOf = [];
      }
      if (schema.pattern) {
        schema.allOf.push(__spreadValues({
          pattern: schema.pattern
        }, schema.errorMessage && refs.errorMessages && {
          errorMessage: { pattern: schema.errorMessage.pattern }
        }));
        delete schema.pattern;
        if (schema.errorMessage) {
          delete schema.errorMessage.pattern;
          if (Object.keys(schema.errorMessage).length === 0) {
            delete schema.errorMessage;
          }
        }
      }
      schema.allOf.push(__spreadValues({
        pattern: stringifyRegExpWithFlags(regex, refs)
      }, message && refs.errorMessages && { errorMessage: { pattern: message } }));
    } else {
      setResponseValueAndErrors(schema, "pattern", stringifyRegExpWithFlags(regex, refs), message, refs);
    }
  }
  function stringifyRegExpWithFlags(regex, refs) {
    var _a;
    if (!refs.applyRegexFlags || !regex.flags) {
      return regex.source;
    }
    const flags = {
      i: regex.flags.includes("i"),
      m: regex.flags.includes("m"),
      s: regex.flags.includes("s")
      // `.` matches newlines
    };
    const source = flags.i ? regex.source.toLowerCase() : regex.source;
    let pattern = "";
    let isEscaped = false;
    let inCharGroup = false;
    let inCharRange = false;
    for (let i = 0; i < source.length; i++) {
      if (isEscaped) {
        pattern += source[i];
        isEscaped = false;
        continue;
      }
      if (flags.i) {
        if (inCharGroup) {
          if (source[i].match(/[a-z]/)) {
            if (inCharRange) {
              pattern += source[i];
              pattern += `${source[i - 2]}-${source[i]}`.toUpperCase();
              inCharRange = false;
            } else if (source[i + 1] === "-" && ((_a = source[i + 2]) == null ? void 0 : _a.match(/[a-z]/))) {
              pattern += source[i];
              inCharRange = true;
            } else {
              pattern += `${source[i]}${source[i].toUpperCase()}`;
            }
            continue;
          }
        } else if (source[i].match(/[a-z]/)) {
          pattern += `[${source[i]}${source[i].toUpperCase()}]`;
          continue;
        }
      }
      if (flags.m) {
        if (source[i] === "^") {
          pattern += `(^|(?<=[\r
]))`;
          continue;
        } else if (source[i] === "$") {
          pattern += `($|(?=[\r
]))`;
          continue;
        }
      }
      if (flags.s && source[i] === ".") {
        pattern += inCharGroup ? `${source[i]}\r
` : `[${source[i]}\r
]`;
        continue;
      }
      pattern += source[i];
      if (source[i] === "\\") {
        isEscaped = true;
      } else if (inCharGroup && source[i] === "]") {
        inCharGroup = false;
      } else if (!inCharGroup && source[i] === "[") {
        inCharGroup = true;
      }
    }
    try {
      new RegExp(pattern);
    } catch (e) {
      console.warn(`Could not convert regex pattern at ${refs.currentPath.join("/")} to a flag-independent form! Falling back to the flag-ignorant source`);
      return regex.source;
    }
    return pattern;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/record.js
  function parseRecordDef(def, refs) {
    var _a, _b, _c, _d, _f, _g, _h;
    if (refs.target === "openAi") {
      console.warn("Warning: OpenAI may not support records in schemas! Try an array of key-value pairs instead.");
    }
    if (refs.target === "openApi3" && ((_a = def.keyType) == null ? void 0 : _a._def.typeName) === ZodFirstPartyTypeKind.ZodEnum) {
      return {
        type: "object",
        required: def.keyType._def.values,
        properties: def.keyType._def.values.reduce((acc, key) => {
          var _a2;
          return __spreadProps(__spreadValues({}, acc), {
            [key]: (_a2 = parseDef(def.valueType._def, __spreadProps(__spreadValues({}, refs), {
              currentPath: [...refs.currentPath, "properties", key]
            }))) != null ? _a2 : {}
          });
        }, {}),
        additionalProperties: refs.rejectedAdditionalProperties
      };
    }
    const schema = {
      type: "object",
      additionalProperties: (_b = parseDef(def.valueType._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath, "additionalProperties"]
      }))) != null ? _b : refs.allowedAdditionalProperties
    };
    if (refs.target === "openApi3") {
      return schema;
    }
    if (((_c = def.keyType) == null ? void 0 : _c._def.typeName) === ZodFirstPartyTypeKind.ZodString && ((_d = def.keyType._def.checks) == null ? void 0 : _d.length)) {
      const _e = parseStringDef(def.keyType._def, refs), { type } = _e, keyType = __objRest(_e, ["type"]);
      return __spreadProps(__spreadValues({}, schema), {
        propertyNames: keyType
      });
    } else if (((_f = def.keyType) == null ? void 0 : _f._def.typeName) === ZodFirstPartyTypeKind.ZodEnum) {
      return __spreadProps(__spreadValues({}, schema), {
        propertyNames: {
          enum: def.keyType._def.values
        }
      });
    } else if (((_g = def.keyType) == null ? void 0 : _g._def.typeName) === ZodFirstPartyTypeKind.ZodBranded && def.keyType._def.type._def.typeName === ZodFirstPartyTypeKind.ZodString && ((_h = def.keyType._def.type._def.checks) == null ? void 0 : _h.length)) {
      const _i = parseBrandedDef(def.keyType._def, refs), { type } = _i, keyType = __objRest(_i, ["type"]);
      return __spreadProps(__spreadValues({}, schema), {
        propertyNames: keyType
      });
    }
    return schema;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/map.js
  function parseMapDef(def, refs) {
    if (refs.mapStrategy === "record") {
      return parseRecordDef(def, refs);
    }
    const keys = parseDef(def.keyType._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "items", "items", "0"]
    })) || {};
    const values = parseDef(def.valueType._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "items", "items", "1"]
    })) || {};
    return {
      type: "array",
      maxItems: 125,
      items: {
        type: "array",
        items: [keys, values],
        minItems: 2,
        maxItems: 2
      }
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/nativeEnum.js
  function parseNativeEnumDef(def) {
    const object = def.values;
    const actualKeys = Object.keys(def.values).filter((key) => {
      return typeof object[object[key]] !== "number";
    });
    const actualValues = actualKeys.map((key) => object[key]);
    const parsedTypes = Array.from(new Set(actualValues.map((values) => typeof values)));
    return {
      type: parsedTypes.length === 1 ? parsedTypes[0] === "string" ? "string" : "number" : ["string", "number"],
      enum: actualValues
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/never.js
  function parseNeverDef() {
    return {
      not: {}
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/null.js
  function parseNullDef(refs) {
    return refs.target === "openApi3" ? {
      enum: ["null"],
      nullable: true
    } : {
      type: "null"
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/union.js
  var primitiveMappings = {
    ZodString: "string",
    ZodNumber: "number",
    ZodBigInt: "integer",
    ZodBoolean: "boolean",
    ZodNull: "null"
  };
  function parseUnionDef(def, refs) {
    if (refs.target === "openApi3")
      return asAnyOf(def, refs);
    const options = def.options instanceof Map ? Array.from(def.options.values()) : def.options;
    if (options.every((x) => x._def.typeName in primitiveMappings && (!x._def.checks || !x._def.checks.length))) {
      const types = options.reduce((types2, x) => {
        const type = primitiveMappings[x._def.typeName];
        return type && !types2.includes(type) ? [...types2, type] : types2;
      }, []);
      return {
        type: types.length > 1 ? types : types[0]
      };
    } else if (options.every((x) => x._def.typeName === "ZodLiteral" && !x.description)) {
      const types = options.reduce((acc, x) => {
        const type = typeof x._def.value;
        switch (type) {
          case "string":
          case "number":
          case "boolean":
            return [...acc, type];
          case "bigint":
            return [...acc, "integer"];
          case "object":
            if (x._def.value === null)
              return [...acc, "null"];
          case "symbol":
          case "undefined":
          case "function":
          default:
            return acc;
        }
      }, []);
      if (types.length === options.length) {
        const uniqueTypes = types.filter((x, i, a) => a.indexOf(x) === i);
        return {
          type: uniqueTypes.length > 1 ? uniqueTypes : uniqueTypes[0],
          enum: options.reduce((acc, x) => {
            return acc.includes(x._def.value) ? acc : [...acc, x._def.value];
          }, [])
        };
      }
    } else if (options.every((x) => x._def.typeName === "ZodEnum")) {
      return {
        type: "string",
        enum: options.reduce((acc, x) => [
          ...acc,
          ...x._def.values.filter((x2) => !acc.includes(x2))
        ], [])
      };
    }
    return asAnyOf(def, refs);
  }
  var asAnyOf = (def, refs) => {
    const anyOf = (def.options instanceof Map ? Array.from(def.options.values()) : def.options).map((x, i) => parseDef(x._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "anyOf", `${i}`]
    }))).filter((x) => !!x && (!refs.strictUnions || typeof x === "object" && Object.keys(x).length > 0));
    return anyOf.length ? { anyOf } : void 0;
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/nullable.js
  function parseNullableDef(def, refs) {
    if (["ZodString", "ZodNumber", "ZodBigInt", "ZodBoolean", "ZodNull"].includes(def.innerType._def.typeName) && (!def.innerType._def.checks || !def.innerType._def.checks.length)) {
      if (refs.target === "openApi3") {
        return {
          type: primitiveMappings[def.innerType._def.typeName],
          nullable: true
        };
      }
      return {
        type: [
          primitiveMappings[def.innerType._def.typeName],
          "null"
        ]
      };
    }
    if (refs.target === "openApi3") {
      const base2 = parseDef(def.innerType._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath]
      }));
      if (base2 && "$ref" in base2)
        return { allOf: [base2], nullable: true };
      return base2 && __spreadProps(__spreadValues({}, base2), { nullable: true });
    }
    const base = parseDef(def.innerType._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "anyOf", "0"]
    }));
    return base && { anyOf: [base, { type: "null" }] };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/number.js
  function parseNumberDef(def, refs) {
    const res = {
      type: "number"
    };
    if (!def.checks)
      return res;
    for (const check of def.checks) {
      switch (check.kind) {
        case "int":
          res.type = "integer";
          addErrorMessage(res, "type", check.message, refs);
          break;
        case "min":
          if (refs.target === "jsonSchema7") {
            if (check.inclusive) {
              setResponseValueAndErrors(res, "minimum", check.value, check.message, refs);
            } else {
              setResponseValueAndErrors(res, "exclusiveMinimum", check.value, check.message, refs);
            }
          } else {
            if (!check.inclusive) {
              res.exclusiveMinimum = true;
            }
            setResponseValueAndErrors(res, "minimum", check.value, check.message, refs);
          }
          break;
        case "max":
          if (refs.target === "jsonSchema7") {
            if (check.inclusive) {
              setResponseValueAndErrors(res, "maximum", check.value, check.message, refs);
            } else {
              setResponseValueAndErrors(res, "exclusiveMaximum", check.value, check.message, refs);
            }
          } else {
            if (!check.inclusive) {
              res.exclusiveMaximum = true;
            }
            setResponseValueAndErrors(res, "maximum", check.value, check.message, refs);
          }
          break;
        case "multipleOf":
          setResponseValueAndErrors(res, "multipleOf", check.value, check.message, refs);
          break;
      }
    }
    return res;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/object.js
  function parseObjectDef(def, refs) {
    const forceOptionalIntoNullable = refs.target === "openAi";
    const result = {
      type: "object",
      properties: {}
    };
    const required = [];
    const shape = def.shape();
    for (const propName in shape) {
      let propDef = shape[propName];
      if (propDef === void 0 || propDef._def === void 0) {
        continue;
      }
      let propOptional = safeIsOptional(propDef);
      if (propOptional && forceOptionalIntoNullable) {
        if (propDef instanceof ZodOptional) {
          propDef = propDef._def.innerType;
        }
        if (!propDef.isNullable()) {
          propDef = propDef.nullable();
        }
        propOptional = false;
      }
      const parsedDef = parseDef(propDef._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath, "properties", propName],
        propertyPath: [...refs.currentPath, "properties", propName]
      }));
      if (parsedDef === void 0) {
        continue;
      }
      result.properties[propName] = parsedDef;
      if (!propOptional) {
        required.push(propName);
      }
    }
    if (required.length) {
      result.required = required;
    }
    const additionalProperties = decideAdditionalProperties(def, refs);
    if (additionalProperties !== void 0) {
      result.additionalProperties = additionalProperties;
    }
    return result;
  }
  function decideAdditionalProperties(def, refs) {
    if (def.catchall._def.typeName !== "ZodNever") {
      return parseDef(def.catchall._def, __spreadProps(__spreadValues({}, refs), {
        currentPath: [...refs.currentPath, "additionalProperties"]
      }));
    }
    switch (def.unknownKeys) {
      case "passthrough":
        return refs.allowedAdditionalProperties;
      case "strict":
        return refs.rejectedAdditionalProperties;
      case "strip":
        return refs.removeAdditionalStrategy === "strict" ? refs.allowedAdditionalProperties : refs.rejectedAdditionalProperties;
    }
  }
  function safeIsOptional(schema) {
    try {
      return schema.isOptional();
    } catch (e) {
      return true;
    }
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/optional.js
  var parseOptionalDef = (def, refs) => {
    var _a;
    if (refs.currentPath.toString() === ((_a = refs.propertyPath) == null ? void 0 : _a.toString())) {
      return parseDef(def.innerType._def, refs);
    }
    const innerSchema = parseDef(def.innerType._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "anyOf", "1"]
    }));
    return innerSchema ? {
      anyOf: [
        {
          not: {}
        },
        innerSchema
      ]
    } : {};
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/pipeline.js
  var parsePipelineDef = (def, refs) => {
    if (refs.pipeStrategy === "input") {
      return parseDef(def.in._def, refs);
    } else if (refs.pipeStrategy === "output") {
      return parseDef(def.out._def, refs);
    }
    const a = parseDef(def.in._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "allOf", "0"]
    }));
    const b = parseDef(def.out._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "allOf", a ? "1" : "0"]
    }));
    return {
      allOf: [a, b].filter((x) => x !== void 0)
    };
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/promise.js
  function parsePromiseDef(def, refs) {
    return parseDef(def.type._def, refs);
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/set.js
  function parseSetDef(def, refs) {
    const items = parseDef(def.valueType._def, __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.currentPath, "items"]
    }));
    const schema = {
      type: "array",
      uniqueItems: true,
      items
    };
    if (def.minSize) {
      setResponseValueAndErrors(schema, "minItems", def.minSize.value, def.minSize.message, refs);
    }
    if (def.maxSize) {
      setResponseValueAndErrors(schema, "maxItems", def.maxSize.value, def.maxSize.message, refs);
    }
    return schema;
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/tuple.js
  function parseTupleDef(def, refs) {
    if (def.rest) {
      return {
        type: "array",
        minItems: def.items.length,
        items: def.items.map((x, i) => parseDef(x._def, __spreadProps(__spreadValues({}, refs), {
          currentPath: [...refs.currentPath, "items", `${i}`]
        }))).reduce((acc, x) => x === void 0 ? acc : [...acc, x], []),
        additionalItems: parseDef(def.rest._def, __spreadProps(__spreadValues({}, refs), {
          currentPath: [...refs.currentPath, "additionalItems"]
        }))
      };
    } else {
      return {
        type: "array",
        minItems: def.items.length,
        maxItems: def.items.length,
        items: def.items.map((x, i) => parseDef(x._def, __spreadProps(__spreadValues({}, refs), {
          currentPath: [...refs.currentPath, "items", `${i}`]
        }))).reduce((acc, x) => x === void 0 ? acc : [...acc, x], [])
      };
    }
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/undefined.js
  function parseUndefinedDef() {
    return {
      not: {}
    };
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/unknown.js
  function parseUnknownDef() {
    return {};
  }

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parsers/readonly.js
  var parseReadonlyDef = (def, refs) => {
    return parseDef(def.innerType._def, refs);
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/selectParser.js
  var selectParser = (def, typeName, refs) => {
    switch (typeName) {
      case ZodFirstPartyTypeKind.ZodString:
        return parseStringDef(def, refs);
      case ZodFirstPartyTypeKind.ZodNumber:
        return parseNumberDef(def, refs);
      case ZodFirstPartyTypeKind.ZodObject:
        return parseObjectDef(def, refs);
      case ZodFirstPartyTypeKind.ZodBigInt:
        return parseBigintDef(def, refs);
      case ZodFirstPartyTypeKind.ZodBoolean:
        return parseBooleanDef();
      case ZodFirstPartyTypeKind.ZodDate:
        return parseDateDef(def, refs);
      case ZodFirstPartyTypeKind.ZodUndefined:
        return parseUndefinedDef();
      case ZodFirstPartyTypeKind.ZodNull:
        return parseNullDef(refs);
      case ZodFirstPartyTypeKind.ZodArray:
        return parseArrayDef(def, refs);
      case ZodFirstPartyTypeKind.ZodUnion:
      case ZodFirstPartyTypeKind.ZodDiscriminatedUnion:
        return parseUnionDef(def, refs);
      case ZodFirstPartyTypeKind.ZodIntersection:
        return parseIntersectionDef(def, refs);
      case ZodFirstPartyTypeKind.ZodTuple:
        return parseTupleDef(def, refs);
      case ZodFirstPartyTypeKind.ZodRecord:
        return parseRecordDef(def, refs);
      case ZodFirstPartyTypeKind.ZodLiteral:
        return parseLiteralDef(def, refs);
      case ZodFirstPartyTypeKind.ZodEnum:
        return parseEnumDef(def);
      case ZodFirstPartyTypeKind.ZodNativeEnum:
        return parseNativeEnumDef(def);
      case ZodFirstPartyTypeKind.ZodNullable:
        return parseNullableDef(def, refs);
      case ZodFirstPartyTypeKind.ZodOptional:
        return parseOptionalDef(def, refs);
      case ZodFirstPartyTypeKind.ZodMap:
        return parseMapDef(def, refs);
      case ZodFirstPartyTypeKind.ZodSet:
        return parseSetDef(def, refs);
      case ZodFirstPartyTypeKind.ZodLazy:
        return () => def.getter()._def;
      case ZodFirstPartyTypeKind.ZodPromise:
        return parsePromiseDef(def, refs);
      case ZodFirstPartyTypeKind.ZodNaN:
      case ZodFirstPartyTypeKind.ZodNever:
        return parseNeverDef();
      case ZodFirstPartyTypeKind.ZodEffects:
        return parseEffectsDef(def, refs);
      case ZodFirstPartyTypeKind.ZodAny:
        return parseAnyDef();
      case ZodFirstPartyTypeKind.ZodUnknown:
        return parseUnknownDef();
      case ZodFirstPartyTypeKind.ZodDefault:
        return parseDefaultDef(def, refs);
      case ZodFirstPartyTypeKind.ZodBranded:
        return parseBrandedDef(def, refs);
      case ZodFirstPartyTypeKind.ZodReadonly:
        return parseReadonlyDef(def, refs);
      case ZodFirstPartyTypeKind.ZodCatch:
        return parseCatchDef(def, refs);
      case ZodFirstPartyTypeKind.ZodPipeline:
        return parsePipelineDef(def, refs);
      case ZodFirstPartyTypeKind.ZodFunction:
      case ZodFirstPartyTypeKind.ZodVoid:
      case ZodFirstPartyTypeKind.ZodSymbol:
        return void 0;
      default:
        return /* @__PURE__ */ ((_) => void 0)(typeName);
    }
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/parseDef.js
  function parseDef(def, refs, forceResolution = false) {
    var _a;
    const seenItem = refs.seen.get(def);
    if (refs.override) {
      const overrideResult = (_a = refs.override) == null ? void 0 : _a.call(refs, def, refs, seenItem, forceResolution);
      if (overrideResult !== ignoreOverride) {
        return overrideResult;
      }
    }
    if (seenItem && !forceResolution) {
      const seenSchema = get$ref(seenItem, refs);
      if (seenSchema !== void 0) {
        return seenSchema;
      }
    }
    const newItem = { def, path: refs.currentPath, jsonSchema: void 0 };
    refs.seen.set(def, newItem);
    const jsonSchemaOrGetter = selectParser(def, def.typeName, refs);
    const jsonSchema = typeof jsonSchemaOrGetter === "function" ? parseDef(jsonSchemaOrGetter(), refs) : jsonSchemaOrGetter;
    if (jsonSchema) {
      addMeta(def, refs, jsonSchema);
    }
    if (refs.postProcess) {
      const postProcessResult = refs.postProcess(jsonSchema, def, refs);
      newItem.jsonSchema = jsonSchema;
      return postProcessResult;
    }
    newItem.jsonSchema = jsonSchema;
    return jsonSchema;
  }
  var get$ref = (item, refs) => {
    switch (refs.$refStrategy) {
      case "root":
        return { $ref: item.path.join("/") };
      case "relative":
        return { $ref: getRelativePath(refs.currentPath, item.path) };
      case "none":
      case "seen": {
        if (item.path.length < refs.currentPath.length && item.path.every((value, index) => refs.currentPath[index] === value)) {
          console.warn(`Recursive reference detected at ${refs.currentPath.join("/")}! Defaulting to any`);
          return {};
        }
        return refs.$refStrategy === "seen" ? {} : void 0;
      }
    }
  };
  var getRelativePath = (pathA, pathB) => {
    let i = 0;
    for (; i < pathA.length && i < pathB.length; i++) {
      if (pathA[i] !== pathB[i])
        break;
    }
    return [(pathA.length - i).toString(), ...pathB.slice(i)].join("/");
  };
  var addMeta = (def, refs, jsonSchema) => {
    if (def.description) {
      jsonSchema.description = def.description;
      if (refs.markdownDescription) {
        jsonSchema.markdownDescription = def.description;
      }
    }
    return jsonSchema;
  };

  // node_modules/.pnpm/zod-to-json-schema@3.24.5_zod@3.24.2/node_modules/zod-to-json-schema/dist/esm/zodToJsonSchema.js
  var zodToJsonSchema = (schema, options) => {
    var _a;
    const refs = getRefs(options);
    const definitions = typeof options === "object" && options.definitions ? Object.entries(options.definitions).reduce((acc, [name3, schema2]) => {
      var _a2;
      return __spreadProps(__spreadValues({}, acc), {
        [name3]: (_a2 = parseDef(schema2._def, __spreadProps(__spreadValues({}, refs), {
          currentPath: [...refs.basePath, refs.definitionPath, name3]
        }), true)) != null ? _a2 : {}
      });
    }, {}) : void 0;
    const name2 = typeof options === "string" ? options : (options == null ? void 0 : options.nameStrategy) === "title" ? void 0 : options == null ? void 0 : options.name;
    const main = (_a = parseDef(schema._def, name2 === void 0 ? refs : __spreadProps(__spreadValues({}, refs), {
      currentPath: [...refs.basePath, refs.definitionPath, name2]
    }), false)) != null ? _a : {};
    const title = typeof options === "object" && options.name !== void 0 && options.nameStrategy === "title" ? options.name : void 0;
    if (title !== void 0) {
      main.title = title;
    }
    const combined = name2 === void 0 ? definitions ? __spreadProps(__spreadValues({}, main), {
      [refs.definitionPath]: definitions
    }) : main : {
      $ref: [
        ...refs.$refStrategy === "relative" ? [] : refs.basePath,
        refs.definitionPath,
        name2
      ].join("/"),
      [refs.definitionPath]: __spreadProps(__spreadValues({}, definitions), {
        [name2]: main
      })
    };
    if (refs.target === "jsonSchema7") {
      combined.$schema = "http://json-schema.org/draft-07/schema#";
    } else if (refs.target === "jsonSchema2019-09" || refs.target === "openAi") {
      combined.$schema = "https://json-schema.org/draft/2019-09/schema#";
    }
    if (refs.target === "openAi" && ("anyOf" in combined || "oneOf" in combined || "allOf" in combined || "type" in combined && Array.isArray(combined.type))) {
      console.warn("Warning: OpenAI may not support schemas with unions as roots! Try wrapping it in an object property.");
    }
    return combined;
  };

  // node_modules/.pnpm/zod-validation-error@3.4.0_zod@3.24.2/node_modules/zod-validation-error/dist/index.mjs
  function isZodErrorLike(err) {
    return err instanceof Error && err.name === "ZodError" && "issues" in err && Array.isArray(err.issues);
  }
  var ValidationError = class extends Error {
    constructor(message, options) {
      super(message, options);
      __publicField(this, "name");
      __publicField(this, "details");
      this.name = "ZodValidationError";
      this.details = getIssuesFromErrorOptions(options);
    }
    toString() {
      return this.message;
    }
  };
  function getIssuesFromErrorOptions(options) {
    if (options) {
      const cause = options.cause;
      if (isZodErrorLike(cause)) {
        return cause.issues;
      }
    }
    return [];
  }
  function isNonEmptyArray(value) {
    return value.length !== 0;
  }
  var identifierRegex = /[$_\p{ID_Start}][$\u200c\u200d\p{ID_Continue}]*/u;
  function joinPath(path) {
    if (path.length === 1) {
      return path[0].toString();
    }
    return path.reduce((acc, item) => {
      if (typeof item === "number") {
        return acc + "[" + item.toString() + "]";
      }
      if (item.includes('"')) {
        return acc + '["' + escapeQuotes(item) + '"]';
      }
      if (!identifierRegex.test(item)) {
        return acc + '["' + item + '"]';
      }
      const separator = acc.length === 0 ? "" : ".";
      return acc + separator + item;
    }, "");
  }
  function escapeQuotes(str) {
    return str.replace(/"/g, '\\"');
  }
  var ISSUE_SEPARATOR = "; ";
  var MAX_ISSUES_IN_MESSAGE = 99;
  var PREFIX = "Validation error";
  var PREFIX_SEPARATOR = ": ";
  var UNION_SEPARATOR = ", or ";
  function createMessageBuilder(props = {}) {
    const {
      issueSeparator = ISSUE_SEPARATOR,
      unionSeparator = UNION_SEPARATOR,
      prefixSeparator = PREFIX_SEPARATOR,
      prefix = PREFIX,
      includePath = true,
      maxIssuesInMessage = MAX_ISSUES_IN_MESSAGE
    } = props;
    return (issues) => {
      const message = issues.slice(0, maxIssuesInMessage).map(
        (issue) => getMessageFromZodIssue({
          issue,
          issueSeparator,
          unionSeparator,
          includePath
        })
      ).join(issueSeparator);
      return prefixMessage(message, prefix, prefixSeparator);
    };
  }
  function getMessageFromZodIssue(props) {
    const { issue, issueSeparator, unionSeparator, includePath } = props;
    if (issue.code === ZodIssueCode.invalid_union) {
      return issue.unionErrors.reduce((acc, zodError) => {
        const newIssues = zodError.issues.map(
          (issue2) => getMessageFromZodIssue({
            issue: issue2,
            issueSeparator,
            unionSeparator,
            includePath
          })
        ).join(issueSeparator);
        if (!acc.includes(newIssues)) {
          acc.push(newIssues);
        }
        return acc;
      }, []).join(unionSeparator);
    }
    if (issue.code === ZodIssueCode.invalid_arguments) {
      return [
        issue.message,
        ...issue.argumentsError.issues.map(
          (issue2) => getMessageFromZodIssue({
            issue: issue2,
            issueSeparator,
            unionSeparator,
            includePath
          })
        )
      ].join(issueSeparator);
    }
    if (issue.code === ZodIssueCode.invalid_return_type) {
      return [
        issue.message,
        ...issue.returnTypeError.issues.map(
          (issue2) => getMessageFromZodIssue({
            issue: issue2,
            issueSeparator,
            unionSeparator,
            includePath
          })
        )
      ].join(issueSeparator);
    }
    if (includePath && isNonEmptyArray(issue.path)) {
      if (issue.path.length === 1) {
        const identifier = issue.path[0];
        if (typeof identifier === "number") {
          return `${issue.message} at index ${identifier}`;
        }
      }
      return `${issue.message} at "${joinPath(issue.path)}"`;
    }
    return issue.message;
  }
  function prefixMessage(message, prefix, prefixSeparator) {
    if (prefix !== null) {
      if (message.length > 0) {
        return [prefix, message].join(prefixSeparator);
      }
      return prefix;
    }
    if (message.length > 0) {
      return message;
    }
    return PREFIX;
  }
  function fromZodError(zodError, options = {}) {
    if (!isZodErrorLike(zodError)) {
      throw new TypeError(
        `Invalid zodError param; expected instance of ZodError. Did you mean to use the "${fromError.name}" method instead?`
      );
    }
    return fromZodErrorWithoutRuntimeCheck(zodError, options);
  }
  function fromZodErrorWithoutRuntimeCheck(zodError, options = {}) {
    const zodIssues = zodError.errors;
    let message;
    if (isNonEmptyArray(zodIssues)) {
      const messageBuilder = createMessageBuilderFromOptions2(options);
      message = messageBuilder(zodIssues);
    } else {
      message = zodError.message;
    }
    return new ValidationError(message, { cause: zodError });
  }
  function createMessageBuilderFromOptions2(options) {
    if ("messageBuilder" in options) {
      return options.messageBuilder;
    }
    return createMessageBuilder(options);
  }
  var toValidationError = (options = {}) => (err) => {
    if (isZodErrorLike(err)) {
      return fromZodErrorWithoutRuntimeCheck(err, options);
    }
    if (err instanceof Error) {
      return new ValidationError(err.message, { cause: err });
    }
    return new ValidationError("Unknown error");
  };
  function fromError(err, options = {}) {
    return toValidationError(options)(err);
  }

  // src/api/schemas.ts
  var LhqModelLineEndingsSchema = z.union([z.literal("LF"), z.literal("CRLF")]);
  var LhqModelOptionsResourcesSchema = z.union([
    z.literal("All"),
    z.literal("Categories")
  ]);
  var LhqModelResourceParameterSchema = z.object({
    description: z.string().optional(),
    order: z.number()
  });
  var LhqModelResourceTranslationStateSchema = z.union([
    z.literal("New"),
    z.literal("Edited"),
    z.literal("NeedsReview"),
    z.literal("Final")
  ]);
  var LhqModelResourceValueSchema = z.object({
    value: z.string().optional(),
    locked: z.boolean().optional(),
    auto: z.boolean().optional()
  });
  var LhqModelResourceSchemaBase = z.object({
    state: LhqModelResourceTranslationStateSchema,
    description: z.string().optional(),
    parameters: z.record(LhqModelResourceParameterSchema).optional(),
    values: z.record(LhqModelResourceValueSchema).optional()
  });
  var LhqModelResourceSchema = LhqModelResourceSchemaBase;
  var baseDataNodeSchema = z.object({
    name: z.string(),
    attrs: z.record(z.string().nullable().optional()).optional()
  });
  var LhqModelDataNodeSchema = baseDataNodeSchema.extend({
    childs: z.lazy(() => z.array(LhqModelDataNodeSchema)).optional()
  });
  var baseCategorySchema = z.object({
    description: z.string().optional(),
    resources: z.lazy(() => LhqModelResourcesCollectionSchema).optional()
  });
  var LhqModelCategorySchema = baseCategorySchema.extend({
    categories: z.lazy(() => LhqModelCategoriesCollectionSchema).optional()
  });
  var LhqModelUidSchema = z.literal("6ce4d54c5dbd415c93019d315e278638");
  var LhqModelVersionSchema = z.union([z.literal(1), z.literal(2)]);
  var LhqCodeGenVersionSchema = z.literal(1);
  var LhqModelCategoriesCollectionSchema = z.record(LhqModelCategorySchema);
  var LhqModelResourcesCollectionSchema = z.record(LhqModelResourceSchema);
  var LhqModelOptionsSchema = z.object({
    categories: z.boolean(),
    resources: LhqModelOptionsResourcesSchema
  });
  var LhqModelMetadataSchema = z.object({
    childs: z.array(LhqModelDataNodeSchema).optional()
  });
  var LhqModelSchema = z.object({
    model: z.object({
      uid: LhqModelUidSchema,
      version: LhqModelVersionSchema,
      options: LhqModelOptionsSchema,
      name: z.string(),
      description: z.string().optional(),
      primaryLanguage: z.string()
    }),
    languages: z.array(z.string()),
    metadatas: LhqModelMetadataSchema.optional(),
    resources: z.lazy(() => LhqModelResourcesCollectionSchema).optional(),
    categories: z.lazy(() => LhqModelCategoriesCollectionSchema).optional()
  });

  // src/generatorUtils.ts
  var DOMParser;
  var regexLF = new RegExp("\\r\\n|\\r", "g");
  var regexCRLF = new RegExp("(\\r(?!\\n))|((?<!\\r)\\n)", "g");
  function validateLhqModel(data) {
    if (typeof data === "string") {
      const parseResult2 = tryJsonParse(data, true);
      if (!parseResult2.success) {
        return { success: false, error: parseResult2.error };
      }
      data = parseResult2.data;
    }
    if (data === void 0 || data === null || typeof data !== "object") {
      return { success: false, error: 'Specified "data" must be an object!' };
    }
    const parseResult = LhqModelSchema.safeParse(data);
    const success = parseResult.success && !isNullOrEmpty(parseResult.data);
    let error = void 0;
    if (!parseResult.success) {
      const messageBuilder = createMessageBuilder({
        prefix: "",
        prefixSeparator: "",
        issueSeparator: "\n"
      });
      const err = fromZodError(parseResult.error, { messageBuilder });
      error = err.toString();
    }
    return { success, error, model: success ? parseResult.data : void 0 };
  }
  function getGeneratedFileContent(generatedFile, applyLineEndings) {
    if (!applyLineEndings || generatedFile.content.length === 0) {
      return generatedFile.content;
    }
    return generatedFile.lineEndings === "LF" ? generatedFile.content.replace(regexLF, "\n") : generatedFile.content.replace(regexCRLF, "\r\n");
  }
  function generateLhqSchema() {
    const jsonSchema = zodToJsonSchema(LhqModelSchema, {
      name: "LhqModel",
      $refStrategy: "root"
    });
    return JSON.stringify(jsonSchema, null, 2);
  }
  var itemGroupTypes = ["None", "Compile", "Content", "EmbeddedResource"];
  var itemGroupTypesAttrs = ["Include", "Update"];
  var csProjectXPath = '//ns:ItemGroup/ns:##TYPE##[@##ATTR##="##FILE##"]';
  var xpathRootNamespace = "string(//ns:RootNamespace)";
  var xpathAssemblyName = "string(//ns:AssemblyName)";
  function getRootNamespaceFromCsProj(lhqModelFileName, t4FileName, csProjectFileName, csProjectFileContent) {
    var _a, _b, _c;
    let referencedLhqFile = false;
    let referencedT4File = false;
    if (isNullOrEmpty(csProjectFileName) || isNullOrEmpty(csProjectFileContent)) {
      return void 0;
    }
    let rootNamespace;
    try {
      const fileContent = tryRemoveBOM(csProjectFileContent);
      if (typeof window !== "undefined" && typeof window.DOMParser !== "undefined") {
        DOMParser = window.DOMParser;
      } else {
        DOMParser = import_xmldom.DOMParser;
      }
      const doc = new DOMParser().parseFromString(fileContent, "text/xml");
      const rootNode = doc;
      const rootNs = ((_a = doc.documentElement) == null ? void 0 : _a.namespaceURI) || "";
      const ns = isNullOrEmpty(rootNs) ? null : rootNs;
      const xpathSelect = xpath.useNamespaces({ ns: rootNs });
      const findFileElement = function(fileName) {
        for (const itemGroupType of itemGroupTypes) {
          for (const attr of itemGroupTypesAttrs) {
            const xpathQuery = csProjectXPath.replace("##TYPE##", itemGroupType).replace("##ATTR##", attr).replace("##FILE##", fileName);
            const element = xpathSelect(xpathQuery, rootNode, true);
            if (element) {
              return element;
            }
          }
        }
        return void 0;
      };
      rootNamespace = xpathSelect(xpathRootNamespace, rootNode, true);
      referencedLhqFile = findFileElement(lhqModelFileName) != void 0;
      const t4FileElement = findFileElement(t4FileName);
      if (t4FileElement) {
        referencedT4File = true;
        const dependentUpon = (_b = t4FileElement.getElementsByTagNameNS(ns, "DependentUpon")[0]) == null ? void 0 : _b.textContent;
        if (dependentUpon && dependentUpon === lhqModelFileName) {
          referencedLhqFile = true;
        }
        const customToolNamespace = (_c = t4FileElement.getElementsByTagNameNS(ns, "CustomToolNamespace")[0]) == null ? void 0 : _c.textContent;
        if (customToolNamespace) {
          rootNamespace = customToolNamespace;
        }
      }
      if (!rootNamespace) {
        rootNamespace = xpathSelect(xpathAssemblyName, rootNode, true);
      }
    } catch (e) {
      console.error("Error getting root namespace.", e);
      rootNamespace = void 0;
    }
    return { csProjectFileName, t4FileName, namespace: rootNamespace, referencedLhqFile, referencedT4File };
  }

  // src/generator.ts
  var GeneratorHostDataKeys = Object.freeze({
    namespace: "namespace",
    fileHeader: "fileHeader"
  });
  function getLibraryVersion() {
    if (false) {
      return "0.0.0";
    }
    return "1.0.89";
  }
  var _Generator = class _Generator {
    constructor() {
      this._generatedFiles = [];
    }
    /**
     * Initializes the generator with the given initialization information.
     * 
     * @param initialization - The initialization information for the generator.
     * @throws Error if the initialization information is invalid or missing.
     */
    static initialize(initialization) {
      if (!_Generator._initialized) {
        if (isNullOrEmpty(initialization)) {
          throw new Error("Generator initialization is required !");
        }
        if (isNullOrEmpty(initialization.hbsTemplates)) {
          throw new Error("Handlebars templates are required (initialization.hbsTemplates) !");
        }
        if (Object.keys(initialization.hbsTemplates).length === 0) {
          throw new Error("Handlebars templates cannot be empty are required (initialization.hbsTemplates) !");
        }
        if (isNullOrEmpty(initialization.hostEnvironment)) {
          throw new Error("Host environment is required (initialization.hostEnvironment) !");
        }
        HbsTemplateManager.init(initialization.hbsTemplates);
        _Generator.hostEnv = initialization.hostEnvironment;
        registerHelpers(initialization.hostEnvironment);
        _Generator._initialized = true;
      }
    }
    // /**
    //  * Returns the content of the generated file with the appropriate line endings.
    //  * 
    //  * @param generatedFile - The generated file.
    //  * @param applyLineEndings - A flag indicating whether to apply line endings to the content. Line endings are determined by the `lineEndings` property of the `GeneratedFile`.
    //  * @returns The content of the generated file with the appropriate line endings.
    //  */
    // public getFileContent(generatedFile: GeneratedFile, applyLineEndings: boolean): string {
    //     if (!applyLineEndings || generatedFile.content.length === 0) {
    //         return generatedFile.content;
    //     }
    //     return generatedFile.lineEndings === 'LF'
    //         ? generatedFile.content.replace(Generator.regexLF, '\n')
    //         : generatedFile.content.replace(Generator.regexCRLF, '\r\n');
    // }
    /**
     * Generates code files based on the provided `LHQ` model and external host data.
     * 
     * This method validates the input `LHQ` model, processes the specified Handlebars template,
     * and generates the corresponding output files. It also handles inline and child template outputs.
     * 
     * @param fileName - The name of the input LHQ model file (*.lhq).
     * @param modelData - The LHQ model data, either as a deserialized JSON object or a JSON as string.
     * @param data - Optional external host data as a key-value mapping (object or JSON as string) used by the templates.
     * @returns A `GenerateResult` object containing the list of generated files.
     * 
     * @throws `AppError` if the generator is not initialized, or if any required input is missing or invalid.
     * 
     * @example
     * const generator = new Generator();
     * Generator.initialize(\{ hbsTemplates: templates, hostEnvironment: hostEnv \});
     * const file = 'model.lhq';
     * const model = fs.readFileSync(file, 'utf8');
     * const data = \{ namespace: 'MyNamespace' \};
     * const result = generator.generate(file, model, data);
     * console.log(result.generatedFiles);
     */
    generate(fileName, modelData, data) {
      var _a, _b, _c;
      if (!_Generator._initialized) {
        throw new AppError("Generator not initialized !");
      }
      if (isNullOrEmpty(fileName)) {
        throw new AppError("Missing input model file name !");
      }
      if (isNullOrEmpty(modelData)) {
        throw new AppError("Missing input modelData !");
      }
      let hostData = {};
      if (typeof data === "string") {
        hostData = jsonParseOrDefault(data, {}, true);
      } else if (typeof data === "object") {
        hostData = data != null ? data : {};
      } else {
        throw new AppError("Invalid host data type (object or string expected) !");
      }
      hostData != null ? hostData : hostData = {};
      const validation = validateLhqModel(modelData);
      if (!validation.success) {
        throw new AppError((_a = validation.error) != null ? _a : `Unable to deserialize or validate LHQ model '${fileName}' !`);
      }
      const model = validation.model;
      const rootModel = new RootModelElement(model);
      const templateId = (_c = (_b = rootModel.codeGenerator) == null ? void 0 : _b.templateId) != null ? _c : "";
      if (isNullOrEmpty(rootModel.codeGenerator) || isNullOrEmpty(templateId)) {
        throw new AppError(`LHQ model '${fileName}' missing code generator template information !`);
      }
      const saveInlineOutputs = (templId, inlineOutputs) => {
        if (inlineOutputs) {
          inlineOutputs.forEach((inline) => {
            this.addResultFile(templId, inline.content, inline);
          });
        }
      };
      const templateModel = new TemplateRootModel(rootModel, {}, hostData);
      templateModel.setCurrentTemplateId(templateId);
      try {
        const templateResult = HbsTemplateManager.runTemplate(templateId, templateModel);
        const mainOutput = templateModel.output;
        this.addResultFile(templateId, templateResult, mainOutput);
        saveInlineOutputs(templateId, templateModel.inlineOutputs);
      } finally {
        templateModel.setCurrentTemplateId(void 0);
      }
      templateModel.childOutputs.forEach((child) => {
        templateModel.setAsChildTemplate(child);
        templateModel.setCurrentTemplateId(child.templateId);
        try {
          const templateResult = HbsTemplateManager.runTemplate(child.templateId, templateModel);
          const output = templateModel.output;
          if (isNullOrEmpty(output)) {
            throw new AppError(`Template '${child.templateId}' missing main output file information (missing 'm-output' helper) !`);
          }
          this.addResultFile(child.templateId, templateResult, output);
          saveInlineOutputs(child.templateId, templateModel.inlineOutputs);
        } finally {
          templateModel.setCurrentTemplateId(void 0);
        }
      });
      return { generatedFiles: this._generatedFiles };
    }
    addResultFile(templateId, templateResult, output) {
      if (isNullOrEmpty(output)) {
        throw new AppError(`Template '${templateId}' missing main output file information (missing 'm-output' helper) !`);
      }
      if (isNullOrEmpty(output.fileName)) {
        throw new AppError(`Template '${templateId}' missing main output file name (missing property 'fileName' in 'm-output' helper) !`);
      }
      if (isNullOrEmpty(output.settings)) {
        throw new AppError(`Template '${templateId}' missing main output settings (in 'm-output' helper) !`);
      }
      this.addResultFileInternal(templateResult, output.fileName, output.settings);
    }
    addResultFileInternal(templateResult, fileName, settings) {
      var _a;
      if (settings.Enabled) {
        const genFileName = isNullOrEmpty(settings.OutputFolder) ? fileName : _Generator.hostEnv.pathCombine(settings.OutputFolder, fileName);
        const bom = settings.EncodingWithBOM;
        const lineEndings = (_a = settings.LineEndings) != null ? _a : DefaultCodeGenSettings.LineEndings;
        const result = { fileName: genFileName, content: templateResult, bom, lineEndings };
        this._generatedFiles.push(result);
      }
    }
  };
  _Generator._initialized = false;
  var Generator = _Generator;
  return __toCommonJS(index_exports);
})();
