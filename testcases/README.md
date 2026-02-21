# Linkset Test Cases

Shared, language-independent test cases for the Linkset component.

## Format

Each test case is a JSON file containing a valid or invalid `application/linkset+json`
document as defined in [RFC 9264](https://www.rfc-editor.org/rfc/rfc9264.html).

### Positive Tests

Files in the root of this directory are valid linkset documents that parsers
MUST accept and round-trip correctly.

| File | Description |
|------|-------------|
| `single-link.json` | Minimal linkset with one link |
| `multiple-links.json` | Linkset with multiple links and relation types |
| `all-properties.json` | Link using all defined properties |
| `rfc9264-example.json` | Example from RFC 9264 with anchors |
| `language-variants.json` | Links with `hreflang` for language negotiation |

### Negative Tests

Files in the `negative/` subdirectory are invalid documents that parsers
SHOULD reject with appropriate errors.

| File | Description |
|------|-------------|
| `missing-linkset-property.json` | Top-level object without `linkset` array |
| `missing-href.json` | Link entry missing required `href` |
| `invalid-json.txt` | Malformed JSON |
