const path = require('path');
// const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
    entry: './src/index.ts',
    mode: 'development',
    output: {
        library: {
            name: 'LhqGenerators',
            type: 'umd'
            // type: 'var'
        },
        filename: 'index.umd.js',
        path: path.resolve(__dirname, 'dist')
    },
    //target: ['web', 'es5'],
    target: 'web',
    devtool: false,
    resolve: {
        extensions: ['.ts', '.js']
    },
    // optimization: {
    //     minimize: true, // Enable minification in development mode
    //     minimizer: [
    //         new TerserPlugin({
    //             terserOptions: {
    //                 format: {
    //                     ecma: 5,
    //                     comments: false
    //                 },
    //                 mangle: false,
    //                 compress: false,
    //                 keep_classnames: true,
    //                 keep_fnames: true
    //             },
    //             extractComments: false,
    //         })
    //     ]
    // },
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
                    // {
                    //     loader: 'babel-loader', // Babel runs AFTER TypeScript has been compiled
                    //     options: {
                    //         presets: [
                    //             [
                    //                 '@babel/preset-env',
                    //                 {
                    //                     targets: '> 0.25%, not dead, IE 11', // Target older browsers (ES5)
                    //                     useBuiltIns: 'entry',  // Use polyfills
                    //                     corejs: 3              // Specify the version of core-js
                    //                 }
                    //             ]
                    //         ]
                    //     }
                    // },
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