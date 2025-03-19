const path = require('path');

module.exports = {
    entry: './src/index.ts',
    mode: 'development',
    output: {
        library: {
            name: 'LhqGenerators',
            type: 'var'
        },
        filename: 'lhqgenerators.js',
        path: path.resolve(__dirname, 'dist')
    },
    target: 'web',
    devtool: 'hidden-source-map',
    resolve: {
        extensions: ['.ts', '.js']
    },
    module: {
        rules: [
            /* {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/
            }, */
            {
                test: /\.ts$/,
                use: [
                    {
                        loader: 'ts-loader',
                        options: {
                            logInfoToStdOut: true,
                            logLevel: 'warn',
                            colors: true,
                            configFile: 'tsconfig.json',
                            onlyCompileBundledFiles: true,
                            transpileOnly: false
                        }
                    }
                ]
            }
        ]
    }
};