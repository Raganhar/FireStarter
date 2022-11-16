namespace firestarter.LegacyFlows;

public static class apfe_verify
{
    public static string content = @"name: verify

on:
  pull_request:

jobs:
  verify:
    runs-on: ubuntu-latest
    environment: dev
    steps:
      - name: ""Checkout""
        uses: actions/checkout@v3
        with:
          ref: ${{ github.ref	}}
      - uses: actions/setup-node@v3
      - run: npm install
      - run: npm run lint 
";
}