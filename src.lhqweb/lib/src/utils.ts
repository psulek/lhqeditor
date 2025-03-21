import { search as jmespath } from 'jmespath';

export function tryRemoveBOM(value: string): string {
    return isNullOrEmpty(value) ? value : (value.charCodeAt(0) === 0xFEFF ? value.slice(1) : value);
}

export function safeJsonParse<T>(value: string): T {
    return JSON.parse(tryRemoveBOM(value)) as T;
}

export function jsonQuery<T>(obj: unknown, query: string, defaultValue?: T): T | undefined {
    return (jmespath(obj, query) as T | undefined) ?? defaultValue;
}

export function normalizePath(path: string): string {
    return path
        .replace(/\\/g, '/') // Replace backslashes with forward slashes
        .replace(/\/\//g, '/') // Remove duplicate forward slashes
        .replace(/[\\/]$/g, '') // Remove trailing forward or back slash
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

export function isNullOrUndefined<T>(value: T | null | undefined): value is undefined | null {
    return value === null || value === undefined;
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

export type KeysMatching<T, V> = { [K in keyof T]-?: T[K] extends V ? K : never }[keyof T];

export function sortBy<T>(source: T[], key: KeysMatching<T, string | number> | undefined, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    // NOTE: dirty hack as unknown to be able to use raw x value when key is undefined
    return arraySortBy(source, x => (key === undefined ? x : x[key]) as unknown as number, sortOrder);
}

export function arraySortBy<T>(source: T[], predicate: (item: T) => number | string, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    return source.concat([]).sort((a, b) => {
        const v1 = predicate(a);
        const v2 = predicate(b);
        const res = v1 > v2 ? 1 : ((v2 > v1) ? -1 : 0);
        return sortOrder === 'asc' ? res : res * -1;
    });
}

export function iterateObject<T>(obj: Record<string, T>, callback:
    (value: T, key: string, index: number, isLast: boolean) => void): void {

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

export type TextEncodeOptions =
    { mode: 'html' } |
    { mode: 'xml', quotes: boolean } |
    { mode: 'json' };

export type TextEncodeModes = Extract<TextEncodeOptions, { mode: unknown }>['mode'];

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

// eslint-disable-next-line @typescript-eslint/no-redundant-type-constituents
export function valueOrDefault<T>(value: T | null | undefined | '' | unknown, defaultValue: T): T {
    let result = isNullOrUndefined(value) ? defaultValue : value;

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
    let idxNewLine = value.indexOf('\r\n');

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
        value += '...';
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
    return objCount(obj) > 0;
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


export function removeProperties<T>(obj: T | undefined, ...propertiesToRemove: unknown[]): T | undefined {
    if (isNullOrUndefined(obj)) return obj;

    propertiesToRemove.forEach(propObj => {
        // eslint-disable-next-line @typescript-eslint/ban-ts-comment
        // @ts-ignore
        for (const key in propObj) {
            if ((obj as object).hasOwnProperty(key)) {
                // eslint-disable-next-line @typescript-eslint/ban-ts-comment
                // @ts-ignore
                delete obj[key];
            }
        }
    });

    return obj;
}