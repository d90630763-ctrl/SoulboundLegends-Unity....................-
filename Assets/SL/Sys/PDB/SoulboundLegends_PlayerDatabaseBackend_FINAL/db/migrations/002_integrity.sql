CREATE OR REPLACE FUNCTION prevent_negative_balance() RETURNS trigger LANGUAGE plpgsql AS $$ BEGIN IF NEW.balance < 0 THEN RAISE EXCEPTION 'negative balance prohibited'; END IF; RETURN NEW; END; $$;
DROP TRIGGER IF EXISTS trg_balance_nonnegative ON economy_balances;
CREATE TRIGGER trg_balance_nonnegative BEFORE INSERT OR UPDATE ON economy_balances FOR EACH ROW EXECUTE FUNCTION prevent_negative_balance();
CREATE UNIQUE INDEX IF NOT EXISTS ux_active_ruin_challenger ON supreme_ruins(challenger_player_id) WHERE state='InChallenge';
CREATE UNIQUE INDEX IF NOT EXISTS ux_ruin_claimed_god ON supreme_ruins(god_id) WHERE inheritance_owner_id IS NOT NULL;
