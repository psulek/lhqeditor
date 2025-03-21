#!/usr/bin/env node

import fs from 'node:fs';
import minimist from 'minimist';
import path from 'node:path';
import type { ZodTypeAny } from 'zod';
import { printNode, zodToTs, createTypeAlias } from 'zod-to-ts'

import { isNullOrEmpty, jsonQuery, safeJsonParse } from '../utils';
import * as schemas from '../api/schemas';
import { generateFromLhq } from './gen';
import { Duration } from '../duration';
import { generateSchema, validateLhqModel } from '../generatorUtils';
import { findOwnerCsProjectFile } from './cliUtils';


type CliArguments = {
    ['gen-schema']: boolean;
    ['validate-schema']: string;
    ['gen-dts']: boolean;
    ['gen-lhq']: string;
};

const args = process.argv.slice(2);
const cliCmd = minimist(args) as unknown as CliArguments;

// Parse the arguments using minimist
const parsedArgs = minimist(args, {
    alias: {
        p: 'project',
        o: 'out',
        d: 'data'
    },
    string: ['project', 'out'],
    default: {
        out: '.',
        data: []
    }
});

if (parsedArgs.data) {
    // Ensure data is always an array (even if only one value is provided)
    if (!Array.isArray(parsedArgs.data)) {
        parsedArgs.data = [parsedArgs.data];
    }

    // Parse each key=value pair into an object
    parsedArgs.data = parsedArgs.data.map((pair: string) => {
        const [key, value] = pair.split('=');
        return { [key]: value };
    });
} else {
    parsedArgs.data = []; // Ensure data is an empty array if not provided
}

type CliActionType = 'generate' | 'validate';

const inputFile = parsedArgs._[0]; // The first non-option argument (file.lhq)
const cliAction: CliActionType = (parsedArgs._[1] as CliActionType) ?? 'generate';
const csProjectFileName = parsedArgs.project || parsedArgs.p; // MyProj.csproj
const outputDir = parsedArgs.out || parsedArgs.o; // outdir
const hostData = parsedArgs.data ?? {};

//const xx = findOwnerCsProjectFile(inputFile);
//const abc = findOwnerCsProjectFile(inputFile);

//testJMesPath();

if (cliAction === 'generate') {
    (async () => {
        await generateFromLhq(inputFile, csProjectFileName, outputDir, hostData);
    })();
}

// if (cliCmd['gen-schema'] === true) {
//     const cwd = process.cwd();
//     const schemaFilePath = path.join(cwd, 'lhq-schema.json');
//     const duration = Duration.start();
//     const schemaContent = generateSchema();
//     duration.end();
//     fs.writeFileSync(schemaFilePath, schemaContent, { encoding: 'utf-8' });

//     console.log(`Schema file '${schemaFilePath}' has been generated in ${duration.elapsedTime}.`);
// } else if (!isNullOrEmpty(cliCmd['gen-lhq'])) {
//     (async () => {
//         await generateFromLhq(cliCmd['gen-lhq']);
//     })();
// } else if (!isNullOrEmpty(cliCmd['validate-schema'])) {
//     const file = cliCmd['validate-schema'];
//     const lhqFile = fs.readFileSync(file, { encoding: 'utf-8' });
//     const model = safeJsonParse<schemas.LhqModel>(lhqFile);
//     const duration = Duration.start();
//     const valid = validateLhqModel(model);
//     duration.end();
//     console.log(`File '${file}' is valid: ${valid.success}${valid.success ? '' : `, ${valid.error}`} in ${duration.elapsedTime}.`);
// } else if (cliCmd['gen-dts'] === true) {

//     const typesFile = path.resolve(__dirname, '../model/types.ts');

//     // eslint-disable-next-line @typescript-eslint/ban-ts-comment
//     // @ts-ignore
//     const regexPattern = /import\s*{(?<imports>[^}]*)}\s*from\s*'\.\/schemas';/m;
//     const inputString = fs.readFileSync(typesFile, { encoding: 'utf-8' });
//     const match = inputString.match(regexPattern);
//     const importsStr = match?.groups?.['imports'].replace(/\s+/g, ' ').trim() ?? '';
//     const allowedSchemaTypes = importsStr.split(',').map((x: string) => x.trim());
//     if (allowedSchemaTypes.length === 0) {
//         throw new Error(`No zod schema types imports found in: ${typesFile}`);
//     }

//     const zodToStr = (zod: ZodTypeAny, identifier: string) => {
//         const { node } = zodToTs(zod, identifier);
//         const typeAlias = createTypeAlias(node, identifier);
//         const nodeString = printNode(typeAlias);
//         console.log(nodeString);
//     }

//     Object.entries(schemas).forEach(([key, value]) => {
//         const schemaName = key.replace(/Schema/i, '');
//         if (allowedSchemaTypes.includes(schemaName)) {
//             zodToStr(value, schemaName);
//         } else {
//             console.log(`Skipping schema: ${key}`);
//         }
//     });

//     //const {node} = zodToTs(schemas.lqhModelMetadataSchema, 'lqhModelMetadataSchema');
//     // const typeAlias = createTypeAlias(node, 'lqhModelMetadataSchema');
//     // const nodeString = printNode(typeAlias);
//     // console.log(nodeString);

//     // const tsType = ZodToTypescript.convert(schemas.lqhModelMetadataSchema, {export: false, name: 'lqhModelMetadataSchema'});
//     // console.log(tsType);
// }

function testJMesPath() {
    const query = `join(', ', map(&join(' ', ['object', @.name]), parameters))`;
    const obj = {
        'state': 'Edited',
        'parameters': [
          {
            'name': 'user',
            'description': 'user name',
            'order': 0
          },
          {
            'name': 'app',
            'description': 'app name',
            'order': 1
          }
        ],
        'values': {
          'en': {
            'value': 'Welcome {0} in this {1} !',
            'locked': true
          },
          'sk': {
            'value': 'Vitajte {0} v tejto {1} !',
            'auto': true
          }
        }
      };

    const result = jsonQuery(obj, query);
    console.log(result);
}