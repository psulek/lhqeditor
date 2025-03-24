//import fsp from 'node:fs/promises';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import fse from 'fs-extra';
import { glob } from 'glob';
import { getRootNamespaceFromCsProj, validateLhqModel } from '../src/generatorUtils.mjs';
import { LhqModel } from '../src/api/schemas.mjs';
import { isNullOrEmpty, jsonParseOrDefault, tryJsonParse, tryRemoveBOM } from '../src/utils.mjs';
import pc from "picocolors"


type CsProjectInfo = {
    csProjFileName: string;
    t4FileName: string;
    namespace: string;
};

export async function findOwnerCsProjectFile(lhqModelFileName: string): Promise<CsProjectInfo> {
    let csProjFileName = '';
    let t4FileName = '';
    let namespace = '';

    if (await fse.pathExists(lhqModelFileName)) {
        const dir = path.dirname(lhqModelFileName);

        const csProjectFiles = await glob('*.csproj', { cwd: dir, nodir: true });


        for (const csProj of csProjectFiles) {
            const csProjPath = path.join(dir, csProj);

            const ttFile = path.basename(lhqModelFileName) + '.tt';
            const csProjContent = await safeReadFile(csProjPath);

            let rootNamespace = getRootNamespaceFromCsProj(path.basename(lhqModelFileName), ttFile, csProjPath, csProjContent);
            if (isNullOrEmpty(rootNamespace) && !isNullOrEmpty(csProjPath)) {
                rootNamespace = path.basename(csProjPath, path.extname(csProjPath)).replace(' ', '_');
            }

            if (csProjFileName === '' || !isNullOrEmpty(namespace)) {
                csProjFileName = csProjPath;
                t4FileName = ttFile;
                namespace = rootNamespace ?? '';
            }
        }
    }

    return { csProjFileName, t4FileName, namespace };
}

export async function validateLhqModelFile(lhqFileName: string): Promise<void> {
    if (!await fse.pathExists(lhqFileName)) {
        throw new Error(`LHQ model file '${lhqFileName}' not found.`);
    }

    const lhqFile = await safeReadFile(lhqFileName);
    const model = jsonParseOrDefault<LhqModel>(lhqFile, {} as LhqModel, true);
    const valid = await validateLhqModel(model);
    const resultStr = valid.success ? pc.greenBright('VALID:') : pc.redBright('INVALID:');
    const fileStr = pc.cyanBright(lhqFileName);
    const validStr = valid.success 
    ? `File ${fileStr} has valid schema.` 
    : `File '${fileStr}' has invalid schema.\n${pc.redBright(valid.error ?? '')}`;

    console.log(`${pc.bold(resultStr)} ${validStr}`);
}

let currentScriptPath: {
    filename: string,
    dirname: string
} | undefined;

export function getCurrentScriptDir() {
    return getCurrentScriptInfo().dirname;
}

export function getCurrentScriptFile() {
    return getCurrentScriptInfo().filename;
}

export function getCurrentScriptInfo(): Exclude<typeof currentScriptPath, undefined> {
    if (!currentScriptPath) {
        const filename = fileURLToPath(import.meta.url);
        const dirname = path.dirname(filename);
        currentScriptPath = { filename, dirname };
    }

    return currentScriptPath;
}

export async function safeReadFile(fileName: string): Promise<string> {
    const content = await fse.readFile(fileName, { encoding: 'utf-8' });
    return isNullOrEmpty(content) ? '' : tryRemoveBOM(content);
}