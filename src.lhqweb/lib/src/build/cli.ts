import minimist from 'minimist';
import { generateSchema, validateLhqModel } from './utils';
import path from 'node:path';
import { isNullOrEmpty } from '../utils';


type CliArguments = {
    ['gen-schema']: boolean;
    ['validate-schema']: string;
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
}