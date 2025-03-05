import * as fs from 'node:fs';
import { fromZodError } from 'zod-validation-error';
import { zodToJsonSchema } from "zod-to-json-schema";

import { LhqModel, lhqModelSchema } from '../model/schemas';

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

    const parseResult = lhqModelSchema.safeParse(data);
    const success = parseResult.success;
    const error = parseResult.success ? undefined : fromZodError(parseResult.error).toString();
    return { success, error };
}

export function generateSchema(schemaFilePath: string) {
    const jsonSchema = zodToJsonSchema(lhqModelSchema, {
        name: "LhqModel",
        $refStrategy: 'root'
    });

    fs.writeFileSync(schemaFilePath, JSON.stringify(jsonSchema, null, 2), { encoding: 'utf-8' });
}