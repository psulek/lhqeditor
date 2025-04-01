import { expect } from 'chai';
import {
    tryRemoveBOM,
    jsonParseOrDefault,
    tryJsonParse,
    jsonQuery,
    normalizePath,
    isNullOrEmpty,
    isNullOrUndefined,
    sortObjectByKey,
    sortBy,
    arraySortBy,
    iterateObject,
    valueOrDefault,
    hasItems,
    objCount,
    removeNewLines,
    removeProperties
} from '../src/utils';

describe('Utils Functions', () => {
    describe('tryRemoveBOM', () => {
        it('should remove BOM from a string', () => {
            const input = '\uFEFFtest';
            const result = tryRemoveBOM(input);
            expect(result).to.equal('test');
        });

        it('should return the same string if no BOM exists', () => {
            const input = 'test';
            const result = tryRemoveBOM(input);
            expect(result).to.equal('test');
        });
    });

    describe('jsonParseOrDefault', () => {
        it('should parse valid JSON', () => {
            const input = '{"key": "value"}';
            const result = jsonParseOrDefault(input, {});
            expect(result).to.deep.equal({ key: 'value' });
        });

        it('should return default value for invalid JSON', () => {
            const input = 'invalid';
            const result = jsonParseOrDefault(input, { key: 'default' });
            expect(result).to.deep.equal({ key: 'default' });
        });
    });

    describe('tryJsonParse', () => {
        it('should return success for valid JSON', () => {
            const input = '{"key": "value"}';
            const { success, data } = tryJsonParse(input);
            expect(success).to.be.true;
            expect(data).to.deep.equal({ key: 'value' });
        });

        it('should return error for invalid JSON', () => {
            const input = 'invalid';
            const { success, error } = tryJsonParse(input);
            expect(success).to.be.false;
            expect(error).to.be.a('string');
        });
    });

    describe('jsonQuery', () => {
        it('should return the result of a JMESPath query', () => {
            const obj = { key: { nested: 'value' } };
            const query = 'key.nested';
            const result = jsonQuery(obj, query);
            expect(result).to.equal('value');
        });

        it('should return default value if query result is undefined', () => {
            const obj = { key: { nested: 'value' } };
            const query = 'key.nonexistent';
            const result = jsonQuery(obj, query, 'default');
            expect(result).to.equal('default');
        });
    });

    describe('normalizePath', () => {
        it('should normalize backslashes to forward slashes', () => {
            const input = 'C:\\path\\to\\file';
            const result = normalizePath(input);
            expect(result).to.equal('C:/path/to/file');
        });

        it('should remove trailing slashes', () => {
            const input = 'C:/path/to/file/';
            const result = normalizePath(input);
            expect(result).to.equal('C:/path/to/file');
        });
    });

    describe('isNullOrEmpty', () => {
        it('should return true for null, undefined, or empty string', () => {
            expect(isNullOrEmpty(null)).to.be.true;
            expect(isNullOrEmpty(undefined)).to.be.true;
            expect(isNullOrEmpty('')).to.be.true;
        });

        it('should return false for non-empty values', () => {
            expect(isNullOrEmpty('value')).to.be.false;
            expect(isNullOrEmpty(0)).to.be.false;
        });
    });

    describe('isNullOrUndefined', () => {
        it('should return true for null or undefined', () => {
            expect(isNullOrUndefined(null)).to.be.true;
            expect(isNullOrUndefined(undefined)).to.be.true;
        });

        it('should return false for defined values', () => {
            expect(isNullOrUndefined('value')).to.be.false;
            expect(isNullOrUndefined(0)).to.be.false;
        });
    });

    describe('sortObjectByKey', () => {
        it('should sort object keys in ascending order', () => {
            const input = { b: 2, a: 1, c: 3 };
            const result = sortObjectByKey(input, 'asc');
            expect(result).to.deep.equal({ a: 1, b: 2, c: 3 });
        });

        it('should sort object keys in descending order', () => {
            const input = { b: 2, a: 1, c: 3 };
            const result = sortObjectByKey(input, 'desc');
            expect(result).to.deep.equal({ c: 3, b: 2, a: 1 });
        });
    });

    describe('sortBy', () => {
        it('should sort an array of objects by a key in ascending order', () => {
            const input = [{ key: 3 }, { key: 1 }, { key: 2 }];
            const result = sortBy(input, 'key', 'asc');
            expect(result).to.deep.equal([{ key: 1 }, { key: 2 }, { key: 3 }]);
        });

        it('should sort an array of objects by a key in descending order', () => {
            const input = [{ key: 1 }, { key: 3 }, { key: 2 }];
            const result = sortBy(input, 'key', 'desc');
            expect(result).to.deep.equal([{ key: 3 }, { key: 2 }, { key: 1 }]);
        });
    });

    describe('arraySortBy', () => {
        it('should sort an array by a key in ascending order', () => {
            const input = [{ key: 3 }, { key: 1 }, { key: 2 }];
            const result = arraySortBy(input, x => x.key, 'asc');
            expect(result).to.deep.equal([{ key: 1 }, { key: 2 }, { key: 3 }]);
        });

        it('should sort an array by a key in descending order', () => {
            const input = [{ key: 1 }, { key: 3 }, { key: 2 }];
            const result = arraySortBy(input, x => x.key, 'desc');
            expect(result).to.deep.equal([{ key: 3 }, { key: 2 }, { key: 1 }]);
        });
    });

    describe('iterateObject', () => {
        it('should iterate over object properties', () => {
            const input = { a: 1, b: 2 };
            const result: Array<{ key: string; value: number }> = [];
            iterateObject(input, (value, key) => result.push({ key, value }));
            expect(result).to.deep.equal([{ key: 'a', value: 1 }, { key: 'b', value: 2 }]);
        });
    });

    // describe('textEncode', () => {
    //     it('should encode text to base64', () => {
    //         const input = 'test';
    //         const result = textEncode(input, {mode: ''});
    //         expect(result).to.equal('dGVzdA==');
    //     });
    // });

    describe('valueOrDefault', () => {
        it('should return the value if defined', () => {
            const value = 'test';
            const result = valueOrDefault(value, 'default');
            expect(result).to.equal('test');
        });

        it('should return the default if value is undefined', () => {
            const result = valueOrDefault(undefined, 'default');
            expect(result).to.equal('default');
        });
    });

    // describe('trimComment', () => {
    //     it('should not change string', () => {
    //         const input = 'code comment';
    //         const result = trimComment(input);
    //         expect(result).to.equal(input);
    //     });

    //     it('should remove newline from a string', () => {
    //         const input = 'code \n comment';
    //         const result = trimComment(input);
    //         expect(result).to.equal('code ...');
    //     });

    //     it('should remove newline#2 from a string', () => {
    //         const input = 'code \r\n comment';
    //         const result = trimComment(input);
    //         expect(result).to.equal('code ...');
    //     });

    //     it('should remove newline#3 from a string', () => {
    //         const input = 'code \r comment';
    //         const result = trimComment(input);
    //         expect(result).to.equal('code ...');
    //     });

    //     it('should replace tab with space', () => {
    //         const input = 'code\tcomment';
    //         const result = trimComment(input);
    //         expect(result).to.equal('code comment');
    //     });

    //     it('should replace tab#2 with space', () => {
    //         const input = 'code-\t\t-123';
    //         const result = trimComment(input);
    //         expect(result).to.equal('code-  -123');
    //     });

    //     it('should not trim when exact 80 chars', () => {
    //         const input = Array(80).fill(null).map(() => String.fromCharCode(33 + Math.floor(Math.random() * 94))).join('');
    //         const result = trimComment(input);
    //         expect(result).to.equal(input);
    //     });

    //     it('should trim when 81 chars', () => {
    //         const input = Array(81).fill(null).map(() => String.fromCharCode(33 + Math.floor(Math.random() * 94))).join('');
    //         const result = trimComment(input);
    //         expect(result).to.equal(input.substring(0, 80) + '...');
    //     });
    // });

    // describe('valueAsBool', () => {
    //     it('should return true for truthy values', () => {
    //         expect(valueAsBool('true')).to.be.true;
    //         expect(valueAsBool(1)).to.be.true;
    //     });

    //     it('should return false for falsy values', () => {
    //         expect(valueAsBool('false')).to.be.false;
    //         expect(valueAsBool(0)).to.be.false;
    //     });
    // });

    // describe('toBoolean', () => {
    //     it('should convert truthy values to true', () => {
    //         expect(toBoolean('true')).to.be.true;
    //         expect(toBoolean('TRUE')).to.be.true;
    //     });

    //     it('should convert falsy values to false', () => {
    //         expect(toBoolean('false')).to.be.false;
    //         expect(toBoolean('0')).to.be.false;
    //         expect(toBoolean('1')).to.be.false;
    //         expect(toBoolean('false')).to.be.false;
    //         expect(toBoolean('false')).to.be.false;
    //     });
    // });

    describe('hasItems', () => {
        it('should return true for arrays with items', () => {
            expect(hasItems([1, 2, 3])).to.be.true;
        });

        it('should return false for empty arrays', () => {
            expect(hasItems([])).to.be.false;
        });
    });

    describe('objCount', () => {
        it('should return the number of properties in an object', () => {
            const input = { a: 1, b: 2 };
            const result = objCount(input);
            expect(result).to.equal(2);
        });
    });

    // describe('copyObject', () => {
    //     it('should create a deep copy of an object', () => {
    //         const input = { a: 1, b: { c: 2 } };
    //         const result = copyObject(input);
    //         expect(result).to.deep.equal(input);
    //         expect(result).to.not.equal(input);
    //     });


    //     it('should create a deep copy of an object without skipping any keys', () => {
    //         const input = { a: 1, b: { c: 2 } };
    //         const result = copyObject(input, []);
    //         expect(result).to.deep.equal(input);
    //         expect(result).to.not.equal(input);
    //     });

    //     it('should create a deep copy of an object and skip specified keys', () => {
    //         const input = { a: 1, b: 2, c: 3 };
    //         const result = copyObject(input, ['b', 'c']);
    //         expect(result).to.deep.equal({ a: 1 });
    //     });

    //     it('should handle skipping keys that do not exist in the object', () => {
    //         const input = { a: 1, b: 2 };
    //         // @ts-ignore
    //         const result = copyObject(input, ['c']);
    //         expect(result).to.deep.equal(input);
    //     });

    //     it('should return an empty object if all keys are skipped', () => {
    //         const input = { a: 1, b: 2 };
    //         const result = copyObject(input, ['a', 'b']);
    //         expect(result).to.deep.equal({});
    //     });

    //     it('should not modify the original object when skipping keys', () => {
    //         const input = { a: 1, b: 2, c: 3 };
    //         const result = copyObject(input, ['b']);
    //         expect(result).to.deep.equal({ a: 1, c: 3 });
    //         expect(input).to.deep.equal({ a: 1, b: 2, c: 3 });
    //     });
    // });

    describe('removeNewLines', () => {
        it('should remove new lines from a string', () => {
            const input = 'line1\nline2';
            const result = removeNewLines(input);
            expect(result).to.equal('line1line2');
        });
    });

    describe('removeProperties', () => {
        it('should remove specified properties from an object', () => {
            const input = { a: 1, b: 2, c: 3 };
            const result = removeProperties(input, {b: undefined, c: undefined});
            expect(result).to.deep.equal({ a: 1 });
        });

        it('should remove properties from multiple objects', () => {
            const input = { a: 1 };

            Object.assign(input, {b: 2}, {c: 3});

            const result = removeProperties(input, {b: 2}, {c: 3});
            
            expect(result).to.deep.equal({ a: 1 });
            expect(result).to.equal(input);
        });
    });
});