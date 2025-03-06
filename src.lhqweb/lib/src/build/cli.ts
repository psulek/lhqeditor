import fs from 'node:fs';
import minimist from 'minimist';
import { generateSchema, validateLhqModel } from './utils';
import path from 'node:path';
import { isNullOrEmpty } from '../utils';
import { printNode, zodToTs, createTypeAlias } from 'zod-to-ts'
//import { ZodToTypescript } from "@duplojs/zod-to-typescript";

import * as schemas from '../model/schemas';
import { ZodTypeAny } from 'zod';



type CliArguments = {
    ['gen-schema']: boolean;
    ['validate-schema']: string;
    ['gen-dts']: boolean;
};

const cliCmd = minimist(process.argv.slice(2)) as unknown as CliArguments;

if (cliCmd['gen-schema'] === true) {
    const cwd = process.cwd();
    const schemaFilePath = path.join(cwd, 'lhq-schema.json');
    generateSchema(schemaFilePath);

    console.log(`Schema file '${schemaFilePath}' has been generated.`);
} else if (!isNullOrEmpty(cliCmd['validate-schema'])) {
    const file = cliCmd['validate-schema'];
    const valid = validateLhqModel(file);
    console.log(`File '${file}' is valid: ${valid.success}${valid.success ? '' : `, ${valid.error}`}`);
} else if (cliCmd['gen-dts'] === true) {

    const typesFile = path.resolve(__dirname, '../model/types.ts');

    // @ts-ignore
    const regexPattern = /import\s*{(?<imports>[^}]*)}\s*from\s*'\.\/schemas';/m;
    const inputString = fs.readFileSync(typesFile, { encoding: 'utf-8' });
    const match = inputString.match(regexPattern);
    const importsStr = match?.groups?.imports.replace(/\s+/g, ' ').trim() ?? '';
    const allowedSchemaTypes = importsStr.split(',').map((x: string) => x.trim());
    if (allowedSchemaTypes.length === 0) {
        throw new Error(`No zod schema types imports found in: ${typesFile}`);
    }
    
    function capitalizeWord(str: string): string {
        if (!str) return str;
        return str.charAt(0).toUpperCase() + str.slice(1);
    };

    const zodToStr = (zod: ZodTypeAny, identifier: string) => {
        const { node } = zodToTs(zod, identifier);
        const typeAlias = createTypeAlias(node, identifier);
        const nodeString = printNode(typeAlias);
        console.log(nodeString);
    }

    Object.entries(schemas).forEach(([key, value]) => {
        const schemaName = key.replace(/Schema/i, '');
        if (allowedSchemaTypes.includes(schemaName)) {
            zodToStr(value, schemaName);
        } else {
            console.log(`Skipping schema: ${key}`);
        }
    });

    //const {node} = zodToTs(schemas.lqhModelMetadataSchema, 'lqhModelMetadataSchema');
    // const typeAlias = createTypeAlias(node, 'lqhModelMetadataSchema');
    // const nodeString = printNode(typeAlias);
    // console.log(nodeString);

    // const tsType = ZodToTypescript.convert(schemas.lqhModelMetadataSchema, {export: false, name: 'lqhModelMetadataSchema'});
    // console.log(tsType);
}