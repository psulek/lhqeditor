import path from 'node:path';
import { expect } from 'chai';
import { glob } from 'glob';

import { getGeneratedFileContent, getRootNamespaceFromCsProj } from '../src/generatorUtils.js';
import { CSharpNamespaceInfo } from '../src/types.js';
import { createFilePath, safeReadFile, verify } from './testUtils.js';

import { folders } from './testUtils.js';
import { LhqModelLineEndings } from '../src/api/schemas.js';
import { GeneratedFile } from '../src/api/types.js';

setTimeout(async () => {

    const csProjectFiles = await glob('*.csproj', { cwd: folders().csproj, nodir: true });
    //const csProjectFiles = ['project04.csproj'];

    describe('Retrieving namespace information', () => {

        csProjectFiles.forEach((csProjectFile) => {
            const ident = csProjectFile.replace('.csproj', '');
            describe(`Namespaces ${ident}`, async function () {
                it(`retrieve namespace`, async function () {
                    // @ts-ignore
                    // const suite = this as Mocha.Suite;

                    const ns = await getNamespace(csProjectFile);
                    await verify('namespaces', ident, ns);
                });
            });
        });

        describe('Encodings', function () {
            async function testEncodings(endings: LhqModelLineEndings) {
                const encodingPath = path.join(folders().data, 'encodings');
                const testFilePath = path.join(encodingPath, `test_${endings.toLowerCase()}.txt`);
                let content = await safeReadFile(testFilePath);
                const generatedFile: GeneratedFile = { fileName: testFilePath, content, lineEndings: endings, bom: false };
                content = getGeneratedFileContent(generatedFile, true);

                const buffer = Buffer.from(content, 'utf8');

                await verify('encodings', `endings_${endings.toLowerCase()}`, buffer);
            }

            it(`Encode with CRLF`, async function () {

                await testEncodings('CRLF');
            });

            it(`Encode with LF`, async function () {
                await testEncodings('LF');
            });
        });
    });

    run();

}, 500);


async function getNamespace(csProjectFile: string): Promise<CSharpNamespaceInfo> {
    const fileName = path.join(folders().csproj, csProjectFile);
    const fp = createFilePath(fileName, folders().cwd, true, true);
    const csProjectContent = await safeReadFile(fileName);
    const rootNamespace = getRootNamespaceFromCsProj('Strings.lhq', 'Strings.lhq.tt', fileName, csProjectContent);

    expect(rootNamespace).to.not.be.undefined;

    rootNamespace!.csProjectFileName = fp.relative!;
    return rootNamespace!
}