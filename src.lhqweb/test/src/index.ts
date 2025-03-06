import * as fs from 'node:fs';
import {LhqModel, LhqModelSchema} from 'lhq-generators';
import { fromZodError } from 'zod-validation-error';

const file = process.argv[2];

const res = validateLhqModel(file);
console.log(res);


export function validateLhqModel(fileOrModel: string | LhqModel): { success: boolean, error: string | undefined } {
    let data: unknown;
    if (typeof fileOrModel === 'string') {
        if (fs.existsSync(fileOrModel)) {
            const json = fs.readFileSync(fileOrModel, { encoding: 'utf-8' });
            data = JSON.parse(json);
        }

        return { success: false, error: `File '${fileOrModel}' does not exist.` };
    } else if (typeof fileOrModel === 'object') {
        data = fileOrModel;
    }

    if (data === undefined || data === null) {
        return { success: false, error: 'File or model must be specified.' };
    }

    const parseResult = LhqModelSchema.safeParse(data);
    const success = parseResult.success;
    const error = parseResult.success ? undefined : fromZodError(parseResult.error).toString();
    return { success, error };
}