// import path from 'node:path';
// import fse from 'fs-extra';
// import { glob } from 'glob';
// import pc from "picocolors"

// import { type LhqModel, generatorUtils, isNullOrEmpty, jsonParseOrDefault, tryRemoveBOM } from '../src/index';

// type CsProjectInfo = {
//     csProjFileName: string;
//     t4FileName: string;
//     namespace: string;
// };

// export async function findOwnerCsProjectFile(lhqModelFileName: string): Promise<CsProjectInfo> {
//     let csProjFileName = '';
//     let t4FileName = '';
//     let namespace = '';

//     if (await fse.pathExists(lhqModelFileName)) {
//         const dir = path.dirname(lhqModelFileName);

//         const csProjectFiles = await glob('*.csproj', { cwd: dir, nodir: true });


//         for (const csProj of csProjectFiles) {
//             const csProjPath = path.join(dir, csProj);

//             const ttFile = path.basename(lhqModelFileName) + '.tt';
//             const csProjContent = await safeReadFile(csProjPath);

//             let rootNamespace = generatorUtils.getRootNamespaceFromCsProj(path.basename(lhqModelFileName), ttFile, csProjPath, csProjContent);
//             if (isNullOrEmpty(rootNamespace) && !isNullOrEmpty(csProjPath)) {
//                 rootNamespace = path.basename(csProjPath, path.extname(csProjPath)).replace(' ', '_');
//             }

//             if (csProjFileName === '' || !isNullOrEmpty(namespace)) {
//                 csProjFileName = csProjPath;
//                 t4FileName = ttFile;
//                 namespace = rootNamespace ?? '';
//             }
//         }
//     }

//     return { csProjFileName, t4FileName, namespace };
// }

// export async function validateLhqModelFile(lhqFileName: string): Promise<void> {
//     if (!await fse.pathExists(lhqFileName)) {
//         throw new Error(`LHQ model file '${lhqFileName}' not found.`);
//     }

//     const lhqFile = await safeReadFile(lhqFileName);
//     const model = jsonParseOrDefault<LhqModel>(lhqFile, {} as LhqModel, true);
//     const valid = await generatorUtils.validateLhqModel(model);
//     const resultStr = valid.success ? pc.greenBright('VALID:') : pc.redBright('INVALID:');
//     const fileStr = pc.cyanBright(lhqFileName);
//     const validStr = valid.success 
//     ? `File ${fileStr} has valid schema.` 
//     : `File '${fileStr}' has invalid schema.\n${pc.redBright(valid.error ?? '')}`;

//     console.log(`${pc.bold(resultStr)} ${validStr}`);
// }

// export async function safeReadFile(fileName: string): Promise<string> {
//     const content = await fse.readFile(fileName, { encoding: 'utf-8' });
//     return isNullOrEmpty(content) ? '' : tryRemoveBOM(content);
// }