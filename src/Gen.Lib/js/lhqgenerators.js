var LhqGenerators;
/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/AppError.ts":
/*!*************************!*\
  !*** ./src/AppError.ts ***!
  \*************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   AppError: () => (/* binding */ AppError)
/* harmony export */ });
class AppError extends Error {
    constructor(message, stack) {
        super(message);
        this.message = message;
        this.name = 'AppError';
        // Maintains proper stack trace for where our error was thrown (only available on V8)
        if (Error.captureStackTrace) {
            Error.captureStackTrace(this, AppError);
        }
        if (stack !== undefined && stack !== null) {
            this.stack = stack;
        }
    }
}


/***/ }),

/***/ "./src/helpers.ts":
/*!************************!*\
  !*** ./src/helpers.ts ***!
  \************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   clearHelpersContext: () => (/* binding */ clearHelpersContext),
/* harmony export */   debugHelpersTimeTaken: () => (/* binding */ debugHelpersTimeTaken),
/* harmony export */   getKnownHelpers: () => (/* binding */ getKnownHelpers),
/* harmony export */   registerHelpers: () => (/* binding */ registerHelpers)
/* harmony export */ });
/* harmony import */ var _utils__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./utils */ "./src/utils.ts");
/* harmony import */ var _hostEnv__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./hostEnv */ "./src/hostEnv.ts");


function registerHelpers() {
    Object.keys(helpersList).forEach(key => {
        // @ts-ignore
        Handlebars.registerHelper(key, helpersList[key]);
        //const fn = helpersList[key];
        //Handlebars.registerHelper(key, () => debugLogAndExec(key, fn, ...arguments));
    });
    clearHelpersContext();
}
const helpersList = {
    'x-header': headerHelper,
    'x-value': objValueHelper,
    'x-indent': indentHelper,
    'x-join': joinHelper,
    'x-concat': concatHelper,
    'x-replace': replaceHelper,
    'x-trimEnd': trimEndHelper,
    'x-equals': equalsHelper,
    'x-isTrue': isTrueHelper,
    'x-isFalse': isFalseHelper,
    'x-resourceComment': resourceCommentHelper,
    'x-resourceValue': resourceValueHelper,
    'x-resourceHasLang': resourceHasLangHelper,
    'x-resourceParamNames': resourceParamNamesHelper,
    'x-merge': mergeHelper,
    'x-sortBy': sortByHelper,
    'x-sortObject': sortObjectByKeyHelper,
    'x-objCount': objCountHelper,
    'x-hasItems': hasItemsHelper,
    'x-textEncode': textEncodeHelper,
    'x-host-webHtmlEncode': hostWebHtmlEncodeHelper,
    'x-render': renderHelper,
    'x-isNullOrEmpty': isNullOrEmptyHelper,
    'x-isNotNullOrEmpty': isNotNullOrEmptyHelper,
    'x-fn': callFunctionHelper,
    'x-logical': logicalHelper,
    'x-debugLog': debugLogHelper,
    'x-var': returnVarFromTempHelper
};
function getKnownHelpers() {
    return Object.fromEntries(Object.keys(helpersList).map(key => [key, true]));
}
let dbgCounter = 0;
let globalVarTemp = {};
let helpersTimeTaken = {};
const trackHelperTimes = false;
function clearHelpersContext() {
    dbgCounter = 0;
    globalVarTemp = {};
    //helpersTimeTaken = {};
}
function debugLogAndExec(helperName, fn, ...args) {
    var _a, _b, _c, _d, _e, _f;
    let debug = false;
    let header = '';
    let cnt = 0;
    if (arguments.length > 0) {
        const ctx = arguments[arguments.length - 1];
        debug = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.valueAsBool)((_b = (_a = ctx.hash) === null || _a === void 0 ? void 0 : _a._debug) !== null && _b !== void 0 ? _b : false);
        if (debug) {
            cnt = ++dbgCounter;
            const debugLog = ((_d = (_c = ctx.hash) === null || _c === void 0 ? void 0 : _c._debugLog) !== null && _d !== void 0 ? _d : '').toString();
            const hash = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.copyObject)((_e = ctx.hash) !== null && _e !== void 0 ? _e : {}, ['_debug', '_debugLog']);
            const restArgs = Array.from(args).slice(0, -1);
            header = `[${cnt}#${ctx.name}](${debugLog}) hash: ${JSON.stringify(hash, null, 0)}`;
            _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog(`${header} ${JSON.stringify(restArgs, null, 0)}`);
        }
    }
    //HostEnv.debugLog(`[debugLogAndExec] fn: ${typeof fn} , arguments: ${JSON.stringify(arguments, null, 0)}, args: ${JSON.stringify(args, null, 0)}`);
    if (debug) {
        //HostEnv.debugLog(`${header}, arguments: ${JSON.stringify(arguments, null, 0)}, args: ${JSON.stringify(args, null, 0)}`);
    }
    let start = 0;
    if (trackHelperTimes) {
        start = Date.now();
    }
    // @ts-ignore
    const res = fn.call(this, ...args);
    if (trackHelperTimes) {
        const duration = Date.now() - start;
        helpersTimeTaken[helperName] = ((_f = helpersTimeTaken[helperName]) !== null && _f !== void 0 ? _f : 0) + duration;
    }
    if (debug) {
        _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog(`${header}, result: ${JSON.stringify(res, null, 0)}`);
    }
    return res;
}
function debugHelpersTimeTaken() {
    if (!trackHelperTimes) {
        return;
    }
    let totalDuration = 0;
    Object.keys(helpersTimeTaken).forEach(key => {
        var _a;
        const duration = (_a = helpersTimeTaken[key]) !== null && _a !== void 0 ? _a : 0;
        totalDuration += duration;
        _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog(`helper '${key}' taken total: ${formatDuration(duration)}`);
    });
    _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog(`All helpers taken total: ${formatDuration(totalDuration)}`);
}
function formatDuration(ms) {
    const seconds = Math.floor(ms / 1000);
    const milliseconds = ms % 1000;
    return seconds > 0
        ? `${seconds} second${seconds > 1 ? 's' : ''} and ${milliseconds} ms`
        : `${milliseconds} ms`;
}
function headerHelper() {
    return `//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool - Localization HQ Editor.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------`;
}
function objValueHelper(context, path) {
    const value = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.getNestedPropertyValue)(context, path);
    return value !== undefined ? value : '';
}
function indentHelper(count, options) {
    count = count < 0 ? 0 : count;
    // @ts-ignore
    var content = options.fn(this);
    var paddedContent = content.split('\n')
        .map(line => '\t'.repeat(count) + line)
        .join('\n');
    return paddedContent;
}
/*
{{#join people delimiter=" and " start="0" end="2"}}{{name}} ({{gender}}, {{age}}){{/join}}
<h1>Jobs</h1>
{{join jobs delimiter=", " start="1" end="2"}}
*/
function joinHelper(items, options) {
    // if (dbgCounter === 0) {
    //     dbgCounter = 1;
    //     // @ts-ignore
    //     HostEnv.debugLog(`[joinHelper] items: ${typeof items}, ${items.name} options: ${JSON.stringify(options)}`);
    // }
    var _a, _b, _c, _d;
    var delimiter = ((_a = options.hash) === null || _a === void 0 ? void 0 : _a.delimiter) || ",", start = ((_b = options.hash) === null || _b === void 0 ? void 0 : _b.start) || 0, len = items ? items.length : 0, end = ((_c = options.hash) === null || _c === void 0 ? void 0 : _c.end) || len, out = "", decorator = ((_d = options.hash) === null || _d === void 0 ? void 0 : _d.decorator) || `"`;
    if (end > len)
        end = len;
    if ('function' === typeof options) {
        for (let i = start; i < end; i++) {
            if (i > start)
                out += delimiter;
            if ('string' === typeof items[i])
                out += items[i];
            else
                out += options(items[i]);
        }
        return out;
    }
    else {
        // @ts-ignore
        var res = [].concat(items).map(x => `${decorator}${x}${decorator}`).slice(start, end).join(delimiter);
        // @ts-ignore
        return new Handlebars.SafeString(res);
    }
}
// usage: {{ x-concat 'prop1' 'prop2' 'prop3' sep="," }}
function concatHelper(...args) {
    var _a;
    const options = args.pop();
    const sep = ((_a = options.hash) === null || _a === void 0 ? void 0 : _a.sep) || ''; // Default to empty string if no separator is provided
    return saveResultToTempData(options, () => args.filter(x => !(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(x)).join(sep));
    //return saveResultToTempData(() => args.filter(x => !isNullOrEmpty(x)).join(sep), ...arguments);
    // @ts-ignore
    //return args.filter(x => !isNullOrEmpty(x)).join(sep);
}
function saveResultToTempData(options, fn) {
    var _a;
    const res = fn();
    const varName = (_a = options === null || options === void 0 ? void 0 : options.hash) === null || _a === void 0 ? void 0 : _a.var;
    if (!(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(varName)) {
        globalVarTemp[varName] = res;
    }
    return res;
}
// function saveResultToTempData(fn: Function, ...args: any): any {
//     // @ts-ignore
//     const res = fn.call(this, ...args);
//     if (arguments.length > 0) {
//         const ctx = arguments[arguments.length - 1] as HbsDataContext;
//         const varName = ctx?.hash?.var;
//
//         if (!isNullOrEmpty(varName)) {
//             ctx.data['temp'] = ctx.data['temp'] ?? {};
//             // @ts-ignore
//             ctx.data['temp'][varName] = res;
//         }
//     }
//    
//     return res;
// }
function replaceHelper(value, options) {
    var _a, _b;
    const what = ((_a = options.hash) === null || _a === void 0 ? void 0 : _a.what) || '', withStr = ((_b = options.hash) === null || _b === void 0 ? void 0 : _b.with) || '';
    if (!what || !withStr || (what === withStr)) {
        return value;
    }
    const regex = new RegExp(what, 'g');
    return value.replace(regex, withStr);
}
// usage: {{x-trimEnd fullPath "/index.html"}}
function trimEndHelper(input, endPattern) {
    try {
        const regex = new RegExp(endPattern + '$');
        return input.replace(regex, '');
    }
    catch (error) {
        _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog('Invalid regex pattern:' + endPattern);
        return input;
    }
}
function equalsHelper(input, value, options) {
    const { cs, val1, val2 } = getDataForCompare(input, value, options);
    return cs ? val1 === val2 : (val1.toLowerCase() === val2.toLowerCase());
}
function isTrueHelper(input) {
    return input === true;
}
function isFalseHelper(input) {
    return input === false;
}
function getDataForCompare(input, value, options) {
    var _a, _b, _c;
    const cs = (((_a = options.hash) === null || _a === void 0 ? void 0 : _a.cs) || "true").toString().toLowerCase() == "true";
    const val1 = typeof input === "string" ? input : ((_b = input === null || input === void 0 ? void 0 : input.toString()) !== null && _b !== void 0 ? _b : '');
    const val2 = typeof value === "string" ? value : ((_c = value === null || value === void 0 ? void 0 : value.toString()) !== null && _c !== void 0 ? _c : '');
    return { cs: cs, val1, val2 };
}
function logicalHelper(input, value, options) {
    var _a;
    const op = (((_a = options.hash) === null || _a === void 0 ? void 0 : _a.op) || 'and').toLowerCase();
    if (op === 'and') {
        //return input === value;
        return (input === true) && (value === true);
    }
    else if (op === 'or') {
        //return input || value;
        return (input === true) || (value === true);
    }
    return false;
}
function trimComment(value) {
    if ((0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(value)) {
        return '';
    }
    let trimmed = false;
    var idxNewLine = value.indexOf('\r\n');
    if (idxNewLine == -1) {
        idxNewLine = value.indexOf('\n');
    }
    if (idxNewLine == -1) {
        idxNewLine = value.indexOf('\r');
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
    return value.replace('\t', ' ');
}
function resourceCommentHelper(resource, options) {
    var _a, _b, _c, _d;
    if (typeof resource === 'object') {
        const model = ((_a = options.hash) === null || _a === void 0 ? void 0 : _a.root).model;
        const primaryLanguage = (_c = (_b = model === null || model === void 0 ? void 0 : model.model) === null || _b === void 0 ? void 0 : _b.primaryLanguage) !== null && _c !== void 0 ? _c : '';
        if (!(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(primaryLanguage) && resource.values) {
            const resourceValue = (_d = resource.values[primaryLanguage]) === null || _d === void 0 ? void 0 : _d.value;
            let propertyComment = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(resourceValue) ? resource.description : resourceValue;
            //try {
            propertyComment = trimComment(propertyComment);
            // }
            // catch(err) {
            //     const s1 = `res_value_${primaryLanguage}: ${resource.values[primaryLanguage]?.value}`;
            //     const s2 = 'res_name: ' + resource.getName!();
            //     HostEnv.debugLog('[resourceCommentHelper] failed for: ' + (isNullOrEmpty(propertyComment) ? 'null': JSON.stringify(propertyComment)) + `, ${s1}, ${s2}`);
            // }
            // @ts-ignore
            return new Handlebars.SafeString(propertyComment);
        }
    }
    return '';
}
function resourceValueHelper(resource, options) {
    var _a, _b, _c, _d, _e, _f, _g;
    if (typeof resource === 'object') {
        const lang = (_b = (_a = options.hash) === null || _a === void 0 ? void 0 : _a.lang) !== null && _b !== void 0 ? _b : '';
        const trim = (_d = (_c = options.hash) === null || _c === void 0 ? void 0 : _c.trim) !== null && _d !== void 0 ? _d : false;
        if (!(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(lang)) {
            const res = (_g = (_f = (_e = resource === null || resource === void 0 ? void 0 : resource.values) === null || _e === void 0 ? void 0 : _e[lang]) === null || _f === void 0 ? void 0 : _f.value) !== null && _g !== void 0 ? _g : '';
            return trim ? res.trim() : res;
        }
    }
    return '';
}
function resourceHasLangHelper(resource, options) {
    var _a, _b;
    if (typeof resource === 'object') {
        const lang = (_b = (_a = options.hash) === null || _a === void 0 ? void 0 : _a.lang) !== null && _b !== void 0 ? _b : '';
        if (!(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(lang) && resource.values && resource.values[lang]) {
            return true;
        }
    }
    return false;
}
function resourceParamNamesHelper(resource, options) {
    var _a, _b;
    if (typeof resource === 'object' && resource.parameters) {
        const withTypes = (_b = (_a = options === null || options === void 0 ? void 0 : options.hash) === null || _a === void 0 ? void 0 : _a.withTypes) !== null && _b !== void 0 ? _b : false;
        return Object.keys(resource.parameters).map(key => {
            return withTypes ? `object ${key}` : key;
        }).join(',');
    }
    return '';
}
function mergeHelper(context, options) {
    var _a;
    return Object.assign({}, context, (_a = options.hash) !== null && _a !== void 0 ? _a : {});
}
function sortByHelper(source, propName, sortOrder = 'asc') {
    return (0,_utils__WEBPACK_IMPORTED_MODULE_0__.sortBy)(source, propName, sortOrder);
}
function sortObjectByKeyHelper(obj, options) {
    var _a, _b;
    const sortOrder = (_b = (_a = options === null || options === void 0 ? void 0 : options.hash) === null || _a === void 0 ? void 0 : _a.sortOrder) !== null && _b !== void 0 ? _b : 'asc';
    return (0,_utils__WEBPACK_IMPORTED_MODULE_0__.sortObjectByKey)(obj, sortOrder);
}
function objCountHelper(obj) {
    return (0,_utils__WEBPACK_IMPORTED_MODULE_0__.objCount)(obj);
}
function hasItemsHelper(obj) {
    return (0,_utils__WEBPACK_IMPORTED_MODULE_0__.hasItems)(obj);
}
function textEncodeHelper(str, options) {
    var _a, _b, _c, _d;
    const mode = (_b = (_a = options === null || options === void 0 ? void 0 : options.hash) === null || _a === void 0 ? void 0 : _a.mode) !== null && _b !== void 0 ? _b : 'html';
    const quotes = (_d = (_c = options === null || options === void 0 ? void 0 : options.hash) === null || _c === void 0 ? void 0 : _c.quotes) !== null && _d !== void 0 ? _d : false;
    const s = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.textEncode)(str, { mode: mode, quotes });
    // @ts-ignore
    return new Handlebars.SafeString(s);
}
function hostWebHtmlEncodeHelper(str) {
    if ((0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(str)) {
        return str;
    }
    const encoded = _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.webHtmlEncode(str);
    // @ts-ignore
    return new Handlebars.SafeString(encoded);
}
function renderHelper(input, options) {
    var _a, _b;
    const when = (_b = (_a = options === null || options === void 0 ? void 0 : options.hash) === null || _a === void 0 ? void 0 : _a.when) !== null && _b !== void 0 ? _b : true;
    // @ts-ignore
    return when ? input : '';
}
function isNullOrEmptyHelper(value) {
    return (0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(value);
}
function isNotNullOrEmptyHelper(input) {
    return !(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(input);
}
function callFunctionHelper(fn, ...args) {
    let fnArgs = undefined;
    if (arguments.length > 0) {
        //const ctx = arguments[arguments.length - 1] as HbsDataContext;
        fnArgs = Array.from(args).slice(0, -1);
    }
    return fnArgs === undefined ? fn() : fn(...fnArgs);
}
// function callFunctionHelper(fn: any): any {
//     return fn();
// }
function debugLogHelper(...args) {
    _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog(args.join(' '));
    return '';
}
function returnVarFromTempHelper(name, options) {
    var _a, _b;
    //const name = options?.hash?.name;
    const defaultVal = (_a = options === null || options === void 0 ? void 0 : options.hash) === null || _a === void 0 ? void 0 : _a.default;
    if ((0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(name)) {
        return '';
    }
    return (_b = globalVarTemp[name]) !== null && _b !== void 0 ? _b : defaultVal;
}


/***/ }),

/***/ "./src/hostEnv.ts":
/*!************************!*\
  !*** ./src/hostEnv.ts ***!
  \************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   HostEnv: () => (/* binding */ HostEnv)
/* harmony export */ });
// declare let HostEnv: {
//     DebugLog: (msg: string) => void;
//     AddResultFile: (name: string, content: string) => void;
// }
class HostEnv {
    static addResultFile(name, content, bom, lineEndings) {
        if (HostAddResultFile) {
            HostAddResultFile(name, content, bom, lineEndings);
            //HostEnv.debugLog(`Added file '${name}' with content: ${content}`);
        }
        else {
            console.log(`Added file '${name}' with content: '${content}'`);
        }
    }
    static addModelGroupSettings(group, settings) {
        const json = JSON.stringify(settings);
        if (HostAddModelGroupSettings) {
            HostAddModelGroupSettings(group, json);
        }
        else {
            console.log(`Added model group '${group}' with settings: ` + json);
        }
    }
    static debugLog(msg) {
        if (HostDebugLog) {
            HostDebugLog(msg);
        }
        else {
            console.log(msg);
        }
    }
    static pathCombine(path1, path2) {
        if (HostPathCombine) {
            return HostPathCombine(path1, path2);
        }
        return path1 + '/' + path2;
    }
    static webHtmlEncode(input) {
        if (HostWebHtmlEncode) {
            return HostWebHtmlEncode(input);
        }
        return input;
    }
    static stopwatchStart() {
        if (HostStopwatchStart) {
            return HostStopwatchStart();
        }
        return Date.now();
    }
    static stopwatchEnd(start) {
        if (HostStopwatchEnd) {
            return HostStopwatchEnd(start);
        }
        return (Date.now() - start).toString();
    }
}


/***/ }),

/***/ "./src/templateManager.ts":
/*!********************************!*\
  !*** ./src/templateManager.ts ***!
  \********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   TemplateManager: () => (/* binding */ TemplateManager)
/* harmony export */ });
/* harmony import */ var _helpers__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./helpers */ "./src/helpers.ts");
/* harmony import */ var _templates_typescriptJson__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./templates/typescriptJson */ "./src/templates/typescriptJson.ts");
/* harmony import */ var _templates_netCoreResxCsharp__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./templates/netCoreResxCsharp */ "./src/templates/netCoreResxCsharp.ts");
/* harmony import */ var _templates_netFwResxCsharp__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./templates/netFwResxCsharp */ "./src/templates/netFwResxCsharp.ts");
/* harmony import */ var _templates_winFormsResxCsharp__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./templates/winFormsResxCsharp */ "./src/templates/winFormsResxCsharp.ts");
/* harmony import */ var _templates_wpfResxCsharp__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./templates/wpfResxCsharp */ "./src/templates/wpfResxCsharp.ts");
/* harmony import */ var _utils__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./utils */ "./src/utils.ts");
/* harmony import */ var _AppError__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./AppError */ "./src/AppError.ts");
// noinspection JSUnusedGlobalSymbols








const CodeGenUID = 'b40c8a1d-23b7-4f78-991b-c24898596dd2';
class TemplateManager {
    static intialize(handlebarFiles) {
        TemplateManager.handlebarFiles = JSON.parse(handlebarFiles);
        // @ts-ignore
        String.prototype.isTrue = function () {
            return this.toLowerCase() === "true";
        };
        (0,_helpers__WEBPACK_IMPORTED_MODULE_0__.registerHelpers)();
    }
    static runTemplate(lhqModelJson, hostData) {
        let lhqModel = JSON.parse(lhqModelJson);
        if (lhqModel) {
            lhqModel = TemplateManager.sortByNameModel(lhqModel);
            const { template, templateId, settingsNode } = TemplateManager.loadTemplate(lhqModel);
            if ((0,_utils__WEBPACK_IMPORTED_MODULE_6__.isNullOrEmpty)(template)) {
                throw new _AppError__WEBPACK_IMPORTED_MODULE_7__.AppError(`Failed to load template '${templateId}' !`);
            }
            let settings = template.loadSettings(settingsNode);
            let host = {};
            if (hostData) {
                host = JSON.parse(hostData);
            }
            const rootModel = {
                model: lhqModel,
                settings: settings,
                host: host,
                extra: {}
            };
            template.generate(rootModel);
            (0,_helpers__WEBPACK_IMPORTED_MODULE_0__.debugHelpersTimeTaken)();
            (0,_helpers__WEBPACK_IMPORTED_MODULE_0__.clearHelpersContext)();
        }
        else {
            throw new Error(`Unable to deserialize LHQ model !`);
        }
    }
    static loadTemplate(model) {
        var _a, _b, _c;
        let template = undefined;
        let templateId = '';
        let node = (_b = (_a = model.metadatas) === null || _a === void 0 ? void 0 : _a.childs) === null || _b === void 0 ? void 0 : _b.find(x => { var _a; return x.name === 'metadata' && ((_a = x.attrs) === null || _a === void 0 ? void 0 : _a['descriptorUID']) === CodeGenUID; });
        if (node) {
            node = (_c = node.childs) === null || _c === void 0 ? void 0 : _c.find(x => { var _a; return x.name === 'content' && ((_a = x.attrs) === null || _a === void 0 ? void 0 : _a['templateId']) !== undefined; });
            if (node) {
                templateId = node === null || node === void 0 ? void 0 : node.attrs['templateId'];
                node = node.childs.find(x => { var _a; return x.name === 'Settings' && ((_a = x.childs) === null || _a === void 0 ? void 0 : _a.length) > 0; });
            }
        }
        if (node && templateId !== undefined && template !== '') {
            const ctor = TemplateManager.generators[templateId];
            template = (ctor && new ctor(TemplateManager.handlebarFiles)) || undefined;
            return { template, templateId, settingsNode: node };
        }
        throw new Error(`Template '${templateId}' not found !`);
    }
    static sortByNameModel(lhqModel) {
        function getFullParentPath(sep, element) {
            let pathArray = [];
            let currentElement = element.getParent();
            pathArray.unshift(element.getName());
            while (currentElement) {
                if (currentElement === null || currentElement === void 0 ? void 0 : currentElement.isRoot()) {
                    break;
                }
                pathArray.unshift(currentElement.getName());
                currentElement = currentElement.getParent();
            }
            return pathArray.join(sep);
        }
        function recursiveCategories(parentCategory) {
            if (parentCategory.categories) {
                parentCategory.categories = (0,_utils__WEBPACK_IMPORTED_MODULE_6__.sortObjectByKey)(parentCategory.categories);
                (0,_utils__WEBPACK_IMPORTED_MODULE_6__.iterateObject)(parentCategory.categories, (category, name, __, isLastCategory) => {
                    category.getName = () => name;
                    category.isRoot = () => false;
                    category.isLast = () => isLastCategory;
                    category.getParent = () => parentCategory;
                    category.hasCategories = () => (0,_utils__WEBPACK_IMPORTED_MODULE_6__.hasItems)(parentCategory.categories);
                    category.hasResources = () => (0,_utils__WEBPACK_IMPORTED_MODULE_6__.hasItems)(parentCategory.resources);
                    category.getFullParentPath = (sep) => getFullParentPath(sep, category);
                    recursiveCategories(category);
                });
            }
            if (parentCategory.resources) {
                parentCategory.resources = (0,_utils__WEBPACK_IMPORTED_MODULE_6__.sortObjectByKey)(parentCategory.resources);
                (0,_utils__WEBPACK_IMPORTED_MODULE_6__.iterateObject)(parentCategory.resources, (resource, name, __, isLastResource) => {
                    resource.getName = () => name;
                    resource.isLast = () => isLastResource;
                    resource.getParent = () => parentCategory;
                    resource.hasParameters = () => (0,_utils__WEBPACK_IMPORTED_MODULE_6__.hasItems)(resource.parameters);
                    resource.getFullParentPath = (sep) => getFullParentPath(sep, resource);
                    if (resource.parameters) {
                        resource.parameters = (0,_utils__WEBPACK_IMPORTED_MODULE_6__.sortObjectByValue)(resource.parameters, x => x.order);
                        (0,_utils__WEBPACK_IMPORTED_MODULE_6__.iterateObject)(resource.parameters, (parameter, _, __, isLastParam) => {
                            parameter.isLast = () => isLastParam;
                            parameter.getParent = () => resource;
                        });
                    }
                });
            }
        }
        lhqModel.getName = () => lhqModel.model.name;
        lhqModel.isRoot = () => true;
        lhqModel.isLast = () => true;
        lhqModel.getParent = () => undefined;
        lhqModel.hasCategories = () => (0,_utils__WEBPACK_IMPORTED_MODULE_6__.hasItems)(lhqModel.categories);
        lhqModel.hasResources = () => (0,_utils__WEBPACK_IMPORTED_MODULE_6__.hasItems)(lhqModel.resources);
        lhqModel.getFullParentPath = () => '';
        recursiveCategories(lhqModel);
        return lhqModel;
    }
}
TemplateManager.generators = {
    [_templates_typescriptJson__WEBPACK_IMPORTED_MODULE_1__.TypescriptJson01Template.Id]: _templates_typescriptJson__WEBPACK_IMPORTED_MODULE_1__.TypescriptJson01Template,
    [_templates_netCoreResxCsharp__WEBPACK_IMPORTED_MODULE_2__.NetCoreResxCsharp01Template.Id]: _templates_netCoreResxCsharp__WEBPACK_IMPORTED_MODULE_2__.NetCoreResxCsharp01Template,
    [_templates_netFwResxCsharp__WEBPACK_IMPORTED_MODULE_3__.NetFwResxCsharp01Template.Id]: _templates_netFwResxCsharp__WEBPACK_IMPORTED_MODULE_3__.NetFwResxCsharp01Template,
    [_templates_winFormsResxCsharp__WEBPACK_IMPORTED_MODULE_4__.WinFormsResxCsharp01Template.Id]: _templates_winFormsResxCsharp__WEBPACK_IMPORTED_MODULE_4__.WinFormsResxCsharp01Template,
    [_templates_wpfResxCsharp__WEBPACK_IMPORTED_MODULE_5__.WpfResxCsharp01Template.Id]: _templates_wpfResxCsharp__WEBPACK_IMPORTED_MODULE_5__.WpfResxCsharp01Template
};


/***/ }),

/***/ "./src/templates/codeGeneratorTemplate.ts":
/*!************************************************!*\
  !*** ./src/templates/codeGeneratorTemplate.ts ***!
  \************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   CodeGeneratorTemplate: () => (/* binding */ CodeGeneratorTemplate)
/* harmony export */ });
/* harmony import */ var _utils__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../utils */ "./src/utils.ts");
/* harmony import */ var _hostEnv__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../hostEnv */ "./src/hostEnv.ts");
/* harmony import */ var _helpers__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../helpers */ "./src/helpers.ts");



class CodeGeneratorTemplate {
    constructor(handlebarFiles) {
        this.handlebarFiles = handlebarFiles;
    }
    getHandlebarFile(templateName) {
        const file = this.handlebarFiles[templateName];
        if (file === undefined || file === '') {
            throw new Error(`Handlebar file with name '${templateName}' not found !`);
        }
        return file;
    }
    prepareFilePath(fileName, outputSettings) {
        const outputFolder = outputSettings.OutputFolder;
        return (0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(outputFolder) ? fileName : _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.pathCombine(outputFolder, fileName);
    }
    addResultFile(name, content, outputSettings) {
        var _a, _b;
        _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.addResultFile(name, content, (_a = outputSettings.EncodingWithBOM) !== null && _a !== void 0 ? _a : false, (_b = outputSettings.LineEndings) !== null && _b !== void 0 ? _b : 'lf');
    }
    setDefaults(outputSettings) {
        var _a;
        outputSettings.EncodingWithBOM = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.valueOrDefault)(outputSettings.EncodingWithBOM, false);
        outputSettings.LineEndings = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.valueOrDefault)(outputSettings.LineEndings, 'lf');
        outputSettings.Enabled = (_a = outputSettings.Enabled) !== null && _a !== void 0 ? _a : true.toString();
    }
    addModelGroupSettings(group, settings, keysToSkip) {
        let obj = settings;
        if (!(0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(keysToSkip) && keysToSkip.length > 0) {
            obj = (0,_utils__WEBPACK_IMPORTED_MODULE_0__.copyObject)(settings, keysToSkip);
        }
        _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.addModelGroupSettings(group, obj);
    }
    compileAndRun(templateFileName, data) {
        let compiled;
        if (this.lastCompiledTemplate === undefined || this.lastCompiledTemplate.templateFileName.toLowerCase() !== templateFileName.toLowerCase()) {
            const handlebarsTemplate = this.getHandlebarFile(templateFileName);
            const options = { knownHelpers: (0,_helpers__WEBPACK_IMPORTED_MODULE_2__.getKnownHelpers)() };
            // @ts-ignore
            compiled = Handlebars.compile(handlebarsTemplate, options);
            this.lastCompiledTemplate = {
                templateFileName: templateFileName,
                compiled: compiled
            };
        }
        else {
            compiled = this.lastCompiledTemplate.compiled;
        }
        if ((0,_utils__WEBPACK_IMPORTED_MODULE_0__.isNullOrEmpty)(compiled)) {
            throw new Error(`Template '${templateFileName}' was not found !`);
        }
        (0,_helpers__WEBPACK_IMPORTED_MODULE_2__.clearHelpersContext)();
        let result = compiled(data);
        result = result.replace(/\t¤$/gm, "");
        return result;
    }
}


/***/ }),

/***/ "./src/templates/csharpResXTemplateBase.ts":
/*!*************************************************!*\
  !*** ./src/templates/csharpResXTemplateBase.ts ***!
  \*************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   CSharpResXTemplateBase: () => (/* binding */ CSharpResXTemplateBase)
/* harmony export */ });
/* harmony import */ var _codeGeneratorTemplate__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./codeGeneratorTemplate */ "./src/templates/codeGeneratorTemplate.ts");
/* harmony import */ var _hostEnv__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../hostEnv */ "./src/hostEnv.ts");
/* harmony import */ var _utils__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../utils */ "./src/utils.ts");
/* harmony import */ var _AppError__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../AppError */ "./src/AppError.ts");




class CSharpResXTemplateBase extends _codeGeneratorTemplate__WEBPACK_IMPORTED_MODULE_0__.CodeGeneratorTemplate {
    constructor(handlebarFiles) {
        super(handlebarFiles);
        this._defaultCompatibleTextEncoding = true;
    }
    checkHasNamespaceName(rootModel) {
        const key = 'namespace';
        if ((0,_utils__WEBPACK_IMPORTED_MODULE_2__.isNullOrEmpty)(rootModel.host[key])) {
            throw new _AppError__WEBPACK_IMPORTED_MODULE_3__.AppError(`Missing value for parameter '${key}'.\n` +
                `> provide valid path to *.csproj which uses required lhq model\n` +
                `> or provide value for parameter '${key}' in cmd data parameters`);
        }
    }
    debugLog(msg) {
        _hostEnv__WEBPACK_IMPORTED_MODULE_1__.HostEnv.debugLog(msg);
    }
    generate(rootModel) {
        //const modelVersion = rootModel.model.model.version;
        // if (modelVersion < 2) {
        //     throw new AppError(`Current LHQ file version (${modelVersion}) is not supported! (min version 2 is supported)`);
        // }
        var _a, _b;
        //const defaultCompatibleTextEncoding = true; //modelVersion < 2;
        const modelName = rootModel.model.model.name;
        rootModel.extra = (_a = rootModel.extra) !== null && _a !== void 0 ? _a : {};
        if (this._settings.CSharp.Enabled.isTrue()) {
            this.checkHasNamespaceName(rootModel);
            rootModel.extra['rootClassName'] = this.getRootCsharpClassName(rootModel);
            const csfileContent = this.compileAndRun(this.csharpTemplateName, rootModel);
            const csFileName = this.prepareFilePath(modelName + '.gen.cs', this._settings.CSharp);
            this.addResultFile(csFileName, csfileContent, this._settings.CSharp);
            this.addModelGroupSettings('CSharp', this._settings.CSharp, ['Enabled']);
        }
        if (this._settings.ResX.Enabled.isTrue()) {
            //this._settings.ResX.CompatibleTextEncoding = valueOrDefault(this._settings.ResX.CompatibleTextEncoding, defaultCompatibleTextEncoding.toString());
            rootModel.extra['useHostWebHtmlEncode'] = this._settings.ResX.CompatibleTextEncoding.isTrue();
            this.addModelGroupSettings('ResX', this._settings.ResX, ['Enabled']);
            (_b = rootModel.model.languages) === null || _b === void 0 ? void 0 : _b.forEach(lang => {
                if (!(0,_utils__WEBPACK_IMPORTED_MODULE_2__.isNullOrEmpty)(lang)) {
                    rootModel.extra['lang'] = lang;
                    const resxfileContent = this.compileAndRun('SharedResx', rootModel);
                    const resxfileName = this.prepareFilePath(`${modelName}.${lang}.resx`, this._settings.ResX);
                    this.addResultFile(resxfileName, resxfileContent, this._settings.ResX);
                }
            });
        }
    }
    loadSettings(node) {
        var _a;
        const result = {
            CSharp: undefined,
            ResX: undefined
        };
        (_a = node.childs) === null || _a === void 0 ? void 0 : _a.forEach(x => {
            const attrs = x.attrs;
            switch (x.name) {
                case 'CSharp':
                    result.CSharp = attrs;
                    break;
                case 'ResX':
                    result.ResX = attrs;
                    break;
            }
        });
        if (result.CSharp === undefined) {
            throw new Error('CSharp settings not found !');
        }
        if (result.ResX === undefined) {
            throw new Error('ResX settings not found !');
        }
        // result.CSharp.Enabled = result.CSharp.Enabled ?? true.toString();
        // result.ResX.Enabled = result.ResX.Enabled ?? true.toString();
        this.setDefaults(result.CSharp);
        this.setDefaults(result.ResX);
        // @ts-ignore
        result.ResX.CompatibleTextEncoding = (0,_utils__WEBPACK_IMPORTED_MODULE_2__.valueOrDefault)(result.ResX.CompatibleTextEncoding, this._defaultCompatibleTextEncoding.toString());
        this._settings = result;
        return result;
    }
}


/***/ }),

/***/ "./src/templates/netCoreResxCsharp.ts":
/*!********************************************!*\
  !*** ./src/templates/netCoreResxCsharp.ts ***!
  \********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   NetCoreResxCsharp01Template: () => (/* binding */ NetCoreResxCsharp01Template)
/* harmony export */ });
/* harmony import */ var _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./csharpResXTemplateBase */ "./src/templates/csharpResXTemplateBase.ts");

class NetCoreResxCsharp01Template extends _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__.CSharpResXTemplateBase {
    static get Id() {
        return 'NetCoreResxCsharp01';
    }
    get csharpTemplateName() {
        return NetCoreResxCsharp01Template.Id;
    }
    getRootCsharpClassName(rootModel) {
        return rootModel.model.model.name + 'Localizer';
    }
}


/***/ }),

/***/ "./src/templates/netFwResxCsharp.ts":
/*!******************************************!*\
  !*** ./src/templates/netFwResxCsharp.ts ***!
  \******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   NetFwResxCsharp01Template: () => (/* binding */ NetFwResxCsharp01Template)
/* harmony export */ });
/* harmony import */ var _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./csharpResXTemplateBase */ "./src/templates/csharpResXTemplateBase.ts");

class NetFwResxCsharp01Template extends _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__.CSharpResXTemplateBase {
    static get Id() {
        return 'NetFwResxCsharp01';
    }
    get csharpTemplateName() {
        return NetFwResxCsharp01Template.Id;
    }
    getRootCsharpClassName(rootModel) {
        return rootModel.model.model.name + 'Context';
    }
}


/***/ }),

/***/ "./src/templates/typescriptJson.ts":
/*!*****************************************!*\
  !*** ./src/templates/typescriptJson.ts ***!
  \*****************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   TypescriptJson01Template: () => (/* binding */ TypescriptJson01Template)
/* harmony export */ });
/* harmony import */ var _codeGeneratorTemplate__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./codeGeneratorTemplate */ "./src/templates/codeGeneratorTemplate.ts");
/* harmony import */ var _utils__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils */ "./src/utils.ts");


class TypescriptJson01Template extends _codeGeneratorTemplate__WEBPACK_IMPORTED_MODULE_0__.CodeGeneratorTemplate {
    generate(rootModel) {
        var _a;
        const model = rootModel.model.model;
        const modelName = model.name;
        if (this._settings.Typescript.Enabled.isTrue()) {
            const dtsFileContent = this.compileAndRun(TypescriptJson01Template.Id, rootModel);
            const dtsFileName = this.prepareFilePath(modelName + '.d.ts', this._settings.Typescript);
            this.addResultFile(dtsFileName, dtsFileContent, this._settings.Typescript);
            this.addModelGroupSettings('Typescript', this._settings.Typescript, ['Enabled']);
        }
        if (this._settings.Json.Enabled.isTrue()) {
            //const metadataFileNameSuffix = valueOrDefault(this._settings.Json.MetadataFileNameSuffix, 'metadata');
            const metadataFileNameSuffix = this._settings.Json.MetadataFileNameSuffix;
            const metadataObj = {
                default: model.primaryLanguage,
                languages: (0,_utils__WEBPACK_IMPORTED_MODULE_1__.sortBy)(rootModel.model.languages, undefined, 'asc')
            };
            const metadataContent = JSON.stringify(metadataObj, null, '\t') + '\n';
            const metadataFileName = this.prepareFilePath(`${modelName}-${metadataFileNameSuffix}.json`, this._settings.Json);
            this.addResultFile(metadataFileName, metadataContent, this._settings.Json);
            this.addModelGroupSettings('Json', this._settings.Json, ['Enabled']);
            rootModel.extra = {};
            const writeEmptyValues = this._settings.Json.WriteEmptyValues.isTrue();
            const allFilesHasLangInName = this._settings.Json.CultureCodeInFileNameForPrimaryLanguage.isTrue();
            (_a = rootModel.model.languages) === null || _a === void 0 ? void 0 : _a.forEach(lang => {
                if (!(0,_utils__WEBPACK_IMPORTED_MODULE_1__.isNullOrEmpty)(lang)) {
                    const isPrimary = model.primaryLanguage === lang;
                    const langName = !isPrimary || allFilesHasLangInName ? `.${lang}` : '';
                    rootModel.extra['lang'] = lang;
                    rootModel.extra['writeEmptyValues'] = writeEmptyValues;
                    const jsonFileContent = this.compileAndRun('JsonPerLanguage', rootModel);
                    const jsonfileName = this.prepareFilePath(`${modelName}${langName}.json`, this._settings.Json);
                    this.addResultFile(jsonfileName, jsonFileContent, this._settings.Json);
                }
            });
        }
    }
    loadSettings(node) {
        var _a;
        const result = { Typescript: undefined, Json: undefined };
        (_a = node.childs) === null || _a === void 0 ? void 0 : _a.forEach(x => {
            const attrs = x.attrs;
            switch (x.name) {
                case 'Typescript':
                    result.Typescript = attrs;
                    break;
                case 'Json':
                    result.Json = attrs;
                    break;
            }
        });
        if (result.Typescript === undefined) {
            throw new Error('Typescript settings not found !');
        }
        if (result.Json === undefined) {
            throw new Error('Json settings not found !');
        }
        //result.Typescript.Enabled = result.Typescript.Enabled ?? true.toString();
        this.setDefaults(result.Typescript);
        //result.Json.Enabled = result.Json.Enabled ?? true.toString();
        this.setDefaults(result.Json);
        result.Json.MetadataFileNameSuffix = (0,_utils__WEBPACK_IMPORTED_MODULE_1__.valueOrDefault)(result.Json.MetadataFileNameSuffix, 'metadata');
        this._settings = result;
        return result;
    }
    static get Id() {
        return 'TypescriptJson01';
    }
}


/***/ }),

/***/ "./src/templates/winFormsResxCsharp.ts":
/*!*********************************************!*\
  !*** ./src/templates/winFormsResxCsharp.ts ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   WinFormsResxCsharp01Template: () => (/* binding */ WinFormsResxCsharp01Template)
/* harmony export */ });
/* harmony import */ var _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./csharpResXTemplateBase */ "./src/templates/csharpResXTemplateBase.ts");

class WinFormsResxCsharp01Template extends _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__.CSharpResXTemplateBase {
    static get Id() {
        return 'WinFormsResxCsharp01';
    }
    get csharpTemplateName() {
        return WinFormsResxCsharp01Template.Id;
    }
    getRootCsharpClassName(rootModel) {
        return rootModel.model.model.name + 'Context';
    }
    loadSettings(node) {
        var _a, _b, _c;
        const settings = super.loadSettings(node);
        settings.CSharp.ParamsMethodsSuffix = (_a = settings.CSharp.ParamsMethodsSuffix) !== null && _a !== void 0 ? _a : 'WithParams';
        settings.CSharp.GenerateParamsMethods = (_b = settings.CSharp.GenerateParamsMethods) !== null && _b !== void 0 ? _b : true.toString();
        settings.CSharp.MissingTranslationFallbackToPrimary = (_c = settings.CSharp.MissingTranslationFallbackToPrimary) !== null && _c !== void 0 ? _c : false.toString();
        return settings;
    }
    generate(rootModel) {
        var _a, _b;
        if (this._settings.CSharp.Enabled.isTrue()) {
            rootModel.extra = (_a = rootModel.extra) !== null && _a !== void 0 ? _a : {};
            const generateParamsMethods = this._settings.CSharp.GenerateParamsMethods.isTrue();
            rootModel.extra['generateParamsMethods'] = generateParamsMethods;
            rootModel.extra['paramsMethodsSuffix'] = (generateParamsMethods ? ((_b = this._settings.CSharp.ParamsMethodsSuffix) !== null && _b !== void 0 ? _b : 'WithParams') : '');
            const bindableFileName = this.prepareFilePath('BindableObject.gen.cs', this._settings.CSharp);
            const bindableContent = this.compileAndRun('WinFormsBindableObject', rootModel);
            this.addResultFile(bindableFileName, bindableContent, this._settings.CSharp);
        }
        super.generate(rootModel);
    }
}


/***/ }),

/***/ "./src/templates/wpfResxCsharp.ts":
/*!****************************************!*\
  !*** ./src/templates/wpfResxCsharp.ts ***!
  \****************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   WpfResxCsharp01Template: () => (/* binding */ WpfResxCsharp01Template)
/* harmony export */ });
/* harmony import */ var _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./csharpResXTemplateBase */ "./src/templates/csharpResXTemplateBase.ts");

class WpfResxCsharp01Template extends _csharpResXTemplateBase__WEBPACK_IMPORTED_MODULE_0__.CSharpResXTemplateBase {
    static get Id() {
        return 'WpfResxCsharp01';
    }
    get csharpTemplateName() {
        return WpfResxCsharp01Template.Id;
    }
    getRootCsharpClassName(rootModel) {
        return rootModel.model.model.name + 'Context';
    }
}


/***/ }),

/***/ "./src/utils.ts":
/*!**********************!*\
  !*** ./src/utils.ts ***!
  \**********************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   copyObject: () => (/* binding */ copyObject),
/* harmony export */   getNestedPropertyValue: () => (/* binding */ getNestedPropertyValue),
/* harmony export */   hasItems: () => (/* binding */ hasItems),
/* harmony export */   isNullOrEmpty: () => (/* binding */ isNullOrEmpty),
/* harmony export */   iterateObject: () => (/* binding */ iterateObject),
/* harmony export */   objCount: () => (/* binding */ objCount),
/* harmony export */   sortBy: () => (/* binding */ sortBy),
/* harmony export */   sortObjectByKey: () => (/* binding */ sortObjectByKey),
/* harmony export */   sortObjectByValue: () => (/* binding */ sortObjectByValue),
/* harmony export */   textEncode: () => (/* binding */ textEncode),
/* harmony export */   toBoolean: () => (/* binding */ toBoolean),
/* harmony export */   valueAsBool: () => (/* binding */ valueAsBool),
/* harmony export */   valueOrDefault: () => (/* binding */ valueOrDefault)
/* harmony export */ });
//const he = require('he');
function getNestedPropertyValue(obj, path) {
    return path.split('.').reduce((acc, part) => {
        if (acc === undefined)
            return undefined;
        // Check if the part includes an array index like "c[1]"
        const match = part.match(/^(\w+)\[(\d+)]$/);
        if (match) {
            const [, property, index] = match;
            // @ts-ignore
            return Array.isArray(acc[property]) ? acc[property][index] : undefined;
        }
        // @ts-ignore
        return acc[part];
    }, obj);
}
/**
 * Checks if a `value` is null, undefined or empty string.
 *
 * @typeParam T - The type of the value.
 * @param value - The value to check.
 *
 * @returns `true` if the value is null, undefined or empty string, `false` otherwise.
 */
function isNullOrEmpty(value) {
    return value === null || value === undefined || value === '';
}
function sortObjectByKey(obj, sortOrder = 'asc') {
    return Object.fromEntries(Object.entries(obj).sort(([a], [b]) => sortOrder === 'asc' ? a.localeCompare(b, 'en') : b.localeCompare(a, 'en')));
}
function sortObjectByValue(obj, predicate, sortOrder = 'asc') {
    return Object.fromEntries(Object.entries(obj).sort(([, a], [, b]) => {
        const aValue = predicate(a);
        const bValue = predicate(b);
        if (aValue < bValue) {
            return sortOrder === 'asc' ? -1 : 1;
        }
        if (aValue > bValue) {
            return sortOrder === 'asc' ? 1 : -1;
        }
        return 0;
    }));
}
function sortBy(source, propName, sortOrder = 'asc') {
    return source.concat([]).sort((a, b) => {
        // @ts-ignore
        const v1 = propName === undefined ? a : a[propName];
        // @ts-ignore
        const v2 = propName === undefined ? b : b[propName];
        const res = v1 > v2 ? 1 : ((v2 > v1) ? -1 : 0);
        return sortOrder === 'asc' ? res : res * -1;
    });
}
function iterateObject(obj, callback) {
    const entries = Object.entries(obj);
    if (entries.length > 0) {
        const lastIndex = entries.length - 1;
        let index = -1;
        for (const [key, value] of entries) {
            index++;
            const isLast = index == lastIndex;
            callback(value, key, index, isLast);
        }
    }
}
const encodingCharMaps = {
    html: {
        '>': '&gt;',
        '<': '&lt;',
        '"': '&quot;',
        "'": '&apos;',
        '&': '&amp;'
    },
    xml: {
        '>': '&gt;',
        '<': '&lt;',
        '&': '&amp;'
    },
    xml_quotes: {
        '>': '&gt;',
        '<': '&lt;',
        '"': '&quot;',
        "'": '&apos;',
        '&': '&amp;'
    },
    json: {
        '\\': '\\\\',
        '"': '\\"',
        '\b': '\\b',
        '\f': '\\f',
        '\n': '\\n',
        '\r': '\\r',
        '\t': '\\t'
    }
};
function textEncode(str, encoder) {
    var _a;
    if (isNullOrEmpty(str)) {
        return str;
    }
    const encodedChars = [];
    for (let i = 0; i < str.length; i++) {
        const ch = str.charAt(i);
        let map = undefined;
        if (encoder.mode === 'html') {
            map = encodingCharMaps.html;
        }
        else if (encoder.mode === 'xml') {
            map = ((_a = encoder.quotes) !== null && _a !== void 0 ? _a : true) ? encodingCharMaps.xml_quotes : encodingCharMaps.xml;
        }
        else {
            map = encodingCharMaps.json;
        }
        if (map.hasOwnProperty(ch)) {
            encodedChars.push(map[ch]);
        }
        else {
            encodedChars.push(ch);
        }
    }
    return encodedChars.join('');
}
function valueOrDefault(value, defaultValue) {
    let result = isNullOrEmpty(value)
        ? defaultValue
        : value;
    if (typeof defaultValue === 'boolean') {
        result = valueAsBool(value);
    }
    return result;
}
function valueAsBool(value) {
    switch (typeof value) {
        case 'boolean':
            return value;
        case 'number':
            return value > 0;
        case 'string':
            return value.toLowerCase() === 'true';
        default:
            return false;
    }
}
function toBoolean(value) {
    return value.toLowerCase() === 'true';
}
function hasItems(obj) {
    const result = objCount(obj) > 0;
    //HostEnv.debugLog(`[hasItems] returns '${result}' for obj: ${JSON.stringify(obj)}`);
    return result;
}
function objCount(obj) {
    if (isNullOrEmpty(obj)) {
        return 0;
    }
    if (Array.isArray(obj)) {
        return obj.length;
    }
    if (typeof obj === 'object') {
        return Object.keys(obj).length;
    }
    return 0;
}
function copyObject(obj, keysToSkip) {
    return Object.fromEntries(Object.entries(obj).filter(([key]) => !keysToSkip.includes(key)));
}


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry needs to be wrapped in an IIFE because it needs to be isolated against other modules in the chunk.
(() => {
/*!**********************!*\
  !*** ./src/index.ts ***!
  \**********************/
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   TemplateManager: () => (/* reexport safe */ _templateManager__WEBPACK_IMPORTED_MODULE_0__.TemplateManager)
/* harmony export */ });
/* harmony import */ var _templateManager__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./templateManager */ "./src/templateManager.ts");
//export * from './types';
// export * from './utils';


})();

LhqGenerators = __webpack_exports__;
/******/ })()
;