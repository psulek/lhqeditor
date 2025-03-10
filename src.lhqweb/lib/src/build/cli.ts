import fs from 'node:fs';
import minimist from 'minimist';
import path from 'node:path';
import { generateSchema, isNullOrEmpty, safeJsonParse, validateLhqModel } from '../utils';
import { printNode, zodToTs, createTypeAlias } from 'zod-to-ts'

import * as schemas from '../model/api/schemas';
import { ZodTypeAny } from 'zod';
import { generateFromLhq } from './gen';


type CliArguments = {
    ['gen-schema']: boolean;
    ['validate-schema']: string;
    ['gen-dts']: boolean;
    ['gen-lhq']: string;
};

const cliCmd = minimist(process.argv.slice(2)) as unknown as CliArguments;

// const f1 = fs.readFileSync('C:\\dev\\github\\psulek\\lhqeditor\\src.lhqweb\\lib\\data\\Strings.lhq', { encoding: 'utf-8' });
// const f1Content = f1.charCodeAt(0) === 0xFEFF ? f1.slice(1) : f1; // Remove BOM if it exists
// const f1Obj = JSON.parse(f1Content);
// //const v1 = getValue(f1Json, 'languages.1', {default: 'wtf'});
// const v1 = getNestedPropertyValue<schemas.LhqModel, string>(f1Obj, 'languages[1]');
// console.log('>> ', v1);


//testJMesPath();

if (cliCmd['gen-schema'] === true) {
    const cwd = process.cwd();
    const schemaFilePath = path.join(cwd, 'lhq-schema.json');
    const schemaContent = generateSchema(schemaFilePath);
    fs.writeFileSync(schemaFilePath, JSON.stringify(schemaContent, null, 2), { encoding: 'utf-8' });

    console.log(`Schema file '${schemaFilePath}' has been generated.`);
} else if (!isNullOrEmpty(cliCmd['gen-lhq'])) {
    (async () => {
        await generateFromLhq(cliCmd['gen-lhq']);
    })();
} else if (!isNullOrEmpty(cliCmd['validate-schema'])) {
    const file = cliCmd['validate-schema'];
    const lhqFile = fs.readFileSync(file, { encoding: 'utf-8' });
    const model = safeJsonParse<schemas.LhqModel>(lhqFile);
    const valid = validateLhqModel(model);
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