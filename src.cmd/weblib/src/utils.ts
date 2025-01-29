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

export function sortBy<T>(source: T[], propName: string, sortOrder: 'asc' | 'desc' = 'asc'): T[] {
    return source.concat([]).sort((a, b) => {
        // @ts-ignore
        const v1 = propName === undefined ? a : a[propName];
        // @ts-ignore
        const v2 = propName === undefined ? b : b[propName]
        const res = v1 > v2 ? 1 : ((v2 > v1) ? -1 : 0);
        return sortOrder === 'asc' ? res : res * -1;
    });
}