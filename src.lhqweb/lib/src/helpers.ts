import Handlebars from "handlebars";

import {
    copyObject,
    hasItems,
    isNullOrEmpty,
    jsonQuery,
    normalizePath,
    objCount,
    removeNewLines,
    removeProperties,
    sortBy,
    sortObjectByKey,
    textEncode,
    TextEncodeModes,
    valueAsBool,
    valueOrDefault,
} from "./utils";

import { AppError } from './AppError';
import { TreeElement } from './model/treeElement';
import { OutputFileData, TemplateRootModel } from './model/templateRootModel';
import { DefaultCodeGenSettings } from './model/modelConst';

export function registerHelpers() {
    Object.keys(helpersList).forEach(key => {
        // @ts-ignore
        Handlebars.registerHelper(key, helpersList[key]);
        //const fn = helpersList[key];
        //Handlebars.registerHelper(key, () => debugLogAndExec(key, fn, ...arguments));
    });

    clearHelpersContext();
}

const helpersList: Record<string, Function> = {
    // generic helpers
    'x-header': headerHelper,
    'x-normalizePath': normalizePathHelper,
    'x-value': valueHelper,
    'x-indent': indentHelper,
    'x-join': joinHelper,
    'x-split': splitHelper,
    'x-raw': rawHelper,
    'x-concat': concatHelper,
    'x-replace': replaceHelper,
    'x-trimEnd': trimEndHelper,
    'x-equals': equalsHelper,
    'x-isTrue': isTrueHelper,
    'x-isFalse': isFalseHelper,
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
    'x-stringify': stringifyHelper,
    'x-toJson': toJsonHelper,
    'x-typeOf': typeOfHelper,

    // model specific helpers
    'm-data': modelDataHelper,
    'm-output': modelOutputHelper,
    'm-output-child': modelOutputChildHelper,

    //'m-settings': modelSettingsHelper,
    //'x-resourceComment': resourceCommentHelper,
    // 'x-resourceValue': resourceValueHelper,
    // 'x-resourceHasLang': resourceHasLangHelper,
    //'x-resourceParamNames': resourceParamNamesHelper,
};

let _knownHelpers: KnownHelpers | undefined = undefined;

export function getKnownHelpers(): KnownHelpers {
    if (_knownHelpers === undefined) {
        _knownHelpers = Object.fromEntries(Object.keys(helpersList).map(key => [key, true]));
    }

    return _knownHelpers;
}

type HbsDataContext<T = Record<string, unknown>> = {
    name?: string;
    hash?: T;
    data?: Record<string, unknown>;
    fn?: (ctx: any) => any;
};

let dbgCounter = 0;
//let globalVarTemp: Record<string, unknown> = {};
let helpersTimeTaken: Record<string, number> = {};
const trackHelperTimes = false;

export function clearHelpersContext() {
    dbgCounter = 0;
    //globalVarTemp = {};
    //helpersTimeTaken = {};
}

function debugLogAndExec(helperName: string, fn: Function, ...args: any) {
    let debug = false;
    let header = '';
    let cnt = 0;

    if (arguments.length > 0) {
        const ctx = arguments[arguments.length - 1] as HbsDataContext;
        debug = valueAsBool(ctx.hash?._debug ?? false);
        if (debug) {
            cnt = ++dbgCounter;
            const debugLog = (ctx.hash?._debugLog ?? '').toString();
            const hash = copyObject(ctx.hash ?? {}, ['_debug', '_debugLog']);
            const restArgs = Array.from(args).slice(0, -1);
            header = `[${cnt}#${ctx.name}](${debugLog}) hash: ${JSON.stringify(hash, null, 0)}`;
            HostEnvironment.debugLog(`${header} ${JSON.stringify(restArgs, null, 0)}`);
        }
    }

    //HostEnvironment.debugLog(`[debugLogAndExec] fn: ${typeof fn} , arguments: ${JSON.stringify(arguments, null, 0)}, args: ${JSON.stringify(args, null, 0)}`);
    if (debug) {
        //HostEnvironment.debugLog(`${header}, arguments: ${JSON.stringify(arguments, null, 0)}, args: ${JSON.stringify(args, null, 0)}`);
    }

    let start = 0;
    if (trackHelperTimes) {
        start = Date.now();
    }

    // @ts-ignore
    const res = fn.call(this, ...args);

    if (trackHelperTimes) {
        const duration = Date.now() - start;
        helpersTimeTaken[helperName] = (helpersTimeTaken[helperName] ?? 0) + duration;
    }

    if (debug) {
        HostEnvironment.debugLog(`${header}, result: ${JSON.stringify(res, null, 0)}`);
    }

    return res;
}

export function debugHelpersTimeTaken(): void {
    if (!trackHelperTimes) {
        return;
    }

    let totalDuration = 0;
    Object.keys(helpersTimeTaken).forEach(key => {
        const duration = helpersTimeTaken[key] ?? 0;
        totalDuration += duration;

        HostEnvironment.debugLog(`helper '${key}' taken total: ${formatDuration(duration)}`);
    });

    HostEnvironment.debugLog(`All helpers taken total: ${formatDuration(totalDuration)}`);
}

function formatDuration(ms: number): string {
    const seconds = Math.floor(ms / 1000);
    const milliseconds = ms % 1000;

    return seconds > 0
        ? `${seconds} second${seconds > 1 ? 's' : ''} and ${milliseconds} ms`
        : `${milliseconds} ms`;
}

const fileHeader =
    `//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool - Localization HQ Editor.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------`;

function headerHelper(options: HbsDataContext) {
    return getRoot(options).host?.fileHeader ?? fileHeader;
}

function normalizePathHelper(context: any, options: HbsDataContext) {
    if (typeof context !== 'string') {
        return context;
    }

    let result = normalizePath(context);

    const replacePathSep = (options.hash?.replacePathSep || '') as string;
    if (!isNullOrEmpty(replacePathSep)) {
        result = result.split('/').join(replacePathSep);
    }

    return result;
}

function queryObjValue(context: any, options: HbsDataContext<{query?: string}>): any {
    let value = context;
    let query = options?.hash?.query;
    if (typeof options?.fn === 'function') {
        query = options.fn(context);
        if (!isNullOrEmpty(query) && typeof query === 'string') {
            query = removeNewLines(query);
        }
    }

    //if (!isNullOrEmpty(query) && typeof query === 'string' && typeof context === 'object') {
    if (!isNullOrEmpty(query) && typeof query === 'string' && !isNullOrEmpty(context)) {
        value = jsonQuery(context, query);
    }

    return value;
}

/**
 * @valueHelper
 * Works same as @modelDataHelper without storing result into @root.data, just returns into template to immediate use
 */
function valueHelper(context: any, options: HbsDataContext) {
    const defaultValue = options?.hash?.default;
    return queryObjValue(context, options) ?? defaultValue;
}

function indentHelper(count: number, options: any) {
    count = count < 0 ? 0 : count;
    // @ts-ignore
    var content = options.fn(this) as string;

    var paddedContent = content.split('\n')
        .map(line => '\t'.repeat(count) + line)
        .join('\n');

    return paddedContent;
}

function splitHelper(context: any, options: HbsDataContext) {
    if (typeof context !== 'string') return context;

    const sep = (options.hash?.sep || '') as string;
    if (isNullOrEmpty(sep)) {
        return context;
    }

    return context.split(sep);
}

function rawHelper(options: HbsDataContext) {
    // @ts-ignore
    const res = options.fn!(this);

    return new Handlebars.SafeString(res);
}

/*
{{#join people delimiter=" and " start="0" end="2"}}{{name}} ({{gender}}, {{age}}){{/join}}
<h1>Jobs</h1>
{{join jobs delimiter=", " start="1" end="2"}}
*/
function joinHelper(items: any[], options: HbsDataContext) {

    // if (dbgCounter === 0) {
    //     dbgCounter = 1;
    //     // @ts-ignore
    //     HostEnvironment.debugLog(`[joinHelper] items: ${typeof items}, ${items.name} options: ${JSON.stringify(options)}`);
    // }

    const delimiter = options.hash?.delimiter || ",";
    const start = (options.hash?.start || 0) as number;
    const len = (items ? items.length : 0) as number;
    let end = (options.hash?.end || len) as number;
    let out = "";
    const decorator = options.hash?.decorator || `"`;

    if (end > len) end = len;

    if ('function' === typeof options) {
        for (let i = start; i < end; i++) {
            if (i > start) out += delimiter;
            if ('string' === typeof items[i])
                out += items[i];
            else
                out += options(items[i]);
        }
        return out;
    } else {
        // @ts-ignore
        var res = [].concat(items).map(x => `${decorator}${x}${decorator}`).slice(start, end).join(delimiter);
        // @ts-ignore
        return new Handlebars.SafeString(res);
    }
}

// usage: {{ x-concat 'prop1' 'prop2' 'prop3' sep="," }}
function concatHelper(...args: any[]): string {
    const options = args.pop() as HbsDataContext;
    const sep = valueOrDefault(options.hash?.sep, '');

    return args.filter(x => !isNullOrEmpty(x)).join(sep);
}

function setCustomData(context: any, options: HbsDataContext, valueOrFn: () => any | any, forceToRoot: boolean): void {
    const value = typeof valueOrFn === 'function' ? valueOrFn() : valueOrFn;
    const key = valueOrDefault(options?.hash?.key, '');
    //const target = valueOrDefault<ITreeElement | undefined>(options?.hash?.target, undefined);

    //@ts-ignore
    //const context = this;

    if (!isNullOrEmpty(key)) {
        if (forceToRoot) {
            const root = getRoot(options);
            root.addToTempData(key, value);
        } else if (context instanceof TreeElement) {
            context.addToTempData(key, value);
        } else if (context instanceof TemplateRootModel) {
            //const root = getRoot(options);
            context.addToTempData(key, value);
        } else {
            HostEnvironment.debugLog(`[setCustomData] unknown context: ${typeof context} for key '${key}' !`);
        }
    }
}

/**
 * @replaceHelper
 * @example 
 * // This will replace "World" with "Universe" and result string will be "Hello Universe"
 * {{x-replace "Hello World" what="World" with="Universe"}}
 */
function replaceHelper(value: string, options: HbsDataContext): string {
    const what = valueOrDefault(options.hash?.what, '');
    const withStr = valueOrDefault(options.hash?.with, '');
    const regexopts = valueOrDefault(options.hash?.opts, 'g');
    const hasOpts = !isNullOrEmpty(options.hash?.opts);

    if (isNullOrEmpty(what) || isNullOrEmpty(withStr) || (what === withStr && !hasOpts)) {
        return value;
    }

    const regex = new RegExp(what, regexopts);
    return value.replace(regex, withStr);
}

// usage: {{x-trimEnd fullPath "/index.html"}}
function trimEndHelper(input: string, endPattern: string): string {
    try {
        const regex = new RegExp(endPattern + '$');
        return input.replace(regex, '');
    } catch (error) {
        HostEnvironment.debugLog('Invalid regex pattern:' + endPattern);
        return input;
    }
}

function equalsHelper(input: any, value: any, options: any): boolean {
    const { cs, val1, val2 } = getDataForCompare(input, value, options)
    return cs ? val1 === val2 : (val1.toLowerCase() === val2.toLowerCase());
}

function isTrueHelper(input: any): boolean {
    return input === true;
}

function isFalseHelper(input: any): boolean {
    return input === false
}

function getDataForCompare(input: any, value: any, options: any): { cs: boolean, val1: string, val2: string } {
    const cs = (options.hash?.cs || "true").toString().toLowerCase() == "true";
    const val1 = typeof input === "string" ? input : (input?.toString() ?? '');
    const val2 = typeof value === "string" ? value : (value?.toString() ?? '');
    return { cs: cs, val1, val2 };
}

function logicalHelper(input: any, value: any, options: any): boolean {
    const op = (options.hash?.op || 'and').toLowerCase();
    if (op === 'and') {
        //return input === value;
        return (input === true) && (value === true);
    } else if (op === 'or') {
        //return input || value;
        return (input === true) || (value === true);
    }

    return false;
}


// function resourceCommentHelper(resource: LhqModelResourceType, options: any): string {
//     if (typeof resource === 'object') {
//         const model = (options.hash?.root as TemplateRootModel).model;
//         const primaryLanguage = model?.model?.primaryLanguage ?? '';
//         if (!isNullOrEmpty(primaryLanguage) && resource.values) {
//             const resourceValue = resource.values[primaryLanguage]?.value;
//             let propertyComment = isNullOrEmpty(resourceValue) ? resource.description : resourceValue;
//             propertyComment = trimComment(propertyComment);
//             // @ts-ignore
//             return new Handlebars.SafeString(propertyComment);
//         }
//     }

//     return '';
// }

// function resourceValueHelper(resource: LhqModelResourceType, options: any): string {
//     if (typeof resource === 'object') {
//         const lang = options.hash?.lang ?? '';
//         const trim = options.hash?.trim ?? false;

//         if (!isNullOrEmpty(lang)) {
//             const res = resource?.values?.[lang]?.value ?? '';
//             return trim ? res.trim() : res;
//         }
//     }

//     return '';
// }

// function resourceHasLangHelper(resource: LhqModelResourceType, options: any): boolean {
//     if (typeof resource === 'object') {
//         const lang = options.hash?.lang ?? '';
//         if (!isNullOrEmpty(lang) && resource.values && resource.values[lang]) {
//             return true;
//         }
//     }

//     return false;
// }

// function resourceParamNamesHelper(resource: LhqModelResourceType, options: any): string {
//     if (typeof resource === 'object' && resource.parameters) {
//         const withTypes = options?.hash?.withTypes ?? false;

//         return Object.keys(resource.parameters).map(key => {
//             return withTypes ? `object ${key}` : key;
//         }).join(',');
//     }

//     return '';
// }

function mergeHelper(...args: any[]): any {
    const options = args.pop() as HbsDataContext;
    // @ts-ignore
    const context = args.length === 0 ? this : args.shift();

    Object.assign(context, ...args, options.hash ?? {});

    let result = undefined;
    if (typeof options?.fn === 'function') {
        try {
            result = options.fn(context);
        } finally {
            removeProperties(context, ...args, options.hash ?? {});
        }
    }

    return result;
}

function sortByHelper<T>(source: T[], propName?: string, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    return sortBy<T>(source, propName, sortOrder);
}

function sortObjectByKeyHelper(obj: Record<string, unknown>, options: any) {
    const sortOrder = options?.hash?.sortOrder ?? 'asc';
    return sortObjectByKey(obj, sortOrder);
}

function objCountHelper<T>(obj: Record<string, T> | Array<T> | undefined): number {
    return objCount<T>(obj);
}

function hasItemsHelper<T>(obj: Record<string, T> | Array<T> | undefined): boolean {
    return hasItems<T>(obj);
}

export function textEncodeHelper(str: string, options: HbsDataContext): string {
    const mode = valueOrDefault<TextEncodeModes>(options?.hash?.mode, 'html');
    const quotes = valueOrDefault(options?.hash?.quotes, false);
    const s = textEncode(str, { mode: mode, quotes });
    // @ts-ignore
    return new Handlebars.SafeString(s);
}

function hostWebHtmlEncodeHelper(str: string): string {
    if (isNullOrEmpty(str)) {
        return str;
    }

    const encoded = HostEnvironment.webHtmlEncode(str);

    // @ts-ignore
    return new Handlebars.SafeString(encoded);
}

function renderHelper(input: any, options: any): string {
    const when = options?.hash?.when ?? true;
    // @ts-ignore
    return when ? input : '';
}

function isNullOrEmptyHelper<T>(value: T | null | undefined | ''): value is undefined | null | '' {
    return isNullOrEmpty(value);
}

function isNotNullOrEmptyHelper(input: any): boolean {
    return !isNullOrEmpty(input);
}

function callFunctionHelper(fn: Function, ...args: any): any {
    let fnArgs = undefined;
    if (arguments.length > 0) {
        fnArgs = Array.from(args).slice(0, -1);
    }

    return fnArgs === undefined ? fn() : fn(...fnArgs);
}

function debugLogHelper(...args: any[]): string {
    HostEnvironment.debugLog(args.join(' '));
    return '';
}

function getRoot(options: HbsDataContext): TemplateRootModel {
    if (isNullOrEmpty(options) || isNullOrEmpty(options?.data)) {
        throw new AppError('Template has unknown definition for root data !');
    }

    return options.data['root'] as TemplateRootModel;
}

// function modelSettingsHelper(name: string, options: HbsDataContext) {
//     const root = getRoot(options);
//     //childs[?name=='ResX'].attrs | [0]
//     const settings = root.codeGenerator?.settings;
//     jsonQuery(settings, `childs[?name=='${name}`+"'].attrs | [0]");
// }

function stringifyHelper(context: any) {
    return new Handlebars.SafeString(JSON.stringify(context));
}

function toJsonHelper(context: any) {
    if (typeof context === 'string') {
        return JSON.parse(context);
    }

    return context;
}

function typeOfHelper(context: any) {
    if (typeof context === 'object') {
        return context.constructor ? context.constructor.name : 'object';
    } else {
        return context === undefined ? 'undefined' : `${context}[${typeof context}]`;
    }
}

/**
 * @dataHelper
 * @example
 * // Example 1: this will concat model.name and "Localizer" string into @root.data dictionary under key "rootClassName"
 * {{m-data (x-concat model.name "Localizer") key="rootClassName" }}
 * 
 * // Example 2: this will query (using jmespath expression) to get custom data and will be stored into @root.data dictionary under key "settings"
 * {{m-data model.codeGenerator.settings key="settings" query="childs[?name=='CSharp'].attrs | [0]" }}
 * 
 * // Example 3: if query is long it can be placed in block helper , like this:
 * {{#m-data model.codeGenerator.settings key="settings" }}
 * childs[?name=='CSharp'].attrs | [0].{
 *    bodySyntax: (UseExpressionBodySyntax||'false') == 'true',
 *    fallbackToPrimary: (MissingTranslationFallbackToPrimary||'false') == 'true',
 *    outputFolder: (OutputFolder||'Resources')
 * }
{{/m-data}}
 */
function modelDataHelper(context: any, options: HbsDataContext) {
    const defaultValue = options?.hash?.default;
    const value = queryObjValue(context, options) ?? defaultValue;

    const forceToRoot = options?.hash?.root ?? false;
    //@ts-ignore
    setCustomData(this, options, value, forceToRoot);
}

function modelOutputHelper(options: HbsDataContext<{ fileName: string, mergeWithDefaults: boolean }>) {
    //@ts-ignore
    const context = this;

    if (!(context instanceof TemplateRootModel)) {
        throw new AppError(`Helper '${options.name}' can be used only on TemplateRootModel (@root) type !`);
    }

    if (isNullOrEmpty(options.hash)) {
        throw new AppError(`Helper '${options.name}' missing hash properties !`);
    }

    let fileName = options?.hash?.fileName ?? '';

    // for child run, fileName can be (and most possible be) already set so take it!
    // if (context.templateRunType === 'child') {
    //     fileName = context.output?.fileName ?? '';
    // }

    // if fileName is not set in hash and also root context does not have set fileName then throw error
    if (isNullOrEmpty(fileName)) {
        throw new AppError(`Helper '${options.name}' missing file name !`);
    }


    const settingsNode = context.model.codeGenerator?.settings;

    const settingsObj = queryObjValue(settingsNode, options as any);
    if (isNullOrEmpty(settingsObj)) {
        throw new AppError(`Helper '${options.name}' could not find code gen settings from query !`);
    }

    const mergeWithDefaults = options?.hash?.mergeWithDefaults ?? true;

    const settings = Object.assign({}, mergeWithDefaults ? DefaultCodeGenSettings : {}, settingsObj);

    const outputFile: OutputFileData = {
        fileName: fileName,
        settings: settings
    };

    context.setOutput(outputFile);
}

function modelOutputChildHelper(options: HbsDataContext<{templateId: string, host?: Record<string, unknown>}>) {
    const context = getRoot(options);

    if (!(context instanceof TemplateRootModel)) {
        throw new AppError(`Helper '${options.name}' can be used only on TemplateRootModel (@root) type !`);
    }

    if (isNullOrEmpty(options.hash)) {
        throw new AppError(`Helper '${options.name}' missing hash properties !`);
    }

    const templateId = options.hash?.templateId;
    if (isNullOrEmpty(templateId)) {
        throw new AppError(`Helper '${options.name}' missing hash property 'templateId' !`);
    }

    context.addChildOutput(templateId, options.hash?.host);
}