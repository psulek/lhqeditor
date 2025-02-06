//const he = require('he');

// export function htmlEncode(value: string, options: any): string {
//     return he.encode(value, options);
// }

import {HostEnv} from "./hostEnv";

export function getNestedPropertyValue<T, U>(obj: T, path: string): U {
    return path.split('.').reduce((acc, part) => {
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
            const isLast = index == lastIndex;
            callback(value, key, index, isLast);
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

export function valueOrDefault<T>(value: T | null | undefined | '', defaultValue: T): T {
    let result = isNullOrEmpty(value)
        ? defaultValue
        : value;

    if (typeof defaultValue === 'boolean') {
        result = (valueAsBool(value) as unknown) as T;
    }

    return result;
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