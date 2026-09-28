import path from 'node:path'
import { fileURLToPath } from 'node:url'
import HtmlWebpackPlugin from 'html-webpack-plugin'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

export default (env, argv) => ({
  mode: argv.mode === 'development' ? 'development' : 'production',
  entry: './src/main.tsx',
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: 'assets/[name].[contenthash].js',
    clean: true,
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.jsx', '.js'],
  },
  module: {
    rules: [
      {
        test: /\.(ts|tsx|js|jsx)$/,
        exclude: /node_modules/,
        use: 'babel-loader',
      },
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader'],
      },
    ],
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: './index.html',
      favicon: './public/favicon.svg',
    }),
  ],
  devServer: {
    port: 5173,
    open: false,
  },
})
