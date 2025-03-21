import * as fse from 'fs-extra';
import * as path from 'path';
import { glob } from 'glob';

import { getRootNamespace } from '../generatorUtils';
import { isNullOrEmpty, tryRemoveBOM } from '../utils';

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

        const csProjectFiles = await glob('*.csproj', {cwd: dir, nodir: true });


        for (const csProj of csProjectFiles) {
            const csProjPath = path.join(dir, csProj);

            const ttFile = path.basename(lhqModelFileName) + '.tt';
            const csProjContent = tryRemoveBOM(await fse.readFile(csProjPath, { encoding: 'utf-8' }));

            let rootNamespace = getRootNamespace(path.basename(lhqModelFileName), ttFile, csProjPath, csProjContent);
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