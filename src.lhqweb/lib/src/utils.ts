import { fromZodError } from 'zod-validation-error';
import { zodToJsonSchema } from "zod-to-json-schema";

// import { search as jmespath, JSONValue, registerFunction, TYPE_STRING } from '@metrichor/jmespath';
import { search as jmespath } from 'jmespath';
import { LhqModel, LhqModelSchema } from './model/api/schemas';

// export function testJMesPath() {
//     registerFunction('trimEnd',
//         (resolvedArgs) => {
//             const [input, endPattern] = resolvedArgs;
//             return trimEnd(input, endPattern);
//         },
//         [{ types: [TYPE_STRING] }, { types: [TYPE_STRING] }]);

//     const obj = {
//         "languages": [
//             "en",
//             "sk",
//             "de\\"
//         ]
//     };

//     const a1 = jmespath(obj, `languages[2]`);
//     const a2 = jmespath(obj, `trimEnd(languages[2],'[\\\\/]')`);
//     console.log(a1);
// }

declare global {
    interface String {
        isTrue(): boolean;
    }
}

// @ts-ignore
String.prototype.isTrue = function () {
    return this.toLowerCase() === "true";
};

export function safeJsonParse<T>(value: string): T {
    return JSON.parse(value.charCodeAt(0) === 0xFEFF ? value.slice(1) : value) as T;
}

export function jsonQuery<T>(obj: any, query: string, defaultValue?: T | undefined): T | undefined {
    return (jmespath(obj, query) as T | undefined) ?? defaultValue;
}
// export function jsonQuery<T, U>(obj: T, query: string): U | undefined {
//     return jmespath(obj, query) as U | undefined;
// }

export function normalizePath(path: string): string {
    return path
        .replace(/\\/g, '/') // Replace backslashes with forward slashes
        .replace(/\/\//g, '/') // Remove duplicate forward slashes
        .replace(/[\\/]$/g, '') // Remove trailing forward or back slash
}

export function getNestedPropertyValue<T, U>(obj: T, path: string, defaultValue?: U | undefined): U | undefined {
    const res = path.split('.').reduce((acc, part) => {
        if (acc === undefined) return undefined;

        // Check if the part includes an array index like "c[1]"
        const match = part.match(/^(\w+)\[(\d+)]$/);

        if (match) {
            const [, property, index] = match;
            // @ts-ignore
            return Array.isArray(acc[property]) ? acc[property][index] : undefined;
        }

        // @ts-ignore
        return acc![part];
    }, obj) as unknown as U;

    return res ?? defaultValue;
}

/**
 * Checks if a `value` is null, undefined or empty string.
 *
 * @typeParam T - The type of the value.
 * @param value - The value to check.
 *
 * @returns `true` if the value is null, undefined or empty string, `false` otherwise.
 */
export function isNullOrEmpty<T>(value: T | null | undefined | ''): value is undefined | null | '' {
    return value === null || value === undefined || value === '';
}

export function sortObjectByKey<T>(obj: Record<string, T>, sortOrder: 'asc' | 'desc' = 'asc'): Record<string, T> {
    return Object.fromEntries(
        Object.entries(obj).sort(([a], [b]) =>
            sortOrder === 'asc' ? a.localeCompare(b, 'en') : b.localeCompare(a, 'en')
        )
    );
}

export function sortObjectByValue<T>(obj: Record<string, T>, predicate: (item: T) => number | string,
    sortOrder: 'asc' | 'desc' = 'asc'): Record<string, T> {

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

export function sortBy<T>(source: T[], propName?: string, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    return source.concat([]).sort((a, b) => {
        // @ts-ignore
        const v1 = propName === undefined ? a : a[propName];
        // @ts-ignore
        const v2 = propName === undefined ? b : b[propName]
        const res = v1 > v2 ? 1 : ((v2 > v1) ? -1 : 0);
        return sortOrder === 'asc' ? res : res * -1;
    });
}

export function iterateObject<T>(obj: Record<string, T>, callback:
    (value: T, key: string, index: number, isLast: boolean) => void) {

    const entries = Object.entries(obj);
    if (entries.length > 0) {
        const lastIndex = entries.length - 1;
        let index = -1;
        for (const [key, value] of entries) {
            index++;
            callback(value, key, index, (index == lastIndex));
        }
    }
}

const encodingCharMaps: Record<string, Record<string, string>> = {
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

export type TextEncodeOptions =
    { mode: 'html' } |
    { mode: 'xml', quotes: boolean } |
    { mode: 'json' };

export type TextEncodeModes = Extract<TextEncodeOptions, { mode: any }>['mode'];

export function textEncode(str: string, encoder: TextEncodeOptions): string {
    if (isNullOrEmpty(str)) {
        return str;
    }

    const encodedChars = [];
    for (let i = 0; i < str.length; i++) {
        const ch = str.charAt(i);

        let map: Record<string, string> = undefined!;
        if (encoder.mode === 'html') {
            map = encodingCharMaps.html;
        } else if (encoder.mode === 'xml') {
            map = (encoder.quotes ?? true) ? encodingCharMaps.xml_quotes : encodingCharMaps.xml;
        } else {
            map = encodingCharMaps.json;
        }

        if (map.hasOwnProperty(ch)) {
            encodedChars.push(map[ch]);
        } else {
            encodedChars.push(ch);
        }
    }

    return encodedChars.join('');
}

export function valueOrDefault<T>(value: T | null | undefined | '' | unknown, defaultValue: T): T {
    let result = isNullOrEmpty(value)
        ? defaultValue
        : value;

    if (typeof defaultValue === 'boolean') {
        result = (valueAsBool(value) as unknown) as T;
    }

    return result as T;
}

export function trimComment(value: string): string {
    if (isNullOrEmpty(value)) {
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

export function valueAsBool(value: unknown): boolean {
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

export function toBoolean(value: string): boolean {
    return value.toLowerCase() === 'true';
}

export function hasItems<T>(obj: Record<string, T> | Array<T> | undefined): boolean {
    const result = objCount(obj) > 0;
    //HostEnv.debugLog(`[hasItems] returns '${result}' for obj: ${JSON.stringify(obj)}`);
    return result;
}

export function objCount<T>(obj: Record<string, T> | Array<T> | undefined): number {
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


export function copyObject<T extends Record<string, unknown>, K extends keyof T>(obj: T, keysToSkip: K[]): Omit<T, K> {
    return Object.fromEntries(
        Object.entries(obj).filter(([key]) => !keysToSkip.includes(key as K))
    ) as Omit<T, K>;
}

export function removeNewLines(input: string): string {
    if (isNullOrEmpty(input)) return input;
    return input.replace(/\r\n|\r|\n/g, '');
}

export function validateLhqModel(data: LhqModel): { success: boolean, error: string | undefined, model?: LhqModel } {
    if (data === undefined || data === null || typeof data !== 'object') {
        return { success: false, error: 'File or model must be specified.' };
    }

    const parseResult = LhqModelSchema.safeParse(data);
    const success = parseResult.success;
    const error = parseResult.success ? undefined : fromZodError(parseResult.error).toString();
    return { success, error, model: success ? parseResult.data : undefined };
}

export function generateSchema(schemaFilePath: string) {
    const jsonSchema = zodToJsonSchema(LhqModelSchema, {
        name: "LhqModel",
        $refStrategy: 'root'
    });

    return JSON.stringify(jsonSchema, null, 2);
}

export function removeProperties(obj: any, ...propertiesToRemove: any) {
    //@ts-ignore
    propertiesToRemove.forEach(propObj => {
        for (let key in propObj) {
            if (obj.hasOwnProperty(key)) {
                delete obj[key];
            }
        }
    });
}
