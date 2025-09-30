import path from 'node:path';
import { glob } from 'glob';
import fse from 'fs-extra';

import { getGeneratedFileContent, getRootNamespaceFromCsProj } from '../src/generatorUtils.js';
import { GeneratedFile, GeneratorInitialization, HostEnvironment, LhqModel } from '../src/index.js';
import { Generator } from '../src/generator.js';
import { safeReadFile, verifyFile } from './testUtils.js';

import { folders } from './testUtils.js';


setTimeout(async () => {
    await initGenerator();

    const testFolders = await glob('*/', { cwd: folders().templates, nodir: false });
    //const testFolders = ['NetCoreResxCsharp01'];

    describe('Generating code from LHQ models', () => {
        testFolders.forEach((folder) => {
            describe(`Generator ${folder}`, async function () {
                this.slow(1000);
                it(`generate code from lhq`, async function () {
                    await generateFromLhq(folder);
                });
            });
        });

    });

    run();
}, 500);


async function generateFromLhq(folder: string): Promise<void> {
    const testDir = path.join(folders().templates, folder);
    const lhqFileName = path.join(testDir, 'Strings.lhq');
    const csProjectFiles = await glob('*.csproj', { cwd: testDir, nodir: true });
    const csProjectFile = path.join(testDir, csProjectFiles[0]);

    const csProjectContent = await safeReadFile(csProjectFile);
    const rootNamespace = getRootNamespaceFromCsProj('Strings.lhq', 'Strings.lhq.tt', csProjectFile, csProjectContent);

    const lhqFile = await safeReadFile(lhqFileName);
    const model = JSON.parse(lhqFile) as LhqModel;
    const data = { namespace: rootNamespace!.namespace };
    const generator = new Generator();
    const result = await generator.generate(lhqFileName, model, data);

    const generatedFolder = path.join(folders().snapshots, 'generated', folder);
    await fse.ensureDir(generatedFolder);

    await Promise.all(result.generatedFiles.map(async function (file) {
        await saveGenFile(file, generatedFolder);
        file.content = '';
    }));

    await verifyFile(path.join(generatedFolder, 'result.txt'), result);
}

async function saveGenFile(generatedFile: GeneratedFile, outputPath?: string): Promise<void> {
    const content = getGeneratedFileContent(generatedFile, true);
    const bom = generatedFile.bom ? '\uFEFF' : '';
    const buffer = Buffer.from(bom + content, 'utf8');

    const fileName = !outputPath ? generatedFile.fileName : path.join(outputPath, generatedFile.fileName);
    await verifyFile(fileName, buffer);
}


async function initGenerator() {
    const generatorInit: GeneratorInitialization = {
        hbsTemplates: {},
        hostEnvironment: new HostEnvironmentCli()
    };

    const hbsTemplatesDir = folders().hbs;

    const hbsFiles = await glob('*.hbs', { cwd: hbsTemplatesDir, nodir: true });

    const templateLoaders = hbsFiles.map(async (hbsFile) => {
        const templateId = path.basename(hbsFile, path.extname(hbsFile));
        const fullFilePath = path.join(hbsTemplatesDir, hbsFile);
        generatorInit.hbsTemplates[templateId] = await safeReadFile(fullFilePath);
    });

    await Promise.all(templateLoaders);

    Generator.initialize(generatorInit);
}

class HostEnvironmentCli extends HostEnvironment {
    public pathCombine(path1: string, path2: string): string {
        return path.join(path1, path2);
    }
}