const plugins = [
  require('autoprefixer')
]

if (process.env.QUASAR_RTL) {
  plugins.push(
    require('postcss-rtl')({})
  )
}

module.exports = {
  plugins
}
