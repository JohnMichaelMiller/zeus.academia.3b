# AI Conversation Log

- Chat ID: 2026-02-25-custom-agents-instructions-prompt
- Operator: johnmillerATcodemag-com
- Model: openai/gpt-5.3-codex@unknown
- Started: 2026-02-25T00:00:00Z
- Ended: 2026-02-25T00:15:00Z
- Total Duration: 00:15:00

## Context

- Repository: zeus.academia.3b
- Branch: Part-Six
- Task: Create a prompt template that generates an instruction file for GitHub Copilot custom agents
- Sources:
  - https://docs.github.com/en/copilot/reference/custom-agents-configuration
  - https://docs.github.com/en/copilot/concepts/agents/coding-agent/about-custom-agents
  - https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents

## Request

User requested a prompt that will generate an instruction file for creating agents, using the three GitHub Copilot custom-agent documentation pages.

## Exchanges

[2026-02-25T00:00:00Z] User

```text
using (https://docs.github.com/en/copilot/reference/custom-agents-configuration), https://docs.github.com/en/copilot/concepts/agents/coding-agent/about-custom-agents, https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents
 create a prompt that will create an instruction file for creating agents
```

[2026-02-25T00:10:00Z] openai/gpt-5.3-codex@unknown

Created a new reusable prompt template with:

- YAML metadata and prompt arguments
- Required sections for agent profile structure, tools, MCP, precedence, and validation
- Explicit cross-reference requirement to project overview instructions
- Safety and environment-differences guidance

## Artifacts

- `.github/prompts/create-custom-agents-instructions.prompt.md` - Prompt template for generating custom-agent instruction files
- `README.md` - Added artifact index entry for the new prompt

## Task Breakdown

1. Reviewed repository prompt conventions
2. Reviewed GitHub custom-agent docs requirements
3. Authored prompt with required schema and checklists
4. Added AI log and summary references
5. Updated README artifact list

## Next Steps

- [ ] Run the new prompt to generate `.github/instructions/custom-agents.instructions.md`
- [ ] Add the generated instruction file to project overview standards
- [ ] Validate the generated file against the latest GitHub docs
