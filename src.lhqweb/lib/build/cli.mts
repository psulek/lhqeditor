#!/usr/bin/env node

import { generateFromLhq } from './gen.mjs';
import { Command } from '@commander-js/extra-typings';
import { validateLhqModelFile } from './cliUtils.mts';
import { isNullOrEmpty } from '../src/utils.mts';

const program = new Command();

const args = process.argv.slice(2);
const lhqfile = args.length > 0 ? args.shift() ?? '' : '';

const cliToolName = 'lhqcmd';

program
    .name(cliToolName)
    .description('Run various actions against LHQ files')
    .version('1.0.0')
    .argument('<lhqfile>', 'The LHQ project file *.lhq (e.g., Strings.lhq)')
    .addHelpText(
        'after',
        `
Examples:
  ${cliToolName} file.lhq generate --project MyProject.csproj --out ./output --data key1=value1
  ${cliToolName} file.lhq validate
`);

const generateCommand = new Command('generate')
    .description('Run template generator associated with LHQ file')
    .usage('<lhqfile> [options]')
    .option('-p, --project <project>', 'The C# project file *.csproj (e.g., MyProject.csproj)')
    .option('-o, --out <out>', 'The output directory', '.')
    .option('-d, --data <data...>', 'Key-value pairs for host data (e.g., key=value)')
    .action(async (options) => {
        let hostData: Record<string, string>;

        if (options.data) {
            hostData = options.data.reduce<{ [key: string]: string }>((acc, item) => {
                const [key, value] = (item as string).split('=');
                acc[key] = value;
                return acc;
            }, {});
        } else {
            hostData = {};
        }

        await generateFromLhq(lhqfile, options.project ?? '', options.out ?? '.', hostData);
    });

const validateCommand = new Command('validate')
    .description('Validate the input LHQ file')
    .usage('<lhqfile>')
    .action(async () => {
        await validateLhqModelFile(lhqfile);
    });


program.addCommand(generateCommand);
program.addCommand(validateCommand);


let rootCmd = 'generate';
if (args.length > 0 && ['generate', 'validate'].includes(args[0])) {
    rootCmd = args.shift()!;
}

if (args.length === 0 && isNullOrEmpty(lhqfile)) {
    program.outputHelp();
    process.exit(0);

}

(async () => {
    await program.parseAsync([rootCmd, ...args], { from: 'user' });
})();