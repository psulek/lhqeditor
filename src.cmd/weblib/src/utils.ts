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

export function arraySort<T>(source: T[], key: string, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    // @ts-ignore
    return arraySortBy<T>(source, x=> key === undefined ? x : x[key], sortOrder);
}

export function arraySortBy<T>(source: T[], predicate: (item: T) => number | string, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    return source.concat([]).sort((a, b) => {
        const v1 = predicate(a);
        const v2 = predicate(b);
        const res = v1 > v2 ? 1 : ((v2 > v1) ? -1 : 0);
        return sortOrder === 'asc' ? res : res * -1;
    });
}